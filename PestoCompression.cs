using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace PsqtCompression
{
    public static class Pesto
    {

        public static ulong[] Compress<T>(T[] input)
        {
            return TokenCompression.CrampAll(input);
        }


        public static T[] Decompress<T>(ulong[] compressed)
        {
            var dSize = sizeof(ulong) / TokenCompression.Sizeof<T>();
            var sbytes = new T[compressed.Length * dSize];

            for (int i = 0; i < sbytes.Length; i += dSize)
                for (int j = 0; j < dSize; j++)
                    sbytes[i + j] = TokenCompression.Extract<T>(compressed[i / dSize], j);

            return sbytes;
        }
    }
}
