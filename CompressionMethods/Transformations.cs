


using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PsqtCompression.CompressionMethods;


public static class DCT4
{
    public static double[] Forward(double[] inputs)
    {
        int N = inputs.Length;
        var output = new double[N];

        for (int k = 0; k < N; k++)
        {
            output[k] = 0;

            for (int n = 0; n < N; n++)
            {
                output[k] += inputs[n] * Math.Cos(Math.PI / N * (n + 0.5) * (k + 0.5));
            }
        }

        return output;
    }

    public static double[] Compress(double[] coefficients, double threshhold, out int zeroCounter)
    {
        double[] result = coefficients.Select(x => Math.Abs(x) >= threshhold ? x : 0).ToArray();

        zeroCounter = result.Count(x => x == 0);

        return result;
    }

    public static double[] Inverse(double[] coeffs)
    {
        int N = coeffs.Length;
        var output = new double[N];

        for (int k = 0; k < N; k++)
        {
            output[k] = 0;

            for (int n = 0; n < N; n++)
            {
                output[k] += coeffs[n] * Math.Cos(Math.PI / N * (n + 0.5) * (k + 0.5));
            }

            output[k] *= 2.0 / N;
        }

        return output;
    }
}


public static class DCT
{
    /// <summary>
    /// DCT-III
    /// </summary>
    /// <param name="inputVector"></param>
    /// <returns></returns>
    public static double[] Forward(double[] inputVector)
    {
        int N = inputVector.Length;
        double[] f_hat = new double[N];
        double[] f = inputVector;

        Parallel.For(0, N, k =>
        {
            f_hat[k] = 0.5 * f[0];

            for (int n = 0; n < N; n++)
            {
                f_hat[k] += f[n] * Math.Cos(Math.PI * (k + 0.5) * n / N);
            }
        });

        return f_hat;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coefficients"></param>
    /// <param name="threshhold"></param>
    /// <param name="zeroCounter"></param>
    /// <returns></returns>
    public static double[] Compress(double[] coefficients, double threshhold, out int zeroCounter)
    {
        double[] result = coefficients.Select(x => Math.Abs(x) >= threshhold ? x : 0).ToArray();

        zeroCounter = result.Count(x => x == 0);

        return result;
    }
    public static double[] Compress(double[] coefficients, double threshhold)
    {
        double[] result = coefficients.Select(x => Math.Abs(x) >= threshhold ? x : 0).ToArray();

        return result;
    }


    /// <summary>
    /// DCT-II with a factor of 2/N
    /// </summary>
    /// <param name="coefVector"></param>
    /// <returns></returns>
    public static double[] Inverse(double[] coefVector)
    {
        int N = coefVector.Length;
        var f_hat = coefVector;
        var f = new double[N];

        Parallel.For(0, N, k =>
        {
            for (int n = 0; n < N; n++)
            {
                f[k] += f_hat[n] * Math.Cos(Math.PI * (n + 0.5) * k / N);
            }

            f[k] *= 2.0 / N;
        });

        return f;
    }
    public static double[] Inverse(sbyte[] coefVector)
    {
        int N = coefVector.Length;
        var f_hat = coefVector;
        var f = new double[N];

        Parallel.For(0, N, k =>
        {
            for (int n = 0; n < N; n++)
            {
                f[k] += f_hat[n] * Math.Cos(Math.PI * (n + 0.5) * k / N);
            }

            f[k] *= 2.0 / N;
        });

        return f;
    }
}



public static class DFT
{

    private static Complex Omega(int k, int n, int N) => (-2 * Constants.Pi * Complex.ImaginaryOne * k * n / N).Exp();

    public static Complex[] Forward(int[] inputVector)
    {
        int N = inputVector.Length;
        Complex[] f_hat = new Complex[N];
        Complex[] f = inputVector.Select(x => new Complex(x, 0)).ToArray();

        // Iterate f_hat
        Parallel.For(0, N, k =>
        {
            // Iterate f
            for (int n = 0; n < N; n++)
            {
                f_hat[k] += f[n] * Omega(k, n, N);
            }
        });

        return f_hat;
    }

    public static Complex[] Compress(Complex[] coefficients, double threshhold, out int zeroCounter)
    {
        Complex[] result;

        result = coefficients.Select(x => x.Magnitude >= threshhold ? x : Complex.Zero).ToArray();

        zeroCounter = result.Count(x => x == Complex.Zero);

        return result;
    }

    private static Complex InverseOmega(int k, int n, int N) => (2 * Constants.Pi * Complex.ImaginaryOne * k * n / N).Exp();

    public static double[] Inverse(Complex[] coefficients)
    {
        int N = coefficients.Length;
        Complex[] f_hat = coefficients;
        Complex[] f = new Complex[N];

        // iterate f
        Parallel.For(0, N, n =>
        {
            // init
            f[n] = Complex.Zero;

            // iterate f_hat
            for (int k = 0; k < N; k++)
                f[n] += f_hat[k] * InverseOmega(k, n, N);

            // normalize
            f[n] /= N;
        });

        return f.Select(x => x.Real).ToArray();
    }
}

public class ComplexComparer : IComparer<Complex>
{
    public int Compare(Complex x, Complex y)
    {
        return x.Magnitude.CompareTo(y.Magnitude);
    }
}
