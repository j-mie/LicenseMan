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

        public static string GetHWID()
        {
            ManagementClass cpuManager = new ManagementClass("win32_processor");
            ManagementObjectCollection cpuCollection = cpuManager.GetInstances();

            foreach (ManagementObject cpu in cpuCollection)
            {
                var UniqueId = "";
                var UniqueIdObj = cpu.Properties["UniqueId"].Value;
                if(UniqueIdObj != null)
                {
                    UniqueId = UniqueIdObj.ToString();
                }

                var ProcessorId = "";
                var ProcessorIdObj = cpu.Properties["ProcessorId"].Value.ToString();

                if(ProcessorIdObj != null)
                {
                    ProcessorId = ProcessorIdObj.ToString();
                }

                var Name = "";
                var NameObj = cpu.Properties["Name"].Value.ToString();
                if(NameObj != null)
                {
                    Name = NameObj.ToString();

                    int index = Name.IndexOf("Intel(R) Core(TM)");
                    Name = (index < 0)
                        ? Name
                        : Name.Remove(index, "Intel(R) Core(TM)".Length);
                }

                var Manufacturer = "";
                var ManufacturerObj = cpu.Properties["Manufacturer"].Value.ToString();
                if(ManufacturerObj != null)
                {
                    Manufacturer = ManufacturerObj.ToString();
                }

                var MaxClockSpeed = "";
                var MaxClockSpeedObj = cpu.Properties["MaxClockSpeed"].Value.ToString();
                if(MaxClockSpeedObj != null)
                {
                    MaxClockSpeed = MaxClockSpeedObj.ToString();
                }

                var HWID = string.Format("{0}{1}{2}{3}{4}", UniqueId, ProcessorId, Manufacturer, MaxClockSpeed, Name);
                return HWID.Substring(0, (HWID.Length > 64) ? 64 : HWID.Length); // only want 64 chars - man this sucks
            }
            
            throw new Exception("No CPU?!?");
        }

        public static int ProtocolVersion = 3;
    }
}
