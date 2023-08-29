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

        var compressed = Psqt.Compress(table);
        var uncompressed = Psqt.Decompress<byte>(compressed);

        Console.WriteLine(Print.CopyPasta(uncompressed));
    }
}