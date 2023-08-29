using MathNet.Numerics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PsqtCompression;
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

        var table = PsqtCompression.Data.MinimalChess.UsTablesNorm;

        var compressed = Psqt.Compress(table);
        var decompressed = Psqt.Decompress<byte>(compressed);

        Console.WriteLine(Print.CopyPasta(decompressed));
        Console.WriteLine(Print.CopyPasta(table));
    }
}