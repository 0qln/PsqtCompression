using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace PsqtCompression.CompressionMethods
{
    internal static class Psqt
    {

        public static ulong[] Compress<T>(T[] input)
            where T : notnull
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
