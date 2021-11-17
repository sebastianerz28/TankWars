using System;
using System.Text.RegularExpressions;
using NetworkUtil;

namespace GameController
{
    public class Controller
    {
        private string playerName;
        private int id;
        private int worldSize;
        private bool idInitialized = false;
        private bool worldSizeInitialized = false;

        public delegate void ErrorOccuredHandler(string ErrorMessage);
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

        private void ReceiveWorld(SocketState state)
        {
            //DO something
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
                id = int.Parse(parts[0]);
                worldSize = int.Parse(parts[1]);
                state.RemoveData(0, totalData.Length);
                state.OnNetworkAction = ReceiveWorld;
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

        public event ErrorOccuredHandler ErrorOccurred;
    }
}
