using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression
{
    internal static class TokenCompression
    {
        //public static void Compress(short[] data)
        //{
        //    Console.WriteLine("\r\n    static short[] Psqa_EG =\r\n    {");

        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        for (int j = 0; j < data[i].Length; j++)
        //        {
        //            Console.Write(data[i][j]);
        //            Console.Write(",");
        //        }
        //    }

        //    Console.WriteLine("\r\n    };");
        //}


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


        public static short ExtractShort(ulong u64, int index) => (short)(u64 >> (index * 16) & 0xFFFFul);
        public static sbyte ExtractSByte(ulong u64, int index) => (sbyte)(u64 >> (index * 8) & 0xFFul);
        public static byte ExtractByte(ulong u64, int index) => (byte)(u64 >> (index * 8) & 0xFFul);

        public static ulong CrampMore(params short[] data)
        {
            return 0;
        }


        public static ulong Cramp(params short[] data)
        {
            ulong result = 0;
            for (int i = 0;i < data.Length;i++)
            {
                ulong x = (ulong)data[i] << 16 * i;
                result |= x;
            }
            return result;
        }
        public static ulong Cramp(params ushort[] data)
        {
            ulong result = 0;
            for (int i = 0; i < data.Length; i++)
            {
                ulong x = (ulong)data[i] << 16 * i;
                result |= x;
            }
            return result;
        }

        public static ulong Cramp(params byte[] data)
        {
            ulong result = 0;
            for (int i = 0; i < data.Length; i++)
            {
                ulong x = (ulong)data[i] << 8 * i;
                result |= x;
            }
            return result;
        }

        public static ulong Cramp(params sbyte[] data)
        {
            ulong result = 0;
            for (int i = 0; i < data.Length; i++)
            {
                ulong x = (ulong)data[i] << 8 * i;
                result |= x;
            }
            return result;
        }

        public static ulong[] CrampAll(sbyte[] input)
        {
            var result = new ulong[input.Length / 8];
            for (int i = 0; i < result.Length; i++)
            {
                sbyte[] bytesGroup = new sbyte[8];
                for (int j = 0; j < 8; j++)
                    bytesGroup[j] = input[i * 8 + j];

                result[i] = Cramp(bytesGroup);
            }
            return result;
        }

        public static ulong[] CrampAll(byte[] input)
        {
            var result = new ulong[input.Length / 8];
            for (int i = 0; i < result.Length; i++)
            {
                var bytesGroup = new byte[8];
                for (int j = 0; j < 8; j++)
                    bytesGroup[j] = input[i * 8 + j];

                result[i] = Cramp(bytesGroup);
            }
            return result;
        }

        public static ulong[] CrampAll(short[] input)
        {
            var result = new ulong[input.Length / 4];
            for (int i = 0; i < result.Length; i++)
            {
                var shortGroup = new short[4];
                for (int j = 0; j < 4; j++)
                    shortGroup[j] = input[i * 4 + j];

                result[i] = Cramp(shortGroup);
            }
            return result;
        }

        public static ulong[] CrampAll(ushort[] input)
        {
            var result = new ulong[input.Length / 4];
            for (int i = 0; i < result.Length; i++)
            {
                var shortGroup = new ushort[4];
                for (int j = 0; j < 4; j++)
                    shortGroup[j] = input[i * 4 + j];

                result[i] = Cramp(shortGroup);
            }
            return result;
        }
    }
}
