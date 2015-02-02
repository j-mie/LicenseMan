
using System;
using System.Collections.Generic;
using System.IO;
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

        public ChunkManager(string NamespaceClass, string Method)
        {
            this.NamespaceClass = NamespaceClass;
            this.Method = Method;

            chunks = new Dictionary<int, byte[]>();
        }

        public void Add(int index, byte[] data)
        {
            chunks.Add(index, data);
        }

        public void Run()
        {
            //int size = chunks.Values.Select(x => x.Length).Sum();
            //byte[] data = new byte[size];
            using(MemoryStream ms = new MemoryStream())
            {
                var ordered = chunks.OrderBy(x => x.Key);

                foreach (var chunk in ordered) //TODO: Use a memory stream
                {
                    ms.Write(chunk.Value, 0, chunk.Value.Length);
                }

                var asm = Assembly.Load(ms.ToArray());
                Console.WriteLine(asm.GetName());

                Type t  = asm.GetType("ExampleApp.Program");
                var methodInfo = t.GetMethod("Main");
                if (methodInfo == null) // the method doesn't exist
                {
                    // throw some exception
                }

                var o = Activator.CreateInstance(t);

                var result = methodInfo.Invoke(o, new object[0]);
            }
        }
    } 
}
