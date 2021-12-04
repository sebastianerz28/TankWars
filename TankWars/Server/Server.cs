using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using GameModel;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TankWars;

namespace TankWars
{
    public class Server
    {
        private const int TankSpeed = 3;
        private const int ProjSpeed = 25;
        private const int TankSize = 60;

        private int UniverseSize = -1;
        private int MSPerFrame = -1;
        private int FramesPerShot = -1;
        private int RespawnRate = -1;

        private int wallCount = 0;
        private int tankID = 0;
        private int projectileID = 0;
        private int beamID = 0;
        private int powerupID = 0;
        private int powerupCounter = 0;


        private Vector2D newTankLoc = new Vector2D(0, 0);

        private Dictionary<long, int> tankIDs;
        private Dictionary<int, Vector2D> tankVelocities;
        private Dictionary<long, Socket> sockets;

        public World world = new World();

        private Stopwatch watch = new Stopwatch();

        static void Main(string[] args)
        {
            Server s = new Server();
            s.Run();
            Console.Read();
        }
        public Server()
        {
            tankIDs = new Dictionary<long, int>();
            tankVelocities = new Dictionary<int, Vector2D>();
            sockets = new Dictionary<long, Socket>();
        }

        public void Run()
        {
            try
            {
                ReadSettingsXml();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            TcpListener listener = Networking.StartServer(SetupMessageReceive, 11000);
            watch.Start();
            while (true)
            {
                while (watch.ElapsedMilliseconds < MSPerFrame)
                { }
                watch.Restart();
                UpdateWorld();
            }
            
        }

        private void UpdateWorld()
        {
            lock (world)
            {
                if (++powerupCounter > 300)
                {
                    world.Powerups.Add(powerupID, new Powerup(powerupID, ))
                }

                StringBuilder sb = new StringBuilder();
                string jsonString = "";

                foreach(Projectile p in world.Projectiles.Values)
                {
                    Vector2D v = p.Direction;
                    v *= ProjSpeed;
                    //TODO: Handle Collisions
                    p.Location += v;
                    jsonString = JsonConvert.SerializeObject(p);
                    sb.Append(jsonString);
                    sb.Append('\n');
                }

                foreach (Beam b in world.Beams.Values)
                {
                    foreach(Tank t in world.Tanks.Values)
                    {
                        if (Intersects(b.Origin, b.Direction, t.Location, TankSize / 2))
                        {
                            t.Died = true;
                            t.HP = 0;
                        }
                    }

                    jsonString = JsonConvert.SerializeObject(b);
                    sb.Append(jsonString);
                    sb.Append('\n');
                }

                world.Beams.Clear();

                foreach(Powerup pow in world.Powerups.Values)
                {
                    //TODO: Check if tank picked up a powerup,
                    jsonString = JsonConvert.SerializeObject(pow);
                    sb.Append(jsonString);
                    sb.Append('\n');
                }

                foreach (Tank t in world.Tanks.Values)
                {
                    //TODO: Handle Collisions
                    t.Location += tankVelocities[t.ID];
                    jsonString = JsonConvert.SerializeObject(t);
                    sb.Append(jsonString);
                    sb.Append('\n');
                }

                foreach (Socket socket in sockets.Values)
                {

                    Networking.Send(socket, sb.ToString());
                }

            }
        }

        private void SetupMessageReceive(SocketState state)
        {
            state.OnNetworkAction = CheckName;

            Networking.GetData(state);
        }

        private void CheckName(SocketState state)
        {
            string s = state.GetData();
            if (s.EndsWith('\n'))
            {
                state.RemoveData(0, s.Length);

                //send worldsize and id and walls
                // TODO: change newTankLoc

                Console.WriteLine("Player " + tankID + " " + s + " joined.");

                Tank t = new Tank(tankID, newTankLoc, newTankLoc, s.Trim('\n'), newTankLoc);
                lock (world)
                {
                    tankIDs.Add(state.ID, tankID);
                    tankVelocities.Add(tankID, new Vector2D(0, 0));
                    world.Tanks.Add(tankID, t);
                    sockets.Add(state.ID, state.TheSocket);
                }

                Networking.Send(state.TheSocket, tankID++ + "\n" + UniverseSize + "\n");
                state.OnNetworkAction = SendWalls;
                state.OnNetworkAction(state);
            }

        }

        private void SendWalls(SocketState state)
        {
            string jsonString = null;
            foreach (Wall w in world.Walls.Values)
            {
                jsonString = JsonConvert.SerializeObject(w) + '\n';
                Networking.Send(state.TheSocket, jsonString);
            }
            state.OnNetworkAction = ReceiveControlCommands;
            Networking.GetData(state);

        }

        private void ReceiveControlCommands(SocketState state)
        {
            if (state.ErrorOccurred)
            {
                Console.WriteLine("Cient " + tankIDs[state.ID] + " disconnected.");
                return;
            }

            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            lock (world)
            {
                foreach (string s in parts)
                {
                    // This will be the final "part" in the data buffer
                    if (!s.EndsWith("\n"))
                        continue;

                    // Ignore empty strings added by the regex splitter
                    if (s.Length == 0)
                    {
                        state.RemoveData(0, s.Length);
                        continue;
                    }
                    //TODO handle exception when client closes
                    JObject obj = JObject.Parse(s);

                    JToken token = obj["moving"];
                    if (token != null)
                    {
                        ControlCmd cmd = JsonConvert.DeserializeObject<ControlCmd>(s);

                        HandleTdir(cmd.Tdir, state.ID);
                        HandleMoving(cmd.Moving, state.ID);
                        HandleFire(cmd.Fire, state.ID);

                        state.RemoveData(0, s.Length);
                        continue;
                    }
                }
            }

            Networking.GetData(state);
        }

        private void HandleTdir(Vector2D tdir, long stateID)
        {
            if (tankIDs.TryGetValue(stateID, out int tankID))
            {
                world.Tanks[tankID].Aiming = tdir;
            }
        }

        private void HandleFire(string fire, long stateID)
        {
            if (tankIDs.TryGetValue(stateID, out int tankID))
            {
                if (fire == "main")
                {
                    world.Projectiles.Add(projectileID, new Projectile(projectileID, world.Tanks[tankID].Location, world.Tanks[tankID].Aiming, false, tankID));
                    projectileID++;
                }
                else if (fire == "alt" )//TODO: Check if tank can fire beam
                {
                    world.Beams.Add(beamID, new Beam(beamID, world.Tanks[tankID].Location, world.Tanks[tankID].Aiming, tankID));
                    beamID++;
                }
            }

        }

        private void HandleMoving(string moving, long stateID)
        {
            if (tankIDs.TryGetValue(stateID, out int tankID))
            {
                switch (moving)
                {
                    case "up":
                        world.Tanks[tankID].Orientation = new Vector2D(0, -1);
                        tankVelocities[tankID] = new Vector2D(0, -1) * TankSpeed;
                        break;
                    case "left":
                        world.Tanks[tankID].Orientation = new Vector2D(-1, 0);
                        tankVelocities[tankID] = new Vector2D(-1, 0) * TankSpeed;
                        break;
                    case "down":
                        world.Tanks[tankID].Orientation = new Vector2D(0, 1);
                        tankVelocities[tankID] = new Vector2D(0, 1) * TankSpeed;
                        break;
                    case "right":
                        world.Tanks[tankID].Orientation = new Vector2D(1, 0);
                        tankVelocities[tankID] = new Vector2D(1, 0) * TankSpeed;
                        break;
                    default:
                        tankVelocities[tankID] = new Vector2D(0, 0);
                        break;
                }

            }
        }

        private void ReadSettingsXml()
        {
            using (XmlReader reader = XmlReader.Create(@"..\..\..\..\Resources\settings.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "UniverseSize":
                                ParseSettingsXmlVal(reader, ref UniverseSize);
                                break;
                            case "MSPerFrame":
                                ParseSettingsXmlVal(reader, ref MSPerFrame);
                                break;
                            case "FramesPerShot":
                                ParseSettingsXmlVal(reader, ref FramesPerShot);
                                break;
                            case "RespawnRate":
                                ParseSettingsXmlVal(reader, ref RespawnRate);
                                break;
                            case "Wall":
                                ParseWallXml(reader);
                                break;
                        }
                    }

                }
            }
        }

