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
        public static TIn[] NormalizePesto<TIn>(TIn[] input)
            where TIn : struct, IMinMaxValue<TIn>
        {
            var result = new TIn[input.Length];

            var min = input.Min();

            if ((dynamic)min >= 0) return input;

            // Iterate pieces
            for (int i = 0; i < input.Length; i++)
            {
                // Initialize result 
                result[i] = (TIn)(input[i] - (dynamic)min);
            }

            return result;
        }

        public static T Project<T>(T x, dynamic min, dynamic max, dynamic newMin, dynamic newMax)
            where T : struct, IMinMaxValue<T>
        {
            double xDo = (double)(dynamic)x;
            var fDy = (((double)newMax - (double)newMin) * (xDo - min) / (max - min) + (double)newMin);
            return (T)fDy;
        }

        // Squish each value down from max up until min
        public static TIn[] TransformPesto<TIn>(TIn[] input, dynamic newMin, dynamic newMax)
            where TIn : struct, IMinMaxValue<TIn>
        {
            double max = (dynamic)input.Max();
            double min = (dynamic)input.Min();

            var result = new TIn[input.Length];

            if (newMin < TIn.MinValue) throw new ArgumentException();
            if (newMax > TIn.MaxValue) throw new ArgumentException();

            // Iterate pieces
            for (int i = 0; i < input.Length; i++)
            {
                // Initialize result 
                result[i] = Project(input[i], min, max, newMin, newMax);
            }

            return result;
        }
        public static float Lerp(float v0, float v1, float t)
        {
            return (1 - t) * v0 + t * v1;
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
            return TokenCompression.Extract<byte>(Data.MinimalChess.CompressedUsTablesNorm[piece * 8 + square / 8 + mg * 8], square % 8)
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
