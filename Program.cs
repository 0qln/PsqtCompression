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

        // TODO:
        // negatives DO work ???

        sbyte pos = 10;
        sbyte neg = -10;

        Test(pos);
        Test(neg);

        void Test<T>(T num)
        {
            Console.WriteLine(num);

            ulong compressedNum = TokenCompression.Cramp(num);
            var uncompressdNum = TokenCompression.Extract<T>(compressedNum, 0);

            Console.WriteLine(uncompressdNum);
        }
    }
}