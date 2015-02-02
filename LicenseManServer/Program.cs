using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LicenseManServer LicenseServer = new LicenseManServer();

            LicenseServer.Start();
        }
    }
}
