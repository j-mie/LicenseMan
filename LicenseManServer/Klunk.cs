using LicenseManShared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManServer
{
    class Klunk
    {
        private byte[] Bytes;

        public Klunk(string AsmName)
        {
            if(File.Exists(AsmName))
            {
                Bytes = Utils.GZipCompressBytes(File.ReadAllBytes(AsmName));
            }
            else
            {
                throw new FileNotFoundException("Could not find Asm", AsmName);
            }
        }

        public List<byte[]> Split()
        {
            List<byte[]> List = new List<byte[]>();

            int Size = 117; // ((Keysize(1024) - 384) / 8) + 37

            foreach (byte[] copySlice in Bytes.Slices(Size))
            {
                List.Add(copySlice);
            }

            return List;
        }
    }
}
