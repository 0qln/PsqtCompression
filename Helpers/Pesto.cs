using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression.Helpers
{
    internal static class Pesto
    {
        public static short[] GenerateShortTables()
        {
            var result = new short[2 * 6 * 64];

            // iterate mg eg
            for (int mg = 0; mg < 2; mg++)
            {
                // iterate pieces
                for (int piece = 0; piece < 6; piece++)
                {
                    // iterate squares
                    for (int  square = 0; square < 64; square++)
                    {
                        int idx = mg * 6 * 64 + piece * 64 + square;
                        var pieceValue = Data.Pesto.mg_eg_value[piece + mg*6];
                        result[idx] = (short)(Data.Pesto.ShortPositionTables[idx] + pieceValue);
                    }
                }
            }

            return result;
        }
    }
}
