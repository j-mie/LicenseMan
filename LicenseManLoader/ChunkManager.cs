
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

                Type t  = asm.GetType(NamespaceClass);
                var methodInfo = t.GetMethod(Method);
                if (methodInfo == null) // the method doesn't exist
                {
                    throw new Exception("Invalid binary sent over the wire!");
                }

                var o = Activator.CreateInstance(t);

                var result = methodInfo.Invoke(o, new object[0]);

                if(ExitOnFinish)
                    Environment.Exit(0);
            }
        }
    } 
}
