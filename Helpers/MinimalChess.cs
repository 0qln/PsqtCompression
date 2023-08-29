using PsqtCompression.CompressionMethods;
using System;
using System.Collections.Generic;
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

            Console.WriteLine(min);

            if ((dynamic)min >= 0) return input;

            // Iterate pieces
            for (int i = 0; i < input.Length; i++)
            {
                // Initialize result 
                result[i] = (TIn)(input[i] - (dynamic)min);
            }

            return result;
        }

        public static int GetPieceEval(int piece, int square, bool mg)
        {
            sbyte[] table = mg ? PsqtCompression.MinimalChess.MidgameTablesNorm : PsqtCompression.MinimalChess.EndgameTablesNorm;
            short[] bonus = mg ? PsqtCompression.MinimalChess.MidgamePiecesNorm : PsqtCompression.MinimalChess.EndgamePiecesNorm;
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
            return PsqtCompression.MinimalChess.TablesNorm[piece * 64 + square + mg * 64] - PsqtCompression.MinimalChess.PiecesNorm[piece + mg];
        }

        /// <param name="mg">
        /// 0: mg / 6: eg
        /// </param>
        public static int GetFromCompressed(int piece, int square, int mg)
        {
            return TokenCompression.Extract<byte>(PsqtCompression.MinimalChess.CompressedUsTablesNorm[piece * 8 + square / 8 + mg * 8], square % 8)
                - PsqtCompression.MinimalChess.NotcompressedUsPiecesNorm[mg + piece];
        }

        /// <param name="mg">
        /// 0: mg / 96: eg
        /// </param>
        /// <returns></returns>
        public static int GetPieceEval_SHORTARRAY(int piece, int square, int mg)
        {
            return (short)(PsqtCompression.MinimalChess.CompressedShortTables[piece * 4 + square / 4 + mg] >> (square % 4 * 16) & 0xFFFFul);
        }
        /// <param name="mg">
        /// 0: mg / 96: eg
        /// </param>
        /// <returns></returns>
        public static int GetPieceEval_USHORTARRAY(int piece, int square, int mg)
        {
            return (short)(PsqtCompression.MinimalChess.CompressedShortTables[piece * 4 + square / 4 + mg] >> (square % 4 * 16) & 0xFFFFul) - 81;
        }
    }
}
