using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using GameModel;
using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TankWars;

namespace GameController
{
    public class Controller
    {
        private string playerName;
        public int id;
        private World world;

        //private Dictionary<string, bool> keyPressed;
        private string prevKeyPress = "none";
        private string currentKeyPress = "none";

        public delegate void ErrorOccuredHandler(string ErrorMessage);
        public delegate void ServerUpdateHandler();
        public delegate void WorldReadyHandler();

        public event ServerUpdateHandler UpdateArrived;
        public event ErrorOccuredHandler ErrorOccurred;
        public event WorldReadyHandler WorldReady;
        private string jsonString;
        private ControlCmd controlCmd = new ControlCmd();

        public Controller()
        {
            world = new World();
            controlCmd.moving = "none";
            controlCmd.fire = "none";
            //keyPressed.Add("up", false);
            //keyPressed.Add("down", false);
            //keyPressed.Add("right", false);
            //keyPressed.Add("left", false);
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

                    if (!s.EndsWith("\n"))
                        continue;
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
                        state.RemoveData(0, s.Length);
                        continue;

                    }

                    token = obj["tank"];
                    if (token != null)
                    {
                        WorldReady();
                        state.OnNetworkAction = ReceiveWorld;
                        // Notify any listeners (the view) that a new game world has arrived from the server
                        if (UpdateArrived != null)
                            UpdateArrived();
                        break;
                    }

                    token = obj["proj"];
                    if (token != null)
                    {
                        WorldReady();
                        state.OnNetworkAction = ReceiveWorld;
                        // Notify any listeners (the view) that a new game world has arrived from the server
                        if (UpdateArrived != null)
                            UpdateArrived();
                        break;
                    }

                    token = obj["beam"];
                    if (token != null)
                    {
                        WorldReady();
                        state.OnNetworkAction = ReceiveWorld;
                        // Notify any listeners (the view) that a new game world has arrived from the server
                        if (UpdateArrived != null)
                            UpdateArrived();
                        break;
                    }

                    token = obj["power"];
                    if (token != null)
                    {
                        WorldReady();
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

        public void HandleMouseMove(Point mousePosition, int viewSize)
        {
            lock (controlCmd)
            {
                controlCmd.tdir = new Vector2D(mousePosition.X - (viewSize / 2.0), mousePosition.Y - (viewSize / 2.0));
                controlCmd.tdir.Normalize();

            }

        }

        public void CancelMouseRequest(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                controlCmd.fire = "none";
            }
            else if (e.Button == MouseButtons.Right)
            {
                controlCmd.fire = "none";
            }
        }

        public void HandleMouseRequest(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                controlCmd.fire = "main";
            }
            else if (e.Button == MouseButtons.Right)
            {
                controlCmd.fire = "alt";
            }
        }

        public void HandleMoveRequest(KeyEventArgs e) // TODO: create a private method to reduce code
        {
            if (e.KeyCode == Keys.W)
            {
                if (currentKeyPress != "up")
                {
                    prevKeyPress = currentKeyPress;
                    currentKeyPress = "up";
                    controlCmd.moving = "up";
                }
            }
            else if (e.KeyCode == Keys.A)
            {
                if (currentKeyPress != "left")
                {
                    prevKeyPress = currentKeyPress;
                    currentKeyPress = "left";
                    controlCmd.moving = "left";
                }
            }
            else if (e.KeyCode == Keys.D)
            {
                if (currentKeyPress != "right")
                {
                    prevKeyPress = currentKeyPress;
                    currentKeyPress = "right";
                    controlCmd.moving = "right";
                }
            }
            else if (e.KeyCode == Keys.S)
            {
                if (currentKeyPress != "down")
                {
                    prevKeyPress = currentKeyPress;
                    currentKeyPress = "down";
                    controlCmd.moving = "down";
                }
            }
        }

        public void CancelMoveRequest(KeyEventArgs e) // TODO: create a private method to reduce code
        {
            if (e.KeyCode == Keys.W)
            {
                if (currentKeyPress == "up")
                {
                    currentKeyPress = prevKeyPress;
                    controlCmd.moving = prevKeyPress;
                    prevKeyPress = "none";
                }
                else if (prevKeyPress == "up")
                {
                    prevKeyPress = "none";
                }
                //keyPressed["up"] = false;
                //controlCmd.moving = "none";
            }
            else if (e.KeyCode == Keys.A)
            {
                if (currentKeyPress == "left")
                {
                    currentKeyPress = prevKeyPress;
                    controlCmd.moving = prevKeyPress;
                    prevKeyPress = "none";
                }
                else if (prevKeyPress == "left")
                {
                    prevKeyPress = "none";
                }
                //keyPressed["left"] = false;
                //controlCmd.moving = "none";
            }
            else if (e.KeyCode == Keys.D)
            {
                if (currentKeyPress == "right")
                {
                    currentKeyPress = prevKeyPress;
                    controlCmd.moving = prevKeyPress;
                    prevKeyPress = "none";
                }
                else if (prevKeyPress == "right")
                {
                    prevKeyPress = "none";
                }
                //keyPressed["right"] = false;
                //controlCmd.moving = "none";
            }
            else if (e.KeyCode == Keys.S)
            {
                if (currentKeyPress == "down")
                {
                    currentKeyPress = prevKeyPress;
                    controlCmd.moving = prevKeyPress;
                    prevKeyPress = "none";
                }
                else if (prevKeyPress == "down")
                {
                    prevKeyPress = "none";
                }
                //keyPressed["down"] = false;
                //controlCmd.moving = "none";
            }
        }

        private void ReceiveWorld(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            lock (world)
            {
                foreach (string s in parts)
                {
                    if (!s.EndsWith("\n"))
                    {
                        continue;
                    }

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
                            if (tank.disconnected)
                            {
                                world.GetTanks().Remove(tank.ID);
                            }
                            else
                            {
                                world.GetTanks()[tank.ID] = tank;
                            }

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


            if (controlCmd.tdir != null)
            {
                jsonString = JsonConvert.SerializeObject(controlCmd) + "\n";
                Networking.Send(state.TheSocket, jsonString);
            }
            Networking.GetData(state);
        }

        private void ReceiveStartup(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])"); //, RegexOptions.IgnorePatternWhitespace

            if (parts.Length < 2 || (!parts[0].EndsWith("\n") && !parts[1].EndsWith("\n")))
            {
                Networking.GetData(state);
                return;
            }
            else
            {
                //TODO: handle case when id and worldsize cannot be parsed
                id = int.Parse(parts[0]);
                world.SetPlayerID(id);
                world.SetWorldSize(int.Parse(parts[1]));
                state.RemoveData(0, parts[0].Length + parts[1].Length);
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

        }
        public World GetWorld()
        {
            return world;
        }



    }

}
