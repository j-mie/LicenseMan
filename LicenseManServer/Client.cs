using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManServer
{
    [Serializable]
    class Client
    {
        internal bool NewUser = false;
        internal bool Verified = false;

        public string Username = "";
        public string Password = "";

        public bool OwnsCopy = false;

        public string PublicKey;

        public void Save()
        {
            if(!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }

            if (String.IsNullOrWhiteSpace(Username))
            {
                return;
            }

            var file = Path.Combine("data", String.Format("{0}.json", Username));
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        public static Client Load(string Name)
        {
            if(!Directory.Exists("data"))
            {
                Directory.CreateDirectory("data");
            }

            var file = Path.Combine("data", String.Format("{0}.json", Name));

            if(File.Exists(file))
            {
                var obj = JsonConvert.DeserializeObject<Client>(File.ReadAllText(file));
                return obj;
            }
            else
            {
                var c = new Client();
                c.NewUser = true;
                return c;
            }   
        }
    }
}
