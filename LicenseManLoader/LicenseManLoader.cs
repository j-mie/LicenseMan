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

        internal LicenseManLoader()
        {
            NetPeerConfig = new NetPeerConfiguration("LicenseMan");
            NetClient = new NetClient(NetPeerConfig);
            Listener = new Listener(NetClient);

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "LicenseManLoader.Config.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                try
                {
                    var IP = IPAddress.Parse(reader.ReadLine());
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

        internal void Load()
        {
            NetClient.Start();
            NetClient.Connect(Endpoint);

            Listener.Listen();
        }

        internal void DownloadAndRun()
        {
            NetOutgoingMessage msg = NetClient.CreateMessage();
            msg.Data = new byte[42];

            NetClient.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
