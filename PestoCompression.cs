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

        public static ulong[] Compress(double[] input, int coefThreshhold)
        {
            // Content Compression
            var result = (double[])input.Clone();

            //var deltaCode = DeltaCompression.Encode(result);
            //PrintArray(deltaCode);

            // Semantic Cramping
            var cramped = TokenCompression.CrampAll(result.Select(x => (sbyte)x).ToArray());

            //PrintArray(cramped);

            return cramped;
        }
        public static ulong[] Compress(byte[] input)
        {
            // Content Compression

            //var deltaCode = DeltaCompression.Encode(result);
            //PrintArray(deltaCode);

            // Semantic Cramping
            var cramped = TokenCompression.CrampAll(input);

            //PrintArray(cramped);

            return cramped;
        }
        public static ulong[] Compress(short[] input)
        {
            var cramped = TokenCompression.CrampAll(input);

            return cramped;
        }
        public static ulong[] Compress(ushort[] input)
        {
            var cramped = TokenCompression.CrampAll(input);

            return cramped;
        }

        public static short[] Decompress_SHORT(ulong[] compressed)
        {
            var sbytes = new short[compressed.Length * 4];

            for (int i = 0; i < sbytes.Length; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    sbytes[i + j] = TokenCompression.ExtractShort(compressed[i / 4], j);
                }
            }

            return sbytes;
        }


        public static byte[] Decompress(ulong[] compressed)
        {
            var sbytes = new byte[compressed.Length * 8];

            for (int i = 0; i < sbytes.Length; i += 8)
            {
                for (int j = 0; j < 8; j++)
                {
                    sbytes[i+j] = TokenCompression.ExtractByte(compressed[i / 8], j);
                }
            }

            return sbytes;
        }
    }
}
