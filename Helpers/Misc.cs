using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression.Helpers
{
    internal static class Misc
    {
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


        public static void For(int startValue_inclusive, int endIndex_exclusive, Action<int> predicate)
        {
            for (int i = startValue_inclusive; i < endIndex_exclusive; i++) predicate.Invoke(i);
        }


        public static short[] AbsuluteOf(short[] input) => input.Select(e => (short)(e + Math.Abs(input.Min()))).ToArray();



        public static string ToBin(this short num)
        {
            return Convert.ToString(num, 10).PadLeft(4, '0').ToUpper();
        }
    }
}
