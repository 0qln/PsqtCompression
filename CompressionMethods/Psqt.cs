﻿using ImageMagick;
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
            // `compressionRate` := number_of_bits_saved_per_number

            // __Normalize__
            // Push all values out of the negative space
            // Our cramping method cannot handle negative values,
            var normalized = MinimalChess.NormalizePesto(input);
            
            
            // __Constants__ 
            // Determine the max used number of bits
            int numBits = Misc.NumBitsUsed(normalized.Max());
            
            // Determine the max possible number reachable
            // with the number of bits
            int highestNumberUsed = (int)Math.Pow(2, numBits) - 1;

            // We cannot yet have negative numbers, not compatible
            // with the token compression method used
            var min = 0;
            
            // save `compressionRate` number of bits per entry,
            // shouldn't be too lossy
            var max = highestNumberUsed >> (int)compressionRate;


            // __Project values into target range__
            var squished = MinimalChess.TransformPesto(normalized, min, max);


            // __Parse values into target format__
            // The new format has to be big enough to hold max and min
            // it also cannot be of floating point representation
            var nums = squished.Select(x => (ulong)(dynamic)Math.Round(x)).ToArray();


            // __Token reduction__
            // Cramp small numbers into one big number
            numBits = Misc.NumBitsUsed(nums.Max());
            var cramped = UlongTokenCompression.CrampAll(nums, numBits);


            // __Return compressed array and helper values__
            origBitSize = numBits;
            return cramped;
        }
        public static ulong[] Compress<TIn>(TIn[] input, int bitsPerNum, out Func<ulong, double> transform)
            where TIn : struct, IMinMaxValue<TIn>
        {
            // `compressionRate` := number_of_bits_saved_per_number

            // __Normalize__
            // Push all values out of the negative space
            // Our cramping method cannot handle negative values,
            var normalized = MinimalChess.NormalizePesto(input);


            // __Constants__ 
            // Determine the max possible number reachable
            // with the number of bits
            int highestNumber = (int)Math.Pow(2, bitsPerNum) - 1;


            // __Project values into target range__
            var squished = MinimalChess.TransformPesto<ulong>(normalized, 0, highestNumber, out transform);


            // __Parse values into target format__
            // The new format has to be big enough to hold max and min
            // it also cannot be of floating point representation
            var nums = squished.Select(x => (ulong)(dynamic)Math.Round(x)).ToArray();


            // __Token reduction__
            // Cramp small numbers into one big number
            var cramped = UlongTokenCompression.CrampAll(nums, bitsPerNum);


            // __Return compressed array__
            return cramped;
        }
        public static ulong[] Compress<T>(T[] input)
            where T : struct, IMinMaxValue<T>
        {
            var table = input;

            var norm = MinimalChess.NormalizePesto(table);

            var squished = MinimalChess.TransformPesto<ulong, Byte>(norm, 0, 255);

            var cramped = UlongTokenCompression.CrampAll(squished);

            return cramped;
        }


        public static T[] Decompress<T>(ulong[] compressed)
            where T : struct, IMinMaxValue<T>
        {
            var extracted = UlongTokenCompression.ExtractAll<T>(compressed);

            return extracted.Select(x => (T)(dynamic)x).ToArray();
        }

        public static ulong[] Decompress(ulong[] compressed, int origBitSize)
        {
            // __Extraction__
            // extract the number out of the cramped ulongs
            var extracted = UlongTokenCompression.ExtractAll<ulong>(compressed, origBitSize);

            // __Return the decompressed array__
            return extracted;
        }
    }

    public static class Ext
    {
        public static ulong[] Decompress(this ulong[] compressed, int origBitSize)
        {
            // __Extraction__
            // extract the number out of the cramped ulongs
            var extracted = UlongTokenCompression.ExtractAll<ulong>(compressed, origBitSize);

            // __Return the decompressed array__
            return extracted;
        }
    }

    static class PsqtOneLiners
    {
        /// <summary>
        /// Psqt.Compress(pesto, 3, out int origBitSize)
        /// origBitSize = 8
        /// </summary>
        /// <param name="compressed"></param>
        /// <param name="origBitSize"></param>
        /// <returns></returns>
        //public static ulong[] Decompress(this ulong[] compressed)
        //{
        //    ulong Extract(ulong u64, int index)
        //    {
        //        return (u64 >> index * 8) & 0xFF;
        //    }
        //    ulong[] ExtractAll(ulong[] input)
        //    {
        //        var result = new ulong[input.Length * 8];

        //        for (int i = 0; i < input.Length; i++)
        //        {
        //            for (int j = 0; j < 8; j++)
        //            {
        //                result[i * 8 + j] = Extract(input[i], j);
        //            }
        //        }

        //        return result;
        //    }

        //    // __Return the decompressed array__
        //    return ExtractAll(compressed);
        //}
        public static ulong[] Decompress(this ulong[] compressed) => compressed
            .SelectMany(u64 => Enumerable.Range(0, 8)
                .Select(index => (u64 >> index * 8) & 0xFF))
            .ToArray();

    }
}
