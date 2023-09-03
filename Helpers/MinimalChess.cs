using PsqtCompression.CompressionMethods;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression.Helpers
{
    internal static class MinimalChess
    {
        public static ulong[] NormalizePesto<TIn>(TIn[] input)
            where TIn : struct, IMinMaxValue<TIn>
        {
            var result = new ulong[input.Length];

            var min = input.Min();

            // return immediatly if there are no negative values
            // in the input vector.
            if ((dynamic)min >= 0) 
                return input.Select(x => (ulong)(dynamic)x).ToArray();

            // Iterate pieces
            for (int i = 0; i < input.Length; i++)            
                // Initialize result 
                result[i] = (ulong)(input[i] - (dynamic)min);
            

            return result;
        }

        public static double Project<T>(T x, dynamic min, dynamic max, dynamic newMin, dynamic newMax)
            where T : struct, IMinMaxValue<T>
        {
            double xDo = (double)(dynamic)x;
            var fDy = (((double)newMax - (double)newMin) * (xDo - min) / (max - min) + (double)newMin);
            return fDy;
        }
        public static TOut Project<TIn, TOut>(TIn x, dynamic min, dynamic max, dynamic newMin, dynamic newMax)
            where TIn : struct, IMinMaxValue<TIn>
        {
            double xDo = (double)(dynamic)x;
            var fDy = (((double)newMax - (double)newMin) * (xDo - min) / (max - min) + (double)newMin);
            return (TOut)(dynamic)fDy;
        }

        public static double[] TransformPesto<TIn>(TIn[] input, dynamic newMin, dynamic newMax)
            where TIn : struct, IMinMaxValue<TIn>
        {
            double max = (dynamic)input.Max();
            double min = (dynamic)input.Min();

            var result = new double[input.Length];

            if (newMin < double.MinValue) throw new ArgumentException();
            if (newMax > double.MaxValue) throw new ArgumentException();

            // Iterate pieces
            for (int i = 0; i < input.Length; i++)            
                // Initialize result 
                result[i] = Project(input[i], min, max, newMin, newMax);
            

            return result;
        }
        public static double[] TransformPesto<TIn>(TIn[] input, dynamic newMin, dynamic newMax, out Func<TIn, double> transform)
            where TIn : struct, IMinMaxValue<TIn>
        {
            double max = (dynamic)input.Max();
            double min = (dynamic)input.Min();

            var result = new double[input.Length];

            if (newMin < double.MinValue) throw new ArgumentException();
            if (newMax > double.MaxValue) throw new ArgumentException();

            transform = new Func<TIn, double>(x => Project(x, min, max, newMin, newMax));

            // Iterate pieces
            for (int i = 0; i < input.Length; i++)
                // Initialize result 
                result[i] = transform(input[i]);


            return result;
        }
        public static double[] TransformPesto<TIn>(TIn[] input, in Func<TIn, double> transform)
            where TIn : struct, IMinMaxValue<TIn>
        {
            var result = new double[input.Length];

            // Iterate pieces
            for (int i = 0; i < input.Length; i++)
                // Initialize result 
                result[i] = transform(input[i]);


            return result;
        }
        public static TOut[] TransformPesto<TIn, TOut>(TIn[] input, dynamic newMin, dynamic newMax)
            where TIn : struct, IMinMaxValue<TIn>
            where TOut : struct, IMinMaxValue<TOut>
        {
            double max = (dynamic)input.Max();
            double min = (dynamic)input.Min();

            var result = new TOut[input.Length];

            if (newMin < TOut.MinValue) throw new ArgumentException();
            if (newMax > TOut.MaxValue) throw new ArgumentException();

            // Iterate pieces
            for (int i = 0; i < input.Length; i++)
                // Initialize result 
                result[i] = Project<TIn, TOut>(input[i], min, max, newMin, newMax);

            return result;
        }

        public static decimal[] Symmetry<T>(T[] input)
            where T : struct
        {
            Debug.Assert(input.Length == 64);

            var result = new decimal[64];

            // iterate rows
            for (int i = 0; i < 8; i++)
            {
                for (int j = 4; j < 8; j++)
                {
                    decimal left = (decimal)(dynamic)input[i + 8 + (7-j)];
                    decimal right = (decimal)(dynamic)input[i + 8 + (j)];
                    result[i * 8 + j] = (left - right);
                }
            }

            return result;
        }

        public static decimal[] SymmetryErrors<T>(T[] input)
            where T : struct
        {
            Debug.Assert(input.Length == 64);

            var result = new decimal[32];

            // iterate rows
            for (int i = 0; i < 8; i++)
            {
                for (int j = 4; j < 8; j++)
                {
                    decimal left = (decimal)(dynamic)input[i + 8 + (7 - j)];
                    decimal right = (decimal)(dynamic)input[i + 8 + (j)];
                    result[i * 4 + j - 4] = (left - right);
                }
            }

            return result;
        }
        public static decimal[] SymmetriesErrors<T>(T[] input)
            where T : struct
        {
            Debug.Assert(input.Length % 64 == 0);

            decimal[] result = new decimal[input.Length / 2];

            for (int i = 0; i < input.Length; i += 64)
            {
                T[] array = new T[64];
                Array.ConstrainedCopy(input, i, array, 0, 64);
                Array.Copy(SymmetryErrors(array), 0, result, i / 2, 32);
            }

            return result;
        }

        public static decimal[] Symmetries<T>(T[] input)
            where T : struct
        {
            Debug.Assert(input.Length % 64 == 0);

            decimal[] result = new decimal[input.Length];

            for (int i = 0; i < input.Length; i+= 64)
            {
                T[] array = new T[64];
                Array.ConstrainedCopy(input, i, array, 0, 64);
                Array.Copy(Symmetry(array), 0, result, i, 64);
            }

            return result;
        }

        public static int GetPieceEval(int piece, int square, bool mg)
        {
            sbyte[] table = mg ? Data.MinimalChess.MidgameTablesNorm : Data.MinimalChess.EndgameTablesNorm;
            short[] bonus = mg ? Data.MinimalChess.MidgamePiecesNorm : Data.MinimalChess.EndgamePiecesNorm;
            return table[piece * 64 + square] - bonus[piece];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="square"></param>
        /// <param name="mg">0 if mg, 6 if eg</param>
        /// <returns></returns>
        public static int GetPieceEval(int piece, int square, int mg)
        {
            return Data.MinimalChess.TablesNorm[piece * 64 + square + mg * 64] - Data.MinimalChess.PiecesNorm[piece + mg];
        }

        /// <param name="mg">
        /// 0: mg / 6: eg
        /// </param>
        public static int GetFromCompressed(int piece, int square, int mg)
        {
            return UlongTokenCompression.Extract<byte>(Data.MinimalChess.CompressedUsTablesNorm[piece * 8 + square / 8 + mg * 8], square % 8)
                - Data.MinimalChess.NotcompressedUsPiecesNorm[mg + piece];
        }

        /// <param name="mg">
        /// 0: mg / 96: eg
        /// </param>
        /// <returns></returns>
        public static int GetPieceEval_SHORTARRAY(int piece, int square, int mg)
        {
            return (short)(Data.MinimalChess.CompressedShortTables[piece * 4 + square / 4 + mg] >> (square % 4 * 16) & 0xFFFFul);
        }
        /// <param name="mg">
        /// 0: mg / 96: eg
        /// </param>
        /// <returns></returns>
        public static int GetPieceEval_USHORTARRAY(int piece, int square, int mg)
        {
            return (short)(Data.MinimalChess.CompressedShortTables[piece * 4 + square / 4 + mg] >> (square % 4 * 16) & 0xFFFFul) - 81;
        }
    }
}
