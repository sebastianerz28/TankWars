using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GameModel;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameController
{
    public class Controller
    {
        private string playerName;
        public int id;
        public int worldSize;
        private bool idInitialized = false;
        private bool worldSizeInitialized = false;
        private World world;

        public delegate void ErrorOccuredHandler(string ErrorMessage);

        public Controller()
        {
            world = new World();
        }

        public void Connect(string hostName, string playerName)
        {
            this.playerName = playerName;
            Networking.ConnectToServer(FirstContact, hostName, 11000);
        }

        private void FirstContact(SocketState state)
        {
            if (state.ErrorOccurred == true)
            {
                ErrorOccurred(state.ErrorMessage);
                return;
            }

            state.OnNetworkAction = ReceiveStartup;
            Networking.Send(state.TheSocket, playerName + "\n");
            Networking.GetData(state);
        }

        private void ReceiveWalls(SocketState state)
        {
            string totalData = state.GetData();
            string [] parts = Regex.Split(totalData, @"(?<=[\n])");
            lock (world)
            {
                foreach (string s in parts)
                {
                    Console.WriteLine(s);
                    // Ignore empty strings added by the regex splitter
                    if (s.Length == 0)
                    {
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                    JObject obj = JObject.Parse(s);

                    JToken token = obj["wall"];
                    if (token != null)
                    {
                        Wall w = JsonConvert.DeserializeObject<Wall>(s);
                        if(world.walls.ContainsKey(w.id))
                        {
                            world.walls[w.id] = w;
                        } else
                        {
                            world.walls.Add(w.id, w);
                        }
                        Console.WriteLine("Wall ID: " + w.id + " JSON: \n" + s);
                        state.RemoveData(0, s.Length);
                        continue;

                    }

                    token = obj["tank"];
                    if (token != null)
                    {
                        Tank tank = JsonConvert.DeserializeObject<Tank>(s);
                        if (world.tanks.ContainsKey(tank.ID))
                        {
                            world.tanks[tank.ID] = tank;
                        }
                        else
                        {
                            world.tanks.Add(tank.ID, tank);
                        }
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                    token = obj["proj"];
                    if (token != null)
                    {
                        Projectile proj = JsonConvert.DeserializeObject<Projectile>(s);
                        if (world.projectiles.ContainsKey(proj.id))
                        {
                            world.projectiles[proj.id] = proj;
                        }
                        else
                        {
                            world.projectiles.Add(proj.id, proj);
                        }
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                    token = obj["beam"];
                    if(token != null)
                    {
                        Beam beam = JsonConvert.DeserializeObject<Beam>(s);
                        if (world.beams.ContainsKey(beam.id))
                        {
                            world.beams[beam.id] = beam;
                        }
                        else
                        {
                            world.beams.Add(beam.id, beam);
                        }
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                    token = obj["power"];
                    if (token != null)
                    {
                        Powerup powerup = JsonConvert.DeserializeObject<Powerup>(s);
                        if (world.powerups.ContainsKey(powerup.id))
                        {
                            world.powerups[powerup.id] = powerup;
                        }
                        else
                        {
                            world.powerups.Add(powerup.id, powerup);
                        }
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                }
            }
            Networking.GetData(state);
        }

        private void ReceiveStartup(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])"); //, RegexOptions.IgnorePatternWhitespace

            if (parts.Length < 2 || !totalData.EndsWith("\n"))
            {
                Networking.GetData(state);
                return;
            }
            else
            {
                //TODO: handle case when id and worldsize cannot be parsed
                id = int.Parse(parts[0]);
                worldSize = int.Parse(parts[1]);
                state.RemoveData(0, totalData.Length);
                state.OnNetworkAction = ReceiveWalls;
                Networking.GetData(state);
            }

            //foreach (string p in parts)
            //{
            //    // Ignore empty strings added by the regex splitter
            //    if (p.Length == 0)
            //        continue;

            //    if (int.TryParse(p, out int val))
            //    {
            //        if (!idInitialized)
            //        {
            //            id = val;
            //        }
            //        else if (!worldSizeInitialized)
            //        {
            //            worldSize = val;
            //        }
            //    }
            //    else
            //    {
            //        Networking.GetData(state);
            //        return;1
            //    }

            //// Then remove it from the SocketState's growable buffer
            //state.RemoveData(0, p.Length);
            //}

            Console.WriteLine(id + " " + worldSize);
        }

        public event ErrorOccuredHandler ErrorOccurred;
    }
}
