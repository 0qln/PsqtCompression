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


        var squished = MinimalChess.TransformPesto(table, 0, 255);
        Console.WriteLine(Print.CopyPasta(squished));
        Console.WriteLine(Print.CopyPasta(table));

        //Console.WriteLine(table.Min());
        //Console.WriteLine(table.Max());
        //Console.WriteLine(squished.Min());
        //Console.WriteLine(squished.Max());
    }
}