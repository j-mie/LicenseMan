using LicenseManShared;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicenseManLoader
{
    class LicenseManLoader
    {
        IPEndPoint Endpoint;
        NetPeerConfiguration NetPeerConfig;
        NetClient NetClient;
        Listener Listener;

        string PublicKey;
        string PrivateKey;

        string Username;
        string Password;

        internal string ServerPublicKey;

        internal LicenseManLoader(string publicKey, string privateKey, string username, string password)
        {
            NetPeerConfig = new NetPeerConfiguration("LicenseMan")
            {
                UseMessageRecycling = false
            };
            NetClient = new NetClient(NetPeerConfig);
            Listener = new Listener(NetClient, privateKey);

            this.PublicKey = publicKey;
            this.PrivateKey = privateKey;

            this.Username = username;
            this.Password = password;

            GetConfig();
            GetKey();
        }

        private void GetConfig()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "LicenseManLoader.Config.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                try
                {
                    #if DEBUG
                    reader.ReadLine();
                    var IP = IPAddress.Parse("127.0.0.1");
                    #else
                    var IP = IPAddress.Parse(reader.ReadLine());
                    #endif
                    var Port = Convert.ToInt32(reader.ReadLine());

                    Endpoint = new IPEndPoint(IP, Port);
                }
                catch (Exception)
                {
                    #if DEBUG
                    throw;
                    #else
                    MessageBox.Show("Error loading config!");
                    #endif
                }
            }
        }

        private void GetKey()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "LicenseManLoader.ServerPublic.key";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                try
                {
                    ServerPublicKey = reader.ReadLine();
                }
                catch (Exception)
                {
                    #if DEBUG
                    throw;
                    #else
                    MessageBox.Show("Error loading server key!");
                    #endif
                }
            }
        }

        internal void Load()
        {
            NetClient.Start();
            NetClient.Connect(Endpoint);

            Listener.Listen();
        }

        internal void SendPublicKey()
        {
            int i = 5 * 100; // 5 seconds
            while(true)
            {
                if(i == 0)
                {
                    MessageBox.Show("Unable to connect to server");
                    Environment.Exit(-1);
                    //throw new Exception("Unable to connect to server!");
                }

                if(NetClient.ConnectionStatus != NetConnectionStatus.Connected)
                {
                    i--;
                    Thread.Sleep(10);
                }
                else
                {
                    break;
                }
            }

            NetOutgoingMessage msg = NetClient.CreateMessage();
            msg.Write((byte)PacketHeaders.Headers.PublicKey);
            msg.Write(PublicKey);
            NetClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }

        internal void SendUsernameAndPassword()
        {
            NetOutgoingMessage msg = NetClient.CreateMessage();
            var username = Crypto.EncryptToString(ServerPublicKey, Username);
            var password = Crypto.EncryptToString(ServerPublicKey, Password);
            var hwid = Crypto.EncryptToString(ServerPublicKey, Utils.GetCpuId());

            msg.Write((byte)PacketHeaders.Headers.Login);
            msg.Write(Utils.ProtocolVersion);
            msg.Write(username);
            msg.Write(password);
            msg.Write(hwid);

            NetClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
