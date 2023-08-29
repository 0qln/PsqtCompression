using PsqtCompression.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Program;
using static PsqtCompression.Helpers.MinimalChess;

namespace PsqtCompression.CompressionMethods
{
    internal static class Psqt
    {
        // Delta compression doesn't work, we would need
        // negatives for some of the deltas.

        
        public static ulong[] Compress<T>(T[] input)
            where T : struct, IMinMaxValue<T>
        {
            var delta = DeltaCompression.Encode(input);

            var squished = TransformPesto(delta, 0, 255);

            var bytes = squished.Select(x => (byte)(dynamic)x).ToArray();

            var cramped = TokenCompression.CrampAll<Byte>(bytes);

            return cramped;
        }


        public static T[] Decompress<T>(ulong[] compressed)
            where T : struct, IMinMaxValue<T>
        {
            var extracted = TokenCompression.ExtractAll<Byte>(compressed);

            var floatingPoints = extracted.Select(x => (double)(dynamic)x).ToArray();

            var deltaDecode = DeltaCompression.Decode<T>(floatingPoints);

            return extracted.Select(x => (T)(dynamic)x).ToArray();
        }
    }
}
