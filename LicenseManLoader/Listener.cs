using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    var bytes = new byte[inc.LengthBytes];
                    inc.ReadBytes(bytes, 0, inc.LengthBytes);
                    Console.WriteLine("Got a binary of {0} bytes", bytes.Length);
                    break;
            }
        }
    }
}
