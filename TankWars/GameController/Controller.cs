using System;
using NetworkUtil;

namespace GameController
{
    public class Controller
    {
        public delegate void ErrorOccuredHandler(string ErrorMessage); 
        public void Connect(string hostName, string playerName)
        {
            Networking.ConnectToServer(FirstContact, hostName, 11000);
        }

        public void FirstContact(SocketState socket)
        {
            if(socket.ErrorOccurred == true)
            {
                ErrorOccurred(socket.ErrorMessage);
            }

        }
        public event ErrorOccuredHandler ErrorOccurred;
    }
}
