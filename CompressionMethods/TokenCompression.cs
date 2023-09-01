using PsqtCompression.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PsqtCompression.CompressionMethods
{
    internal static class TokenCompression
    {
        public static int Sizeof<T>() => Marshal.SizeOf(default(T)) * 8;


        public static TOut Extract<TOut>(ulong u64, int index)
        {
            return (TOut)(dynamic)(u64 >> index * Sizeof<TOut>());
        }
        public static TOut Extract<TOut>(ulong u64, int index, int sizeofT)
        {
            // Token efficient
            return (TOut)(dynamic)((u64 >> index * sizeofT) & ~(~0ul << sizeofT));


            // Index has to be inverted here
            // because the u64 might not be filled with a perfect fit,
            // we have to shift by some value x to the right
            // TODO
            ///return (TOut)(dynamic)((u64 << 64 - index * sizeofT - sizeofT) >>> 64 - index * sizeofT);


            // Probably faster, haven't benchmarked it
            ///return (TOut)(dynamic)((u64 >> (index * sizeofT)) & ((1ul << sizeofT) - 1));


            // Token efficient for a known `TOut`

            // example: `TOut` := byte
            ///return (byte)((u64 >> (index * sizeofT)) & 0xFFul);

            // example: `TOut` := short
            ///return (short)((u64 >> (index * sizeofT)) & 0xFFFFul);
        }

        public static ulong Cramp<T>(params T[] data)
            where T : notnull
        {
            ulong result = 0;

            for (int i = 0; i < data.Length; i++)
            {
                result |= (ulong)(dynamic)data[i] << Sizeof<T>() * i;
            }

            return result;
        }

        public static ulong Cramp<T>(T[] data, int sizeofT)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(data);

            ulong result = 0;
            for (int i = 0; i < data.Length; i++)
            {
                result |= (ulong)(dynamic)data[i] << sizeofT * i;
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="sizeOfT">In bits</param>
        /// <returns></returns>
        public static ulong[] CrampAll<T>(T[] input, int sizeOfT)
            where T : notnull
        {
            var dSize = Sizeof<ulong>() / sizeOfT;
            var result = new ulong[input.Length / dSize];

            Console.WriteLine(dSize);

            for (int i = 0; i < result.Length; i++)
            {
                var group = new T[dSize];
                for (int j = 0; j < dSize; j++)
                {
                    group[j] = input[i * dSize + j];
                }
                result[i] = Cramp(group, sizeOfT);
            }

            return result;
        }
        public static TOut[] ExtractAll<TOut>(ulong[] input, int sizeOfTOut)
            where TOut : notnull
        {
            var dSize = Sizeof<ulong>() / sizeOfTOut;
            var result = new TOut[input.Length * dSize];

            for (int i = 0; i < input.Length; i += 1)
            {
                for (int j = 0; j < dSize; j++)
                {
                    result[i * dSize + j] = Extract<TOut>(input[i], j, sizeOfTOut);
                }
            }

            return result;
        }


        public static ulong[] CrampAll<T>(T[] input)
            where T : notnull
        {
            var dSize = Sizeof<ulong>() / Sizeof<T>();
            var result = new ulong[input.Length / dSize];

            for (int i = 0; i < result.Length; i++)
            {
                T[] group = new T[dSize];
                for (int j = 0; j < dSize; j++)
                {
                    group[j] = input[i * dSize + j];
                }
                result[i] = Cramp(group);
            }

            return result;
        }

        public static TOut[] ExtractAll<TOut>(ulong[] input)
            where TOut : notnull
        {
            var dSize = Sizeof<ulong>() / Sizeof<TOut>();
            var result = new TOut[input.Length * dSize];

            for (int i = 0; i < input.Length; i += 1)
            {
                for (int j = 0; j < dSize; j++)
                {
                    result[i * dSize + j] = Extract<TOut>(input[i], j);
                }
            }

            return result;
        }

        
    }
}
