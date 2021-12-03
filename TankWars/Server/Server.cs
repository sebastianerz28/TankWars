using System;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using GameModel;
using NetworkUtil;
using Newtonsoft.Json;
using TankWars;

namespace TankWars
{
    public class Server
    {
        private int UniverseSize = -1;
        private int MSPerFrame = -1;
        private int FramesPerShot = -1;
        private int RespawnRate = -1;

        private int wallCount = 0;
        private int TankID = 0;
        private Vector2D newTankLoc = new Vector2D(0,0);

        public World world = new World();

        static void Main(string[] args)
        {
            Server s = new Server();
            s.Run();
            Console.Read();
        }
        public Server()
        {

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
                //send worldsize and id and walls


                Tank t = new Tank(TankID, newTankLoc, newTankLoc, s.Trim('\n'), newTankLoc, 0, false, false, true);
                lock (world)
                {
                    world.Tanks.Add(TankID, t);
                }
                
                Networking.Send(state.TheSocket, TankID++ + "\n" + UniverseSize + "\n");
                state.OnNetworkAction = SendWalls;
                state.OnNetworkAction(state);
            }

        }

        private void SendWalls(SocketState state)
        {
            string jsonString = null;
            foreach(Wall w in world.Walls.Values)
            {
                jsonString = JsonConvert.SerializeObject(w);
                Networking.Send(state.TheSocket, jsonString);
            }
            state.OnNetworkAction = ReceiveControlCommands;
            Networking.GetData(state);

        }

        private void ReceiveControlCommands(SocketState state)
        {



            Networking.GetData(state);
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
                            } else
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
    }


}