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

        internal Listener(NetClient client)
        {
            this.Client = client;
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

        internal void HandleMsg(NetIncomingMessage inc)
        {
            switch (inc.MessageType)
            {
                case NetIncomingMessageType.Data:
                    var header = inc.ReadByte();

                    if (header == (byte)5)
                    {
                        string DisconnectReason = inc.ReadString();
                        MessageBox.Show(DisconnectReason);
                        Environment.Exit(-1);
                    }
                    else if (header == (byte)11)
                    {
                        string publickey = inc.ReadString();
                    }
                    else if (header == (byte)20)
                    {
                        var length = inc.ReadInt32();

                        var index = inc.ReadInt32();
                        var max = inc.ReadInt32();

                        Console.WriteLine("Got {0} out of {1}", index, max);

                        var bytes = new byte[length];
                        inc.ReadBytes(bytes, 0, length);
                        Console.WriteLine("Got a binary of {0} bytes", bytes.Length);
                    }

                    break;
            }
        }
    }
}
