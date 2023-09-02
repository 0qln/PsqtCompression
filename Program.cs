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
        int origBitSize;
        var pesto = PsqtCompression.Data.Pesto.ShortTables;
        var minim = PsqtCompression.Data.MinimalChess.ShortTables;

        //Print.Analyze(Psqt.Compress(pesto, 3, out origBitSize));
        //Print.Analyze(Psqt.Compress(pesto, 4, out origBitSize));

        var table = pesto;
        //{
        //    1, 1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
        //};

        // table.Select((val, idx) => new { val, idx })
        //var grouped = from x in table.Select((val, idx) => new { val, idx })
        //              group x.val by x.idx >= table.Length/2;

        short[][] level0 = table
            .Select((val, idx) => new { val, idx })
            .GroupBy(x => x.idx / 64, x => x.val)
            .Select(x => x.ToArray())
            .ToArray();

        short[][][] level1 = level0
            .Select((val, idx) => new { val, idx })
            .GroupBy(x => x.idx / 6, x => x.val)
            .Select(x => x.ToArray())
            .ToArray();

        for (int i = 0; i < level1.Length; i++)
        {
            for (int j = 0; j < level1[i].Length; j++)
            {
                Console.WriteLine(Print.CopyPasta(level1[i][j]));
            }
        }



        return;
    }
}