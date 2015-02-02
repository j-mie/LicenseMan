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
                Bytes = File.ReadAllBytes(AsmName);
            }
            else
            {
                throw new FileNotFoundException("Could not find Asm", AsmName);
            }
        }

        public List<byte[]> Split()
        {
            List<byte[]> List = new List<byte[]>();

            int Bits = Bytes.Length;
            int Size = 117 + 5; // ((Keysize(1024) - 384) / 8) + 37
            int Splits = (int)Math.Ceiling((double)Bits / (double)Size);

            foreach (byte[] copySlice in Bytes.Slices(Splits))
            {
                List.Add(copySlice);
            }

            return List;
        }
    }
}
