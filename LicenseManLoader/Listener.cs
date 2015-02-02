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
                    Console.WriteLine(inc.ReadString());
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