        private void ParseWallXml(XmlReader reader)
        {
            // at this point, the reader has encountered a <Wall> tag
            string x1 = null;
            string y1 = null;
            string x2 = null;
            string y2 = null;

            Vector2D p1 = null;
            Vector2D p2 = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "p1":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name)
                                        {
                                            case "x":
                                                reader.Read();
                                                if (reader.NodeType == XmlNodeType.Text)
                                                    x1 = reader.Value;
                                                break;
                                            case "y":
                                                reader.Read();
                                                if (reader.NodeType == XmlNodeType.Text)
                                                    y1 = reader.Value;
                                                break;
                                        }
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        if (reader.Name == "p1")
                                            break;
                                    }
                                }
                                break;
                            case "p2":
                                while (reader.Read())
                                {
                                    if (reader.NodeType == XmlNodeType.Element)
                                    {
                                        switch (reader.Name)
                                        {
                                            case "x":
                                                reader.Read();
                                                if (reader.NodeType == XmlNodeType.Text)
                                                    x2 = reader.Value;
                                                break;
                                            case "y":
                                                reader.Read();
                                                if (reader.NodeType == XmlNodeType.Text)
                                                    y2 = reader.Value;
                                                break;
                                        }
                                    }
                                    else if (reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        if (reader.Name == "p2")
                                            break;
                                    }
                                }
                                break;

                        }

                        if (double.TryParse(x1, out double x1Dub) && double.TryParse(y1, out double y1Dub))
                        {
                            p1 = new Vector2D(x1Dub, y1Dub);
                        }

                        if (double.TryParse(x2, out double x2Dub) && double.TryParse(y2, out double y2Dub))
                        {
                            p2 = new Vector2D(x2Dub, y2Dub);
                        }

                        break;
                    case XmlNodeType.EndElement:
                        if (reader.Name == "Wall")
                        {
                            if (p1 is null || p2 is null)
                            {
                                throw new IOException("Could not parse Wall");
                            }
                            else
                            {
                                world.Walls.Add(wallCount, new Wall(wallCount, p1, p2));
                                wallCount++;
                            }
                            return;
                        }
                        break;
                }


            }
        }

        private void ParseSettingsXmlVal(XmlReader reader, ref int settingsVal)
        {
            reader.Read();
            if (reader.NodeType == XmlNodeType.Text)
            {
                string toBeRead = reader.Value;
                if (!int.TryParse(toBeRead, out settingsVal))
                {
                    throw new IOException("Could not parse" + reader.Name);
                }
            }
        }


        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        private static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        private Vector2D RandomSpawnLocation()
        {
            Random rand = new Random();

            double x = rand.Next(-UniverseSize / 2, UniverseSize / 2);
            double y = rand.Next(-UniverseSize / 2, UniverseSize / 2);

            // TODO: check for collisions

            return new Vector2D(x, y);
        }

    }
}