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

        var c1 = Psqt.Compress(pesto);
        var c2 = Psqt.Compress(minim);

        var nc1 = Psqt.Decompress<byte>(c1);
        var nc2 = Psqt.Decompress<byte>(c2);

        Console.WriteLine(Print.CopyPasta(Misc.Delta(nc1, nc2)));
    }
}