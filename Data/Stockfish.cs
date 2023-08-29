﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression.Data
{
    internal static class SFData
    {
        public static short[] PsqaAbsulute_MG =
        {
        26,109,127,128,124,160,174,186,140,184,207,213,166,209,241,250,167,214,245,252,192,223,259,254,134,174,205,238,0,118,145,175,164,197,195,185,190,207,214,204,196,216,197,213,197,209,219,228,193,221,216,223,190,205,202,209,189,191,205,201,167,202,191,185,170,181,187,196,180,188,193,207,176,190,200,204,188,196,197,195,174,186,197,204,179,199,207,213,199,213,217,219,184,182,200,210,204,196,196,205,198,206,209,213,198,207,214,208,205,206,210,209,201,215,213,206,197,211,207,209,196,207,211,209,199,199,202,199,472,528,472,399,479,504,435,380,396,459,370,321,365,391,339,299,355,380,306,271,324,346,282,232,289,321,266,234,260,290,246,200,
    };
        public static short[] PsqaAbsulute_EG =
        {
        4,35,51,79,33,46,82,108,60,73,92,129,65,98,113,128,55,84,109,139,49,56,84,117,31,50,49,112,0,12,44,83,60,79,74,92,74,91,88,101,89,99,99,107,86,96,100,112,88,99,90,111,79,104,103,104,78,86,99,101,68,71,74,83,91,87,90,91,88,91,99,98,106,92,98,94,94,101,91,107,95,108,107,94,106,101,93,110,104,105,120,95,118,100,119,113,31,43,53,74,46,69,78,96,61,82,91,103,77,97,113,124,71,94,109,121,62,82,89,101,50,73,76,92,26,48,57,66,101,145,185,176,153,200,233,235,188,230,269,275,203,256,272,272,196,266,299,299,192,272,284,291,147,221,216,231,111,159,173,178,
    };

        public static short GetPsq_MG(int piece, int square)
        {
            return Psqa_MG[piece * 32 + square];
        }
        public static short[] Psqa_MG =
        {
        -175,-92,-74,-73,-77,-41,-27,-15,-61,-17,6,12,-35,8,40,49,-34,13,44,51,-9,22,58,53,-67,-27,4,37,-201,-83,-56,-26,-37,-4,-6,-16,-11,6,13,3,-5,15,-4,12,-4,8,18,27,-8,20,15,22,-11,4,1,8,-12,-10,4,0,-34,1,-10,-16,-31,-20,-14,-5,-21,-13,-8,6,-25,-11,-1,3,-13,-5,-4,-6,-27,-15,-4,3,-22,-2,6,12,-2,12,16,18,-17,-19,-1,9,3,-5,-5,4,-3,5,8,12,-3,6,13,7,4,5,9,8,0,14,12,5,-4,10,6,8,-5,6,10,8,-2,-2,1,-2,271,327,271,198,278,303,234,179,195,258,169,120,164,190,138,98,154,179,105,70,123,145,81,31,88,120,65,33,59,89,45,-1,
    };

        public static short GetPsq_EG(int piece, int square)
        {
            return Psqa_EG[piece * 32 + square];
        }
        public static short[] Psqa_EG =
        {
        -96,-65,-49,-21,-67,-54,-18,8,-40,-27,-8,29,-35,-2,13,28,-45,-16,9,39,-51,-44,-16,17,-69,-50,-51,12,-100,-88,-56,-17,-40,-21,-26,-8,-26,-9,-12,1,-11,-1,-1,7,-14,-4,0,12,-12,-1,-10,11,-21,4,3,4,-22,-14,-1,1,-32,-29,-26,-17,-9,-13,-10,-9,-12,-9,-1,-2,6,-8,-2,-6,-6,1,-9,7,-5,8,7,-6,6,1,-7,10,4,5,20,-5,18,0,19,13,-69,-57,-47,-26,-54,-31,-22,-4,-39,-18,-9,3,-23,-3,13,24,-29,-6,9,21,-38,-18,-11,1,-50,-27,-24,-8,-74,-52,-43,-34,1,45,85,76,53,100,133,135,88,130,169,175,103,156,172,172,96,166,199,199,92,172,184,191,47,121,116,131,11,59,73,78,
    };

        public static short[][] ExtractedRawPsqa_MG =
        {

        new short[]
        {
            -175,-92,-74,-73,-77,-41,-27,-15,-61,-17,6,12,-35,8,40,49,-34,13,44,51,-9,22,58,53,-67,-27,4,37,-201,-83,-56,-26,
        },

        new short[]
        {
            -37,-4,-6,-16,-11,6,13,3,-5,15,-4,12,-4,8,18,27,-8,20,15,22,-11,4,1,8,-12,-10,4,0,-34,1,-10,-16,
        },

        new short[]
        {
            -31,-20,-14,-5,-21,-13,-8,6,-25,-11,-1,3,-13,-5,-4,-6,-27,-15,-4,3,-22,-2,6,12,-2,12,16,18,-17,-19,-1,9,
        },

        new short[]
        {
            3,-5,-5,4,-3,5,8,12,-3,6,13,7,4,5,9,8,0,14,12,5,-4,10,6,8,-5,6,10,8,-2,-2,1,-2,
        },

        new short[]
        {
            271,327,271,198,278,303,234,179,195,258,169,120,164,190,138,98,154,179,105,70,123,145,81,31,88,120,65,33,59,89,45,-1,
        },

    };
        public static short[][] ExtractedRawPsqa_EG =
        {

        new short[]
        {
            -96,-65,-49,-21,-67,-54,-18,8,-40,-27,-8,29,-35,-2,13,28,-45,-16,9,39,-51,-44,-16,17,-69,-50,-51,12,-100,-88,-56,-17,
        },

        new short[]
        {
            -40,-21,-26,-8,-26,-9,-12,1,-11,-1,-1,7,-14,-4,0,12,-12,-1,-10,11,-21,4,3,4,-22,-14,-1,1,-32,-29,-26,-17,
        },

        new short[]
        {
            -9,-13,-10,-9,-12,-9,-1,-2,6,-8,-2,-6,-6,1,-9,7,-5,8,7,-6,6,1,-7,10,4,5,20,-5,18,0,19,13,
        },

        new short[]
        {
            -69,-57,-47,-26,-54,-31,-22,-4,-39,-18,-9,3,-23,-3,13,24,-29,-6,9,21,-38,-18,-11,1,-50,-27,-24,-8,-74,-52,-43,-34,
        },

        new short[]
        {
            1,45,85,76,53,100,133,135,88,130,169,175,103,156,172,172,96,166,199,199,92,172,184,191,47,121,116,131,11,59,73,78,
        },

    };
        public static short[] Psqa =
        {
        -175,-96,-92,-65,-74,-49,-73,-21,-77,-67,-41,-54,-27,-18,-15,8,-61,-40,-17,-27,6,-8,12,29,-35,-35,8,-2,40,13,49,28,-34,-45,13,-16,44,9,51,39,-9,-51,22,-44,58,-16,53,17,-67,-69,-27,-50,4,-51,37,12,-201,-100,-83,-88,-56,-56,-26,-17,-37,-40,-4,-21,-6,-26,-16,-8,-11,-26,6,-9,13,-12,3,1,-5,-11,15,-1,-4,-1,12,7,-4,-14,8,-4,18,0,27,12,-8,-12,20,-1,15,-10,22,11,-11,-21,4,4,1,3,8,4,-12,-22,-10,-14,4,-1,0,1,-34,-32,1,-29,-10,-26,-16,-17,-31,-9,-20,-13,-14,-10,-5,-9,-21,-12,-13,-9,-8,-1,6,-2,-25,6,-11,-8,-1,-2,3,-6,-13,-6,-5,1,-4,-9,-6,7,-27,-5,-15,8,-4,7,3,-6,-22,6,-2,1,6,-7,12,10,-2,4,12,5,16,20,18,-5,-17,18,-19,0,-1,19,9,13,3,-69,-5,-57,-5,-47,4,-26,-3,-54,5,-31,8,-22,12,-4,-3,-39,6,-18,13,-9,7,3,4,-23,5,-3,9,13,8,24,0,-29,14,-6,12,9,5,21,-4,-38,10,-18,6,-11,8,1,-5,-50,6,-27,10,-24,8,-8,-2,-74,-2,-52,1,-43,-2,-34,271,1,327,45,271,85,198,76,278,53,303,100,234,133,179,135,195,88,258,130,169,169,120,175,164,103,190,156,138,172,98,172,154,96,179,166,105,199,70,199,123,92,145,172,81,184,31,191,88,47,120,121,65,116,33,131,59,11,89,59,45,73,-1,78,
    };

        public static short[][] ExtractedRawPsqa =
        {
        new short[]
        {
            -175,-96,-92,-65,-74,-49,-73,-21,-77,-67,-41,-54,-27,-18,-15,8,-61,-40,-17,-27,6,-8,12,29,-35,-35,8,-2,40,13,49,28,-34,-45,13,-16,44,9,51,39,-9,-51,22,-44,58,-16,53,17,-67,-69,-27,-50,4,-51,37,12,-201,-100,-83,-88,-56,-56,-26,-17,
        },

        new short[]
        {
            -37,-40,-4,-21,-6,-26,-16,-8,-11,-26,6,-9,13,-12,3,1,-5,-11,15,-1,-4,-1,12,7,-4,-14,8,-4,18,0,27,12,-8,-12,20,-1,15,-10,22,11,-11,-21,4,4,1,3,8,4,-12,-22,-10,-14,4,-1,0,1,-34,-32,1,-29,-10,-26,-16,-17,
        },

        new short[]
        {
            -31,-9,-20,-13,-14,-10,-5,-9,-21,-12,-13,-9,-8,-1,6,-2,-25,6,-11,-8,-1,-2,3,-6,-13,-6,-5,1,-4,-9,-6,7,-27,-5,-15,8,-4,7,3,-6,-22,6,-2,1,6,-7,12,10,-2,4,12,5,16,20,18,-5,-17,18,-19,0,-1,19,9,13,
        },

        new short[]
        {
            3,-69,-5,-57,-5,-47,4,-26,-3,-54,5,-31,8,-22,12,-4,-3,-39,6,-18,13,-9,7,3,4,-23,5,-3,9,13,8,24,0,-29,14,-6,12,9,5,21,-4,-38,10,-18,6,-11,8,1,-5,-50,6,-27,10,-24,8,-8,-2,-74,-2,-52,1,-43,-2,-34,
        },

        new short[]
        {
            271,1,327,45,271,85,198,76,278,53,303,100,234,133,179,135,195,88,258,130,169,169,120,175,164,103,190,156,138,172,98,172,154,96,179,166,105,199,70,199,123,92,145,172,81,184,31,191,88,47,120,121,65,116,33,131,59,11,89,59,45,73,-1,78,
        },

    };
        public static (short, short)[][] RawPiecePsqa =
        {
        new (short, short)[]
        { // Knight
            (-175, -96), (-92, -65), (-74, -49), (-73, -21),
            (-77, -67), (-41, -54), (-27, -18), (-15, 8),
            (-61, -40), (-17, -27), (6, -8), (12, 29),
            (-35, -35), (8, -2), (40, 13), (49, 28),
            (-34, -45), (13, -16), (44, 9), (51, 39),
            (-9, -51), (22, -44), (58, -16), (53, 17),
            (-67, -69), (-27, -50), (4, -51), (37, 12),
            (-201, -100), (-83, -88), (-56, -56), (-26, -17)
        },
        new (short, short)[]
        { // Bishop
            (-37, -40), (-4, -21), (-6, -26), (-16, -8),
            (-11, -26), (6, -9), (13, -12), (3, 1),
            (-5, -11), (15, -1), (-4, -1), (12, 7),
            (-4, -14), (8, -4), (18, 0), (27, 12),
            (-8, -12), (20, -1), (15, -10), (22, 11),
            (-11, -21), (4, 4), (1, 3), (8, 4),
            (-12, -22), (-10, -14), (4, -1), (0, 1),
            (-34, -32), (1, -29), (-10, -26), (-16, -17)
        },
        new (short, short)[]
        { // Rook
            (-31, -9), (-20, -13), (-14, -10), (-5, -9),
            (-21, -12), (-13, -9), (-8, -1), (6, -2),
            (-25, 6), (-11, -8), (-1, -2), (3, -6),
            (-13, -6), (-5, 1), (-4, -9), (-6, 7),
            (-27, -5), (-15, 8), (-4, 7), (3, -6),
            (-22, 6), (-2, 1), (6, -7), (12, 10),
            (-2, 4), (12, 5), (16, 20), (18, -5),
            (-17, 18), (-19, 0), (-1, 19), (9, 13)
        },
        new (short, short)[]
        { // Queen
            (3, -69), (-5, -57), (-5, -47), (4, -26),
            (-3, -54), (5, -31), (8, -22), (12, -4),
            (-3, -39), (6, -18), (13, -9), (7, 3),
            (4, -23), (5, -3), (9, 13), (8, 24),
            (0, -29), (14, -6), (12, 9), (5, 21),
            (-4, -38), (10, -18), (6, -11), (8, 1),
            (-5, -50), (6, -27), (10, -24), (8, -8),
            (-2, -74), (-2, -52), (1, -43), (-2, -34)
        },
        new (short, short)[]
        { // King
            (271,  1), (327, 45), (271, 85), (198, 76) ,
            (278, 53), (303,100), (234,133), (179,135) ,
            (195, 88), (258,130), (169,169), (120,175) ,
            (164,103), (190,156), (138,172), ( 98,172) ,
            (154, 96), (179,166), (105,199), ( 70,199) ,
            (123, 92), (145,172), ( 81,184), ( 31,191) ,
            ( 88, 47), (120,121), ( 65,116), ( 33,131) ,
            ( 59, 11), ( 89, 59), ( 45, 73), ( -1, 78)
        }
    };
    }
}
