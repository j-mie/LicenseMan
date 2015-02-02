using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LicenseManLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            LicenseManLoader Loader = new LicenseManLoader();

            new Thread(() => {
                Loader.Load();
            }).Start();


            Loader.DownloadAndRun();
        }
    }
}
