


using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PsqtCompression
{
    public static class DFT
    {
        private static Complex Omega(int k, int n, int N) => ComplexExtensions.Exp(-2 * Constants.Pi * Complex.ImaginaryOne * k * n / N);
        private static Complex InverseOmega(int k, int n, int N) => ComplexExtensions.Exp(2 * Constants.Pi * Complex.ImaginaryOne * k * n / N);

        public static Complex[] Forward(int[] inputVector)
        {
            int N = inputVector.Length;            
            Complex[] f_hat = new Complex[N];
            Complex[] f = inputVector.Select(x => new Complex(x, 0)).ToArray();

            // Iterate f_hat
            for (int k = 0; k < N; k++)
            {
                // Iterate f
                for (int  n = 0; n < N; n++)
                {
                    f_hat[k] += f[n] * Omega(k, n, N);
                }
            }

            return f_hat;
        }

        public static Complex[] Compress(Complex[] coefficients, double threshhold)
        {
            var result = coefficients.Select(x => x.Magnitude >= threshhold ? x : Complex.Zero).ToArray();
            //var result = coefficients.Where(x => x.Magnitude >= threshhold).ToArray();
            return result;
        }

        public static double[] Inverse(Complex[] inputVector)
        {
            int N = inputVector.Length;
            Complex[] f_hat = inputVector;
            Complex[] f = new Complex[N];

            // iterate f
            for (int n = 0; n < N; n++)
            {
                // init
                f[n] = Complex.Zero;

                // iterate f_hat
                for (int k = 0; k < N; k++)
                    f[n] += f_hat[k] * InverseOmega(k, n, N);

                // normalize
                f[n] /= N;
            }

            return f.Select(x => x.Real).ToArray();
        }

        private static Complex[] Multiply(Complex[,] matrix, Complex[] vector, Func<Complex, Complex>? normalization = null)
        {
            if (matrix.GetLength(0) != vector.Length ||
                matrix.GetLength(1) != vector.Length)
                throw new ArgumentException
                    ($"Matrix and vector were of wrong size " +
                    $"(Matrix: [{matrix.GetLength(0)}; {matrix.GetLength(1)}], Vector: {vector.Length})");

            Complex[] result = new Complex[vector.Length];
            for (int i = 0; i < vector.Length; i++)
                for (int j = 0; j < vector.Length; j++)
                    result[i] += normalization is null 
                        // Apply normally
                        ? matrix[i, j] * vector[i]
                        // Apply with normalization
                        : normalization(matrix[i, j] * vector[i]);

            return result;
        }
    }

    public static class FFT2
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">A 2 dimensional array of equal width and hight.</param>
        /// <returns></returns>
        public static byte[,] Compress(short[,] data)
        {
            if (data.GetLength(0) != data.GetLength(1)) 
                throw new ArgumentException("The data must be of equal width and height.");

            if (Math.Log2(data.GetLength(0)) % 1 != 0)
                throw new ArgumentException("The width and height of the data must be equal to 2^n.");


            

            return null;
        }


        public static byte[,] Decompress(byte[,] frequencySpectrum)
        {
            return null;
        } 
    }
}
