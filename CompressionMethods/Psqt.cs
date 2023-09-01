using PsqtCompression.Helpers;
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
        // Delta compression doesn't work, we would need
        // negatives for some of the deltas.

        
        public static ulong[] Compress<T>(T[] input, uint compressionRate, out int origBitSize)
            where T : struct, IMinMaxValue<T>
        {
            // __Constants__ 
            // Determine the max used number of bits
            int numBits = Misc.NumBitsUsed(input.Max());
            int highestNumberUsed = (int)Math.Pow(2, numBits);
            // We cannot have negative numbers
            var min = 0;
            // save `compressionRate` number of bits per entry,
            // shouldn't be too lossy
            var max = highestNumberUsed >> (int)compressionRate;


            // __Push all values out of the negative space__
            // our cramping method cannot handle negative values,
            var normalized = MinimalChess.NormalizePesto(input);


            // __Project values into target range__
            // this is where the compression happens
            var squished = MinimalChess.TransformPesto(normalized, min, max);


            // __Parse values into target format__
            // the new format has to be big enough to hold max and min
            // it also cannot be of floating point representation
            var nums = squished.Select(x => (ulong)(dynamic)Math.Round(x)).ToArray();


            // __Token reduction__
            // Cramp small numbers into one big number
            numBits = Misc.NumBitsUsed(nums.Max());
            var cramped = TokenCompression.CrampAll(nums, numBits);


            // __Return compressed array and helper values__
            origBitSize = numBits;
            return cramped;
        }

        public static ulong[] Compress<T>(T[] input)
            where T : struct, IMinMaxValue<T>
        {
            var table = input;

            var norm = MinimalChess.NormalizePesto(table);

            var squished = MinimalChess.TransformPesto<ulong, Byte>(norm, 0, 255);

            var cramped = TokenCompression.CrampAll(squished);

            return cramped;
        }


        public static T[] Decompress<T>(ulong[] compressed)
            where T : struct, IMinMaxValue<T>
        {
            var extracted = TokenCompression.ExtractAll<T>(compressed);

            return extracted.Select(x => (T)(dynamic)x).ToArray();
        }

        public static ulong[] Decompress(ulong[] compressed, int origBitSize)
        {
            // __Extraction__
            // extract the number out of the cramped ulongs
            var extracted = TokenCompression.ExtractAll<ulong>(compressed, origBitSize);

            // __Return the decompressed array__
            return extracted;
        }
    }
}
