using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression
{
    internal static class TokenCompression
    {
        public static int Sizeof<T>() => Marshal.SizeOf(default(T)) * 8;


        public static T Extract<T>(ulong u64, int index)
        {
            return (T)(dynamic)(u64 >> (index * Sizeof<T>()) & (~0ul >> Sizeof<T>()));
        }

        public static ulong Cramp<T>(params T[] data)
        {
            ulong result = 0;
            for (int i = 0; i < data.Length; i++)
            {
                result |= (ulong)(dynamic)data[i] << Sizeof<T>() * i;
            }
            return result;
        }


        public static ulong[] CrampAll<T>(T[] input)
        {
            var dSize = sizeof(ulong) / Sizeof<T>();
            var result = new ulong[input.Length / dSize];
            for (int i = 0; i < result.Length; i++)
            {
                T[] group = new T[dSize];
                for (int j = 0; j < dSize; j++)
                    group[j] = input[i * dSize + j];

                result[i] = Cramp(group);
            }
            return result;
        }

    }
}
