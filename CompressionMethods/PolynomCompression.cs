using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools.V113.Runtime;
using System.Reflection.Metadata;
using System.Globalization;

namespace PsqtCompression.CompressionMethods
{
    internal static class PolynomCompression
    {
        public static List<short> Decompress(List<decimal> polynom, int originalLength)
        {
            var result = new List<decimal>();

            for (int i = 0; i < originalLength; i++)
                result.Add(ExtractY(polynom, i));

            return result.Select(d => (short)d).ToList();
        }
        public static decimal ExtractY(List<decimal> polynom, int x)
        {
            decimal result = 0;

            for (int i = 0; i < polynom.Count(); i++)
            {
                double power = polynom.Count - i - 1;
                result += (decimal)Math.Pow(x, power) * polynom.ElementAt(i);
            }

            return result;
        }

        public static string BuildFunction(List<decimal> polynom)
        {
            string result = string.Empty;

            for (int i = 0; i < polynom.Count; i++)
            {
                double power = polynom.Count - i - 1;
                result += $"{polynom.ElementAt(i)} * x ^ {power}";

                if (i != polynom.Count - 1)
                    result += " + ";
            }

            return result;
        }

        public static List<decimal> Compress(short[] data)
        {
            var driver = new ChromeDriver(new ChromeOptions { });
            driver.Navigate().GoToUrl(@"https://valdivia.staff.jade-hs.de/interpol.html");

            var pointCount = driver.FindElement(By.Id("neq"));
            pointCount.Clear();
            pointCount.SendKeys(data.Length.ToString());
            pointCount.SendKeys(Keys.Enter);

            var degree = driver.FindElement(By.Id("degree"));
            degree.Clear();
            degree.SendKeys((data.Length - 1).ToString());
            degree.SendKeys(Keys.Return);

            IWebElement? finalElement = null;
            for (int i = 0; i < data.Length; i++)
            {
                var webPointX = driver.FindElement(By.Id($"Data{i}_0"));
                var webPointY = driver.FindElement(By.Id($"Data{i}_1"));
                webPointX.SendKeys(i.ToString());
                webPointY.SendKeys(data[i].ToString());

                finalElement = webPointY;
            }
            finalElement?.SendKeys(Keys.Enter);

            var polynom = driver.FindElement(By.Id("messages"));
            var polynomFactors = ExtractFactorsFromPolynom(polynom.Text);

            //polynomFactors.ForEach(f => Console.WriteLine(f));

            List<decimal> result = polynomFactors.Select(n =>
            {
                if (n.Contains('e'))
                {
                    string s = n.Replace(',', '.').Replace('e', 'E');
                    return decimal.Parse(s, NumberStyles.AllowExponent | NumberStyles.Float, CultureInfo.InvariantCulture);
                }

                return Convert.ToDecimal(n);
            }).ToList();

            //Console.WriteLine(polynom.Text);

            result.ForEach(Console.WriteLine);
            return result;
        }

        public static List<string> ExtractFactorsFromPolynom(string polynom)
        {
            List<string> factors = new List<string>();


            int currIndex = 0;
            while (currIndex < polynom.Length)
            {
                char sign = polynom[currIndex];

                if (sign != '-' &&
                    sign != '+' &&
                    sign != '=') currIndex++;

                else
                {
                    string number = "";

                    while (currIndex < polynom.Length
                        && polynom[currIndex] != 'x')
                    {
                        if (polynom[currIndex] != ' ') number += polynom[currIndex];
                        currIndex++;
                    }

                    string factor = new string(sign + number)
                        .Replace(" ", "")
                        .Replace("++", "+")
                        .Replace("--", "-")
                        .Replace("==", "")
                        .Replace(".", ",");

                    if (factor == "-") factor = "-1";
                    if (factor == "+") factor = "+1";

                    Console.WriteLine(factor);

                    factors.Add(factor);
                }
            }

            return factors;
        }
    }
}
