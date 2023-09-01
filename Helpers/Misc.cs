using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression.Helpers
{
    internal static class Misc
    {
        public static void Noop() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">[0,63]</param>
        /// <param name="white"></param>
        /// <returns>[0,31]</returns>
        public static int MapSquareIndexToPsqa(int index, bool white)
        {
            int file = index & 7;
            //-----------row----------------//  +  //---------file----------//
            return (index / 8 * 4 ^ (white ? 0 : 0x1C)) + (file < 4 ? file : 7 - file);
        }


        public static float Lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
        }


        // input number should be positive
        public static int NumBitsUsed<T>(T input) => 1 + (int)Math.Log2(Math.Floor((double)(dynamic)input));


        public static int CountMaxBits(short value)
        {
            int count = 0;
            while (value > 0)
            {
                value >>= 1;
                count++;
            }
            return count;
        }


        public static long[] Delta<T1, T2>(T1[] input1, T2[] input2)
        {
            Debug.Assert(input1.Length == input2.Length);

            var size = input1.Length;
            var result = new long[size];

            for (int i = 0; i < size; i++)
            {
                result[i] = (dynamic)input1[i] - (dynamic)input2[i];
            }

            return result;
        }


        public static void For(int startValue_inclusive, int endIndex_exclusive, Action<int> predicate)
        {
            for (int i = startValue_inclusive; i < endIndex_exclusive; i++) predicate.Invoke(i);
        }


        public static short[] AbsuluteOf(short[] input) => input.Select(e => (short)(e + Math.Abs(input.Min()))).ToArray();



        public static string ToBin(this short num)
        {
            return Convert.ToString(num, 2).PadLeft(sizeof(short), '0').ToUpper();
        }

        public static string ToBin(this ushort num)
        {
            return Convert.ToString(num, 2).PadLeft(sizeof(ushort), '0').ToUpper();
        }

        public static string ToBin(this long num)
        {
            return Convert.ToString(num, 2).PadLeft(sizeof(long), '0').ToUpper();
        }
    }
}
