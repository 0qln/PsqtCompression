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
        var table = PsqtCompression.Data.MinimalChess.UShortTables;

        for (uint i = 0; i < 6; i++)
        {
            var compressed = Psqt.Compress(table, i, out int origBitSize);
            var decompressed = Psqt.Decompress(compressed, origBitSize);

            Console.WriteLine(Print.CopyPasta(compressed));
            Console.WriteLine(Print.CopyPasta(decompressed));
            Console.WriteLine(compressed.Length);
        }


    }
}