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

        internal void DisconnectWMsg(string Message, NetConnection ClientConn)
        {
            var c = Clients[ClientConn];

            Logger.Info(@"Disconnecting {0}:[{2}] with reason {1}", ClientConn.RemoteEndPoint.Address, Message, c.Username);

            NetOutgoingMessage msg = NetServer.CreateMessage();
            msg.Write((byte)5);
            msg.Write(Message);


            NetServer.SendMessage(msg, ClientConn, NetDeliveryMethod.ReliableOrdered);

            ClientConn.Disconnect(Message);
        }

        internal void SendChunk(int index, int max, byte[] data, NetConnection ClientConn)
        {
            var c = Clients[ClientConn];

            Logger.Debug(@"Sending Chunk {0} out of {1} to {2}:[{3}]", index, max, ClientConn.RemoteEndPoint.Address, c.Username);

            data = Crypto.Encrypt(c.PublicKey, data);

            NetOutgoingMessage msg = NetServer.CreateMessage();
            msg.Write((byte)20);
            msg.Write(data.Length);
            msg.Write(index);
            msg.Write(max);
            msg.Write(data);

            NetServer.SendMessage(msg, ClientConn, NetDeliveryMethod.ReliableOrdered);
        }

        internal void HandleMsg(NetIncomingMessage inc)
        {
            switch (inc.MessageType)
            {

                case NetIncomingMessageType.Data:
                    byte type = inc.ReadByte();

                    if(type == (byte)10)
                    {
                        HandlePublicKey(inc);
                    } else if (type == (byte)11)
                    {
                        HandleLogin(inc);
                    }
                    break;

                case NetIncomingMessageType.StatusChanged:
                    Logger.Debug("{0} status changed to: {1}", inc.SenderConnection.RemoteEndPoint.Address, inc.SenderConnection.Status);
                    break;

                case NetIncomingMessageType.DebugMessage:
                    Logger.Debug(inc.ReadString());
                    break;
                case NetIncomingMessageType.ErrorMessage:
                    Logger.Error(inc.ReadString());
                    break;
                case NetIncomingMessageType.WarningMessage:
                    Logger.Warn(inc.ReadString());
                    break;
                //case NetIncomingMessageType.VerboseDebugMessage:
                    
            }
        }

        private void HandlePublicKey(NetIncomingMessage inc)
        {
            Logger.Debug("Got public key from: {0}", inc.SenderConnection.RemoteEndPoint.Address);

            Client c = new Client();
            c.PublicKey = inc.ReadString();

            Clients.Add(inc.SenderConnection, c);
        }

        private void HandleLogin(NetIncomingMessage inc)
        {
            var Client = Clients[inc.SenderConnection];

            inc.ReadInt32(); // Padding?!?
            string UsernameBase64 = inc.ReadString();
            inc.ReadInt32(); // Padding?!?
            string PasswordBase64 = inc.ReadString();

            byte[] UsernameBytes = Convert.FromBase64String(UsernameBase64);
            byte[] PasswordBytes = Convert.FromBase64String(PasswordBase64);

            var Username = Crypto.Decrypt(this.Config.PrivateKey, UsernameBytes);
            var Password = Crypto.Decrypt(this.Config.PrivateKey, PasswordBytes);

            Logger.Info("Got Username and Password from: {0} - {1}", inc.SenderConnection.RemoteEndPoint.Address, Username);

            var c = Client.Load(Username);
            c.PublicKey = Client.PublicKey;

            Clients[inc.SenderConnection] = c;

            if (c.NewUser == true)
            {
                c.Username = Username;
                c.Password = Password;
                c.Save();

                DisconnectWMsg("Account created. Please contact owner.", inc.SenderConnection);
            }
            else
            {
                if (Password != c.Password)
                {
                    DisconnectWMsg("Invalid Username or Password", inc.SenderConnection);
                }
                else if (!c.OwnsCopy)
                {
                    DisconnectWMsg("You do not own a copy of this software!", inc.SenderConnection);
                }
                else
                {
                    c.Verified = true;

                    Logger.Info("Building new Klunk object for {0}:[{1}]", inc.SenderConnection.RemoteEndPoint.Address, Username);

                    Klunk file = new Klunk(Config.FileName);
                    var data = file.Split();

                    Logger.Info("Klunk size = {0}", data.Count());

                    int i = 0;

                    foreach(var chunk in data)
                    {
                        i++;
                        SendChunk(i, data.Count(), chunk, inc.SenderConnection);
                    }
                }
            }
        }
    }
}
