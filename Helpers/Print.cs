using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression.Helpers
{
    internal class Print
    {
        private static int _printedCount;

        public static string CopyPasta<T>(T[] values)
        {
            string result = string.Empty;
            string type = typeof(T).Name;
            result += $"{type}[] {$"Values{_printedCount++}"}";
            result += "\n";
            result += "{";
            result += "\n";
            for (int i = 0; i < values.Length - 1; i++)
            {
                result += ToString(values[i]);
                result += ", ";
                if (i % 8 == 7)
                    result += "\n";
                if (i % 64 == 63)
                    result += "\n";
            }
            result += ToString(values[^1]);
            result += "\n";
            result += "};";
            return result;


            string ToString(T value) => value.ToString().PadLeft(5);
        }

        public static string CopyPasta<TIn, TOut>(TIn[] values, [NotNull] Func<TIn, TOut> predicate)
            where TIn : notnull
        {
            string result = string.Empty;
            string type = typeof(TOut).Name;
            result += $"{type}[] {$"Values{_printedCount++}"}";
            result += "\n";
            result += "{";
            result += "\n";
            for (int i = 0; i < values.Length - 1; i++)
            {
                result += ToString(values[i]);
                if (i % 8 == 7)
                    result += "\n";
                if (i % 64 == 63)
                    result += "\n";
            }
            result += ToString(values[^1]);
            result += "\n";
            result += "};";
            return result;

            string ToString(TIn value) => predicate(value).ToString().PadLeft(5);
        }



        public static void Analyze<T>(T[] input)
        {
            Console.WriteLine(Print.CopyPasta(input));
            Console.WriteLine($"Size: {input.Length}");
            Console.WriteLine($"Min: {input.Min()}");
            Console.WriteLine($"Max: {input.Max()}");
        }


        public static void PrintArray<T>(T[] array)
        {
            Console.WriteLine();
            Array.ForEach(array, x => Console.WriteLine(x));
            Console.WriteLine();
        }
        public static void PrintArray<T>(T[] array, T[] expected, out double avgDelta, out double worstDelta)
        {
            double[] deltas = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                deltas[i] = Math.Abs(Convert.ToDouble(array[i]) - Convert.ToDouble(expected[i]));

                Console.WriteLine(
                $"Value: {array[i]?.ToString()?.PadRight(30)}" +
                $"Expected: {expected[i]?.ToString()?.PadRight(30)}" +
                $"Delta: {deltas[i]}");
            }

            avgDelta = deltas.Sum() / deltas.Length;
            worstDelta = deltas.Max();
        }
        public static void PrintArray<T>(T[] array, T[] expected)
        {
            double[] deltas = new double[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                deltas[i] = Math.Abs(Convert.ToDouble(array[i]) - Convert.ToDouble(expected[i]));

                Console.WriteLine(
                $"Value: {array[i]?.ToString()?.PadRight(30)}" +
                $"Expected: {expected[i]?.ToString()?.PadRight(30)}" +
                $"Delta: {deltas[i]}");
            }
        }



        static Bitmap ShowCoefficients(Complex[] data)
        {
            short ValueOf(Complex num) => (short)num.Magnitude;
            return VisualizeAsGraph(Misc.AbsuluteOf(data.Select(ValueOf).ToArray()), 5, true);
        }



        static void PrintAsPSQT(short[] data, bool white)
        {
            if (data.Length != 64 &&
                data.Length != 32) throw new Exception($"Expected Length: {32}/{64}. Data Length {data.Length}");



            if (data.Length == 64)
            {
                for (int i = 0; i < 64; i++)
                {
                    Console.Write((data[white ? i : i ^ 56] + "").PadLeft(6));

                    if (i % 8 == 0)
                        Console.WriteLine();
                }
            }

            if (data.Length == 32)
            {
                for (int i = 0; i < 64; i++)
                {
                    if (i % 8 == 0)
                        Console.WriteLine();


                    Console.Write((data[Misc.MapSquareIndexToPsqa(i, white)] + "").PadLeft(6));
                }
            }
        }



        static List<IWebDriver> _drivers = new();
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen", Justification = "<Ausstehend>")]
        static void ShowImage(Bitmap image)
        {
            string tempDiscSave = "\\temp.png";
            image.Save(tempDiscSave);
            string path = Path.GetFullPath(tempDiscSave);
            var options = new ChromeOptions { LeaveBrowserRunning = false };
            var driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(path);
            _drivers.Add(driver);

            Program.OnApplicationExit += () =>
            {
                File.Delete(tempDiscSave);
            };
        }




        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen", Justification = "<Ausstehend>")]
        static Bitmap VisualizeAsGraph(short[] data, int SCALE_FACTOR = 50, bool fill = false)
        {
            Bitmap bitmap = new Bitmap(SCALE_FACTOR * data.Length + SCALE_FACTOR, SCALE_FACTOR * Misc.AbsuluteOf(data).Max() + SCALE_FACTOR);

            for (int x = 0; x < bitmap.Width; x++)
                for (int y = 0; y < bitmap.Height; y++)
                    bitmap.SetPixel(x, y, Color.Black);

            for (int i = 0; i < data.Length; i++)
            {
                int x = i * SCALE_FACTOR;
                int y = Misc.AbsuluteOf(data)[i] * SCALE_FACTOR;

                do
                {
                    for (int x2 = 0; x2 < SCALE_FACTOR; x2++)
                        for (int y2 = 0; y2 < SCALE_FACTOR; y2++)
                            bitmap.SetPixel(x2 + x, y2 + y, Color.White);

                    y++;
                }
                while (fill && y + SCALE_FACTOR < bitmap.Height);
            }

            return bitmap;
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen", Justification = "<Ausstehend>")]
        static Bitmap VisualizePSQT(short[] data, bool white)
        {
            if (data.Length != 64 &&
                data.Length != 32) throw new Exception($"Expected Length: {32}/{64}. Data Length {data.Length}");

            const int SCALE_FACTOR = 1;

            Bitmap bitmap = new Bitmap(8 * SCALE_FACTOR, 8 * SCALE_FACTOR);

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    int value = 0;
                    int index = y * 8 + x;

                    if (data.Length == 64)
                        value = data[white ? index : index ^ 56];

                    if (data.Length == 32)
                        value = data[Misc.MapSquareIndexToPsqa(index, white)];


                    for (int x2 = 0; x2 < SCALE_FACTOR; x2++)
                        for (int y2 = 0; y2 < SCALE_FACTOR; y2++)
                            bitmap.SetPixel(x * SCALE_FACTOR + x2, y * SCALE_FACTOR + y2, Color.FromArgb(value, value, value));
                }

            return bitmap;
        }

    }
}
