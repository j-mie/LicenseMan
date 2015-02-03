
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LicenseManLoader
{
    class ChunkManager
    {
        Dictionary<int, byte[]> chunks;

        string NamespaceClass;
        string Method;
        bool ExitOnFinish;

        public ChunkManager(string NamespaceClass, string Method, bool ExitOnFinish)
        {
            this.NamespaceClass = NamespaceClass;
            this.Method = Method;
            this.ExitOnFinish = ExitOnFinish;
            chunks = new Dictionary<int, byte[]>();
        }

        public void Add(int index, byte[] data)
        {
            chunks.Add(index, data);
        }

        private byte[] Decompress(MemoryStream MemoryStream)
        {
            using (GZipStream stream = new GZipStream(MemoryStream, CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        public void Run()
        {
            var ordered = chunks.OrderBy(x => x.Key); // Order the chunks, probably already done but just in case!

            using(MemoryStream ms = new MemoryStream())
            {
                foreach (var chunk in ordered)
                {
                    ms.Write(chunk.Value, 0, chunk.Value.Length); // Write each chunk to the memory stream
                }

                ms.Position = 0; // Reset the position of the stream

                var AsmBytes = Decompress(ms); // Decompress the stream which was GZip compressed from the server

                var Asm = Assembly.Load(AsmBytes);
                Console.WriteLine(Asm.GetName());

                Type Type = Asm.GetType(NamespaceClass);
                var MethodInfo = Type.GetMethod(Method);

                if (MethodInfo == null) // the method doesn't exist
                {
                    throw new Exception("Invalid binary sent over the wire!");
                }

                var Instance = Activator.CreateInstance(Type);

                var result = MethodInfo.Invoke(Instance, new object[0]);

                if(ExitOnFinish)
                    Environment.Exit(0);
            }
        }
    } 
}
