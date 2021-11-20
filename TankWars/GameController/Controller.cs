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
        //private bool idInitialized = false;
        //private bool worldSizeInitialized = false;
        private World world;

        public delegate void ErrorOccuredHandler(string ErrorMessage);
        public delegate void ServerUpdateHandler();

        public event ServerUpdateHandler UpdateArrived;
        public event ErrorOccuredHandler ErrorOccurred;

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
                ErrorOccurred("Could not connect to the server.");
                return;
            }

            state.OnNetworkAction = ReceiveStartup;
            Networking.Send(state.TheSocket, playerName + "\n");
            Networking.GetData(state);
        }

        private void ReceiveWalls(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
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
                        if (world.GetWalls().ContainsKey(w.id))
                        {
                            world.GetWalls()[w.id] = w;
                        }
                        else
                        {
                            world.GetWalls().Add(w.id, w);
                        }
                        Console.WriteLine("Wall ID: " + w.id + " JSON: \n" + s);
                        state.RemoveData(0, s.Length);
                        continue;

                    }

                    token = obj["tank"];
                    if (token != null)
                    {
                        state.OnNetworkAction = ReceiveWorld;
                        // Notify any listeners (the view) that a new game world has arrived from the server
                        if (UpdateArrived != null)
                            UpdateArrived();
                        break;
                    }

                    token = obj["proj"];
                    if (token != null)
                    {
                        state.OnNetworkAction = ReceiveWorld;
                        // Notify any listeners (the view) that a new game world has arrived from the server
                        if (UpdateArrived != null)
                            UpdateArrived();
                        break;
                    }

                    token = obj["beam"];
                    if (token != null)
                    {
                        state.OnNetworkAction = ReceiveWorld;
                        // Notify any listeners (the view) that a new game world has arrived from the server
                        if (UpdateArrived != null)
                            UpdateArrived();
                        break;
                    }

                    token = obj["power"];
                    if (token != null)
                    {
                        state.OnNetworkAction = ReceiveWorld;
                        // Notify any listeners (the view) that a new game world has arrived from the server
                        if (UpdateArrived != null)
                            UpdateArrived();
                        break;
                    }

                }
            }
            Networking.GetData(state);
        }

        public void CancelMouseRequest()
        {
            throw new NotImplementedException();
        }

        public void HandleMouseRequest()
        {
            throw new NotImplementedException();
        }

        public void HandleMoveRequest()
        {
            throw new NotImplementedException();
        }

        public void CancelMoveRequest()
        {
            throw new NotImplementedException();
        }

        private void ReceiveWorld(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
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
                    JToken token = obj["tank"];
                    if (token != null)
                    {

                        Tank tank = JsonConvert.DeserializeObject<Tank>(s);
                        if (world.GetTanks().ContainsKey(tank.ID))
                        {
                            world.GetTanks()[tank.ID] = tank;
                        }
                        else
                        {
                            world.GetTanks().Add(tank.ID, tank);
                        }
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                    token = obj["proj"];
                    if (token != null)
                    {
                        Projectile proj = JsonConvert.DeserializeObject<Projectile>(s);
                        if (world.GetProjectiles().ContainsKey(proj.id))
                        {
                            world.GetProjectiles()[proj.id] = proj;
                        }
                        else
                        {
                            world.GetProjectiles().Add(proj.id, proj);
                        }
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                    token = obj["beam"];
                    if (token != null)
                    {
                        Beam beam = JsonConvert.DeserializeObject<Beam>(s);
                        if (world.GetBeams().ContainsKey(beam.id))
                        {
                            world.GetBeams()[beam.id] = beam;
                        }
                        else
                        {
                            world.GetBeams().Add(beam.id, beam);
                        }
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                    token = obj["power"];
                    if (token != null)
                    {
                        Powerup powerup = JsonConvert.DeserializeObject<Powerup>(s);
                        if (world.GetPowerups().ContainsKey(powerup.id))
                        {
                            world.GetPowerups()[powerup.id] = powerup;
                        }
                        else
                        {
                            world.GetPowerups().Add(powerup.id, powerup);
                        }
                        state.RemoveData(0, s.Length);
                        continue;
                    }

                }
            }

            // Notify any listeners (the view) that a new game world has arrived from the server
            if (UpdateArrived != null)
                UpdateArrived();

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
                world.SetPlayerID(id);
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
        public World GetWorld()
        {
            return world;
        }



    }

}
