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
        NetPeerConfiguration NetPeerConfig;
        Logger Logger;
        Config Config;

        Dictionary<NetConnection, Client> Clients = new Dictionary<NetConnection, Client>();

        internal Server(Config Config)
        {
            this.Logger = LogManager.GetLogger("Listener");
            this.Config = Config;

            NetPeerConfig = new NetPeerConfiguration("LicenseMan")
            {
                MaximumConnections = 1000,
                ConnectionTimeout = 30,
                Port = Config.ListenPort,
                EnableUPnP = Config.EnableUPnP
            };

            NetServer = new NetServer(this.NetPeerConfig);
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

                        Client c = new Client();
                        c.PublicKey = inc.ReadString();

                        Clients.Add(inc.SenderConnection, c);
                    } else if (type == (byte)11)
                    {
                        int usernameLength = inc.ReadInt32();
                        int passwordLength = inc.ReadInt32();

                        byte[] UsernameBytes = new byte[usernameLength];
                        byte[] PasswordBytes = new byte[passwordLength];

                        inc.ReadBytes(UsernameBytes, 5, usernameLength);
                        inc.ReadBytes(PasswordBytes, 5 + usernameLength, passwordLength);

                        var Username = Crypto.Decrypt(this.Config.PrivateKey, UsernameBytes);
                        var Password = Crypto.Decrypt(this.Config.PrivateKey, PasswordBytes);

                        Logger.Debug("Got Username and Password from: {0} - {1}", inc.SenderConnection.RemoteEndPoint.Address, Username);
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
