using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManServer
{
    class Server
    {
        NetServer NetServer;
        NetPeerConfiguration Config;
 
        public Server(int Port)
        {
            Config = new NetPeerConfiguration("LicenseMan")
            {
                MaximumConnections = 1000,
                ConnectionTimeout = 30,
                Port = Port,
                EnableUPnP = true
            };

            NetServer = new NetServer(this.Config);
        }

        public void Listen()
        {
            NetIncomingMessage Msg;

            while (true)
            {
                if((Msg = NetServer.ReadMessage()) == null) continue;

                HandleMsg(Msg);
            }
        }

        private void HandleMsg(NetIncomingMessage inc)
        {
            switch (inc.MessageType)
            {
                case NetIncomingMessageType.Data:
                    
                    break;

                case NetIncomingMessageType.StatusChanged:
                    Console.WriteLine(inc.SenderConnection + " status changed. " + inc.SenderConnection.Status);
                    break;

                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    Console.WriteLine(inc.ReadString());
                    break;
            }
        }
    }
}
