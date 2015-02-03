using CredentialManagement;
using LicenseManShared;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LicenseManLoader
{
    class Listener
    {
        NetClient Client;
        string PrivateKey;

        string NamespaceClass;
        string Method;

        internal Listener(NetClient client, string PrivateKey)
        {
            this.Client = client;
            this.PrivateKey = PrivateKey;
        }

        internal void Listen()
        {
            NetIncomingMessage Msg;

            while (true)
            {
                if ((Msg = Client.ReadMessage()) == null) continue;

                HandleMsg(Msg);
            }
        }

        ChunkManager chunk;

        internal void HandleMsg(NetIncomingMessage inc)
        {
            switch (inc.MessageType)
            {
                case NetIncomingMessageType.Data:
                    var header = inc.ReadByte();

                    switch (header)
                    {
                        case (byte)PacketHeaders.Headers.PublicKey:
                            string publickey = inc.ReadString();
                            break;
                        case (byte)PacketHeaders.Headers.Disconnect:
                            string DisconnectReason = inc.ReadString();
                            //if (DisconnectReason == "Invalid Username or Password")
                            //{
                            //    var cm = new Credential { Target = "LicenseMan" };
                            //    cm.Delete();
                            //}
                            MessageBox.Show(DisconnectReason);
                            Environment.Exit(59);
                            break;
                        case (byte)PacketHeaders.Headers.Chunk:
                            HandleChunk(inc); 
                        break;
                        case (byte)PacketHeaders.Headers.AssemblySettings:
                            HandleAssemblySettings(inc);
                        break;
                    }
                    break;
            }
        }

        private void HandleAssemblySettings(NetIncomingMessage inc)
        {
            var NamespaceClass = inc.ReadString();
            var Method = inc.ReadString();
            var ExitOnFinish = inc.ReadBoolean();

            this.NamespaceClass = Crypto.DecryptToString(PrivateKey, NamespaceClass);
            this.Method = Crypto.DecryptToString(PrivateKey, Method);

            if (chunk == null)
            {
                chunk = new ChunkManager(this.NamespaceClass, this.Method, ExitOnFinish);
            }
        }

        private void HandleChunk(NetIncomingMessage inc)
        {
            var length = inc.ReadInt32();
            var index = inc.ReadInt32();
            var max = inc.ReadInt32();

            Console.WriteLine("Got {0} out of {1}", index, max);

            byte[] bytes = new byte[length];
            inc.ReadBytes(bytes, 0, length);
            bytes = Crypto.DecryptToBytes(PrivateKey, bytes);

            Console.WriteLine("Got a binary of {0} bytes", bytes.Length);

            chunk.Add(index, bytes);
            if (index == max)
            {
                chunk.Run();
            }
        }
    }
}
