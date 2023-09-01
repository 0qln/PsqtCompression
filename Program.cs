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
        var pesto = PsqtCompression.Data.Pesto.ShortTables;
        var minim = PsqtCompression.Data.MinimalChess.ShortTables;

        var compressed = Psqt.Compress(pesto, 4, out int origBitSize);
        var decompressed = Psqt.Decompress(compressed, origBitSize);
        Print.Analyze(compressed);
        Print.Analyze(decompressed);

        Console.WriteLine(Print.CopyPasta(compressed.Decompress(origBitSize)));

        Console.WriteLine(origBitSize);
    }
}