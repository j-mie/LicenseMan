﻿
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

        public ChunkManager()
        {
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
            }
        }
    } 
}
