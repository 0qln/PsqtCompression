using MathNet.Numerics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PsqtCompression.CompressionMethods;
using PsqtCompression.Helpers;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

static class Program
{
    public static event Action? OnApplicationExit;
    public static void Main(string[] args)
    {
        Main_(args);
        OnApplicationExit?.Invoke();
    }


    public static void Main_(string[] args)
    {
        Console.WriteLine();

        var table = PsqtCompression.Data.MinimalChess.UShortTables;

        //var squished = MinimalChess.TransformPesto<ushort, byte>(table, 0, 255);

        //var compressed = Psqt.Compress(table, 2, out int origBitSize);
        //var decompressed = Psqt.Decompress(compressed, origBitSize);

        ushort[] shorts1 =
        {
            0x0, 
            0xFF,
            0x0,
            0xFF,
        };
        ushort[] shorts2 =
        {
            0x6,
            0x7,
            0x8,
            0x9
        };


        int printBase = 2;
        int normSize = 16;
        int size = 8;
        int padCount = Convert.ToString(long.MaxValue, printBase).Length;
        ulong value1 = TokenCompression.Cramp(shorts1, normSize);
        ulong value2 = TokenCompression.Cramp(shorts1, size);

        var shorts = new List<ushort>(shorts1);
            shorts.AddRange(shorts2);

        var compressed = TokenCompression.CrampAll(shorts.ToArray(), size);


        Console.WriteLine(Print.CopyPasta(shorts1, x => p(x)));
        Console.WriteLine(Print.CopyPasta(shorts2, x => p(x)));
        Console.WriteLine(Print.CopyPasta(compressed, x => p(x)));


        var decompressed = TokenCompression.ExtractAll<uint>(compressed, size);
        Console.WriteLine(Print.CopyPasta(decompressed, x => p(x)));

        ulong x = 0x00_00_00_00_00_00_00_00;
        ulong y = 0xFF_FF_FF_FF_FF_FF_FF_FF;
        ulong z = 0x08_07_06_05_04_03_02_01;

        for (int i = 0; i < 8; i++)
        {
            Console.WriteLine(p(TokenCompression.Extract<ulong>(z, i, 61)));
        }

        //Console.WriteLine(Print.CopyPasta(compressed, x => ((long)x).ToBin()));
        //Console.WriteLine(Print.CopyPasta(decompressed, x => x.ToBin()));

        //Console.WriteLine(Misc.NumBitsUsed(table.Max()));


        string p<T>(T x) => Convert.ToString((long)(dynamic)x, printBase).PadLeft(padCount, '0') + "\n";
    }
}