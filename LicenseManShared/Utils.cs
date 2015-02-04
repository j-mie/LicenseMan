using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManShared
{
    public class Utils
    {
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static byte[] GZipCompressBytes(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }

        public static string GetCpuId()
        {
            ManagementClass cpuManager = new ManagementClass("win32_processor");
            ManagementObjectCollection cpuCollection = cpuManager.GetInstances();

            foreach (ManagementObject cpu in cpuCollection)
            {
                return cpu.Properties["processorID"].Value.ToString();
            }
            
            throw new Exception("No CPU?!?");
        }

        public static int ProtocolVersion = 2;
    }
}
