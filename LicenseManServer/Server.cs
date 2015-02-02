using Lidgren.Network;
using NLog;
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
        Logger Logger;

        Dictionary<NetConnection, string> Keys = new Dictionary<NetConnection, string>();

        internal Server(int Port, bool UPnP)
        {
            Logger = LogManager.GetLogger("Listener");

            Config = new NetPeerConfiguration("LicenseMan")
            {
                MaximumConnections = 1000,
                ConnectionTimeout = 30,
                Port = Port,
                EnableUPnP = UPnP
            };

            NetServer = new NetServer(this.Config);
        }

        internal void Listen()
        {
            NetServer.Start();

            NetIncomingMessage Msg;

            while (true)
            {
                if((Msg = NetServer.ReadMessage()) == null) continue;

                HandleMsg(Msg);
            }
        }

        internal void HandleMsg(NetIncomingMessage inc)
        {
            switch (inc.MessageType)
            {

                case NetIncomingMessageType.Data:
                    byte type = inc.ReadByte();

                    if(type == (byte)10)
                    {
                        Logger.Debug("Got public key from: {0}", inc.SenderConnection.RemoteEndPoint.Address);
                        Keys.Add(inc.SenderConnection, inc.ReadString());
                    }
                    break;

                case NetIncomingMessageType.StatusChanged:
                    Logger.Debug("{0} status changed to: {1}", inc.SenderConnection.RemoteEndPoint.Address, inc.SenderConnection.Status);
                    break;

                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    Logger.Debug(inc.ReadString());
                    break;
            }
        }
    }
}
