using MathNet.Numerics;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PsqtCompression;
using System.Drawing;
using System.Numerics;

static class Program
{
    static event Action? OnApplicationExit;


    static void PrintAsPSQT(short[] data, bool white)
    {
        if (data.Length != 64 &&
            data.Length != 32) throw new Exception($"Expected Length: {32}/{64}. Data Length {data.Length}");



        if (data.Length == 64)
        {
            for (int i = 0; i < 64; i++)
            {
                Console.Write((data[white ? i : i ^ 56]+"").PadLeft(6));

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


                Console.Write((data[Helpers.MapSquareIndexToPsqa(i, white)] + "").PadLeft(6));
            }
        }
    }


    static string ToBin(this short num)
    {
        return Convert.ToString(num, 10).PadLeft(4, '0').ToUpper();
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

        OnApplicationExit += () =>
        {
            File.Delete(tempDiscSave);
        };
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
                value = data[Helpers.MapSquareIndexToPsqa(index, white)];


                for (int x2 = 0; x2 < SCALE_FACTOR; x2++)
                for (int y2 = 0; y2 < SCALE_FACTOR; y2++)
                    bitmap.SetPixel(x * SCALE_FACTOR + x2, y * SCALE_FACTOR + y2, Color.FromArgb(value, value, value));
            }

        return bitmap;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Plattformkompatibilität überprüfen", Justification = "<Ausstehend>")]
    static Bitmap VisualizeAsGraph(short[] data, int SCALE_FACTOR = 50, bool fill = false)
    {
        Bitmap bitmap = new Bitmap(SCALE_FACTOR * data.Length + SCALE_FACTOR, SCALE_FACTOR * AbsuluteOf(data).Max() + SCALE_FACTOR);

        for (int x = 0; x < bitmap.Width; x++)
            for (int y = 0; y < bitmap.Height; y++)
                bitmap.SetPixel(x, y, Color.Black);

        for (int i = 0; i < data.Length; i++)
        {
            int x = i * SCALE_FACTOR;
            int y = AbsuluteOf(data)[i] * SCALE_FACTOR;

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


    static short[] AbsuluteOf(short[] input) => input.Select(e => (short)(e + Math.Abs(input.Min()))).ToArray();


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

    static void For(int startValue_inclusive, int endIndex_exclusive, Action<int> predicate) {
        for (int i = startValue_inclusive; i < endIndex_exclusive; i++) predicate.Invoke(i);
    }


    static Bitmap ShowCoefficients(Complex[] data)
    {
        short ValueOf(Complex num) => (short)num.Magnitude;
        return VisualizeAsGraph(AbsuluteOf(data.Select(ValueOf).ToArray()), 5, true);
    }


    //static Bitmap ShowFFT2(short[] data)
    //{
    //    if (data.Length != 64) throw new Exception($"Expected Length: 64. Data Length {data.Length}");


    //    const int SCALE_FACTOR = 1;
    //    Bitmap bitmap = new Bitmap(8 * SCALE_FACTOR, 8 * SCALE_FACTOR);
    //    short[,] imageData = new short[8, 8];
    //    for (int i = 0; i < 64; i++)
    //    {
    //        int x = i % 8;
    //        int y = i / 8;
    //        imageData[x,y] = data[i];
    //    }

    //    // iterate cols
    //    for (int x = 0; x < 8; x++)
    //    {
    //        // iterate rows
    //        for (int y = 0; y < 8; y++)
    //        {

    //        }
    //    }
    //}

    static int CountMaxBits(short value)
    {
        int count = 0;
        while (value > 0)
        {
            value >>= 1;
            count++;
        }
        return count;
    }


    static string CopyPasta<T>(T[] values)
    {
        string result = string.Empty;
        string type = typeof(T).Name;
        result += $"{type}[] {nameof(values)}"; 
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

        string ToString<T>(T value) => value.ToString().PadLeft(5); 
    }

    static string CopyPasta<TIn, TOut>(TIn[] values, Func<TIn, TOut> predicate)
    {
        string result = string.Empty;
        string type = typeof(TOut).Name;
        result += $"{type}[] {nameof(values)}";
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

        string ToString(TIn value) => predicate(value).ToString().PadLeft(5);
    }

    // NOTE:    Absolute EG can be compressed to byte
    public static void Main(string[] args)
    {
        var dataMG = PsqtData.MidgameTablesNorm;
        var dataEG = PsqtData.EndgameTablesNorm;


        Console.WriteLine();

        var table = PsqtData.UsTablesNorm;

        var compressed = Pesto.Compress(table);
        var decompressed = Pesto.Decompress<byte>(compressed);

        Console.WriteLine(CopyPasta(decompressed));
        Console.WriteLine(CopyPasta(table));

        //for (int i = 0; i < 6; i++)
        //{
        //    Console.WriteLine(Helpers.GetFromCompressed(i, 0, 6));
        //}

        //Console.WriteLine(CopyPasta(PsqtData.PiecesNorm, (short x) => (short)(x - 128)));


        //Console.WriteLine(CopyPasta(PsqtData.TablesNorm));
        //Console.WriteLine($"Range: {range}");

        //Console.WriteLine("Numbers not occuring: ");
        //for (int i = 0; i < occurances.Length; i++)
        //    if (occurances[i] == false) Console.WriteLine(i);

        //Console.WriteLine("Numbers occuring: ");
        //for (int i = 0; i < occurances.Length; i++)
        //    if (occurances[i] == true) Console.WriteLine(i);

        return;
        
        //var compressed = Pesto.Compress(data, 0);

        //var decompressed = Pesto.Decompress(compressed);

        //PrintArray(decompressed, data, out double avgError, out double worstCaseError);
        
        //Console.WriteLine(compressed.Where(x => x != 0).Count());
        //Console.WriteLine(decompressed.Length);
        //Console.WriteLine(avgError);
        //Console.WriteLine(worstCaseError);


        //return;

        //var f_hat = DCT.Forward(data);
        //var f = DCT.Inverse(f_hat);

        //bool compression100 = false;
        //double lastCompresion = 0;
        //double bestCompressionScore = -1;
        //int bestCompressionThreshhold = -1;

        //for (int threshhold = 0; threshhold < f_hat.Max() + 100 && !compression100; threshhold += 10)
        //{
        //    var f_hat_compressed = DCT.Compress(f_hat, threshhold, out int zeros);
        //    var f_compressed = DCT.Inverse(f_hat_compressed);


        //    int[] order = f.Select(x => Array.IndexOf(f, x)).ToArray();
        //    int[] order_compressed = f_compressed.Select(x => Array.IndexOf(f_compressed, x)).ToArray();
            
        //    // wether the order of elements is preserved or not
        //    bool orderPreserverd = true;
        //    // the amount of times the order was violated
        //    // set this to -1, if it is not needed
        //    // TODO: this doesn't work?? getting weird results
        //    int orderViolations = -1;

        //    for (int i = 0; i < order.Length; i++)
        //        if (order[i] != order_compressed[i])
        //        {
        //            orderPreserverd = false;

        //            if (orderViolations == -1)
        //                break;
        //            else
        //                orderViolations++;
        //        }


        //    double[] deltas = new double[f.Length];
        //    for (int i = 0; i < deltas.Length; i++)
        //        deltas[i] = Math.Abs(f_compressed[i] - f[i]);
            

        //    // the avarage case error
        //    var avgDelta = deltas.Sum() / deltas.Length;
        //    // the worst case error
        //    var worstDelta = deltas.Max();
        //    // the ratio from zeros to non zeros in the coefficients
        //    // high value: lots of compression, low value: few compression
        //    var compressionResult = (double)zeros / (double)f_hat_compressed.Length;
        //    // try to evaluate how good the compression amount relative to the loss is
        //    // high value: good, low value: bad
        //    var compressionScore = compressionResult / avgDelta;

        //    if (compressionResult != lastCompresion)
        //    {
        //        Console.WriteLine("\n");
        //        Console.WriteLine($"COMPRESSION:                                {threshhold}");
        //        Console.WriteLine($"COMPRESSION %:                              {compressionResult}");
        //        Console.WriteLine($"Score:                                      {compressionScore}");
        //        Console.WriteLine($"zeros:                                      {zeros}");
        //        //Console.WriteLine($"non-zeros:                                  {f_hat_compressed.Length - zeros}");
        //        Console.WriteLine($"avg error:                                  {avgDelta}");
        //        Console.WriteLine($"worst case error:                           {worstDelta}");
        //        //Console.WriteLine($"avg error relative to compression:          {avgDelta / zeros}");
        //        //Console.WriteLine($"worst case error relative to compression:   {worstDelta / zeros}");
        //        Console.WriteLine($"ORDER PRESERVERD:                           {orderPreserverd}");
        //        if (orderViolations != -1)
        //        Console.WriteLine($"ORDER VIOLATIONS:                           {orderViolations}");
        //    }


        //    if (compressionResult == 1) compression100 = true;
        //    lastCompresion = compressionResult;

        //    if (compressionScore > bestCompressionScore
        //        && threshhold > 200)
        //    {
        //        bestCompressionScore = compressionScore;
        //        bestCompressionThreshhold = threshhold;
        //    }
        //}


        //Console.WriteLine("\n Best compression: ");
        //Console.WriteLine($"Threshhold: {bestCompressionThreshhold}");
        //Console.WriteLine($"Score: {bestCompressionScore}");


        Console.Read();
        OnApplicationExit?.Invoke();
    }




    static void PrintPsqa()
    {
        for (int piece = 0; piece < PiecePsqa.Length / 16; piece++)
        {
            Console.WriteLine();
            Console.WriteLine();
            for (int value = 0; value < 16; value++)
            {
                Console.Write("0b");

                Console.Write(ExtractShort(PiecePsqa[piece * 16 + value], 0).ToBin());
                Console.Write("_");
                Console.Write(ExtractShort(PiecePsqa[piece * 16 + value], 1).ToBin());
                Console.Write("__");
                Console.Write(ExtractShort(PiecePsqa[piece * 16 + value], 2).ToBin());
                Console.Write("_");
                Console.Write(ExtractShort(PiecePsqa[piece * 16 + value], 3).ToBin());

                Console.Write(", ");
                Console.WriteLine();
            }
        }
    }


    static short ExtractShort(ulong u64, int index) => (short)(u64 >> (48 - index * 16) & 0xFFFFul);


    static ulong[] PawnPsqa =
    {
        // Pawn
        // length: 24
            0b0000000001111110_0000000011000110__0000000010000000_0000000011001000, 0b0000000010000111_0000000011010111__0000000010001110_0000000011010011, 0b0000000010001100_0000000011011110__0000000010010001_0000000011010100, 0b0000000010000101_0000000011001000__0000000001111001_0000000010111100,
            0b0000000001110011_0000000011000101__0000000001101101_0000000011000111, 0b0000000010000111_0000000011000100__0000000010001011_0000000011010011, 0b0000000010011011_0000000011010000__0000000010010011_0000000011010001, 0b0000000010000010_0000000011000110__0000000001101000_0000000011001001,
            0b0000000001111001_0000000011010101__0000000001101000_0000000011001111, 0b0000000010000100_0000000011000110__0000000010001111_0000000011001100, 0b0000000010100011_0000000011000000__0000000010001101_0000000011000001, 0b0000000001111110_0000000011000011__0000000001110111_0000000011001000,
            0b0000000010000111_0000000011011010__0000000001111000_0000000011010100, 0b0000000001110001_0000000011010000__0000000001111110_0000000011001000, 0b0000000010000111_0000000011001001__0000000001111100_0000000011001010, 0b0000000001110000_0000000011011100__0000000010000001_0000000011010111,
            0b0000000001111111_0000000011101001__0000000001110001_0000000011100000, 0b0000000001110110_0000000011100001__0000000010010010_0000000011101011, 0b0000000001110100_0000000011101100__0000000001110111_0000000011010111, 0b0000000001101110_0000000011010110__0000000001110001_0000000011011100,
            0b0000000001110101_0000000011001101__0000000010000010_0000000011000000, 0b0000000001111010_0000000011011011__0000000001110001_0000000011100100, 0b0000000010000000_0000000011100110__0000000001101110_0000000011011111, 0b0000000010000110_0000000011010101__0000000001110011_0000000011010101,
    };

    static ulong[] PiecePsqa =
    {        

        // Knight
        // length: 16
            0b0000001001011110_0000001011110110__0000001010110001_0000001100010101, 0b0000001011000011_0000001100100101__0000001011000100_0000001101000001, 0b0000001011000000_0000001100010011__0000001011100100_0000001100100000, 0b0000001011110010_0000001101000100__0000001011111110_0000001101011110,
            0b0000001011010000_0000001100101110__0000001011111100_0000001100111011, 0b0000001100010011_0000001101001110__0000001100011001_0000001101110011, 0b0000001011101010_0000001100110011__0000001100010101_0000001101010100, 0b0000001100110101_0000001101100011__0000001100111110_0000001101110010,
            0b0000001011101011_0000001100101001__0000001100011010_0000001101000110, 0b0000001100111001_0000001101011111__0000001101000000_0000001101111101, 0b0000001100000100_0000001100100011__0000001100100011_0000001100101010, 0b0000001101000111_0000001101000110__0000001101000010_0000001101100111,
            0b0000001011001010_0000001100010001__0000001011110010_0000001100100100, 0b0000001100010001_0000001100100011__0000001100110010_0000001101100010, 0b0000001001000100_0000001011110010__0000001010111010_0000001011111110, 0b0000001011010101_0000001100011110__0000001011110011_0000001101000101,

        // Bishop
        // length: 16
            0b0000001100010100_0000001101101011__0000001100110101_0000001101111110, 0b0000001100110011_0000001101111001__0000001100101001_0000001110001011, 0b0000001100101110_0000001101111001__0000001100111111_0000001110001010, 0b0000001101000110_0000001110000111__0000001100111100_0000001110010100,
            0b0000001100110100_0000001110001000__0000001101001000_0000001110010010, 0b0000001100110101_0000001110010010__0000001101000101_0000001110011010, 0b0000001100110101_0000001110000101__0000001101000001_0000001110001111, 0b0000001101001011_0000001110010011__0000001101010100_0000001110011111,
            0b0000001100110001_0000001110000111__0000001101001101_0000001110010010, 0b0000001101001000_0000001110001001__0000001101001111_0000001110011110, 0b0000001100101110_0000001101111110__0000001100111101_0000001110010111, 0b0000001100111010_0000001110010110__0000001101000001_0000001110010111,
            0b0000001100101101_0000001101111101__0000001100101111_0000001110000101, 0b0000001100111101_0000001110010010__0000001100111001_0000001110010100, 0b0000001100010111_0000001101110011__0000001100111010_0000001101110110, 0b0000001100101111_0000001101111001__0000001100101001_0000001110000010,

        // Rook
        // length: 16
            0b0000010011011101_0000010101011011__0000010011101000_0000010101010111, 0b0000010011101110_0000010101011010__0000010011110111_0000010101011011, 0b0000010011100111_0000010101011000__0000010011101111_0000010101011011, 0b0000010011110100_0000010101100011__0000010100000010_0000010101100010,
            0b0000010011100011_0000010101101010__0000010011110001_0000010101011100, 0b0000010011111011_0000010101100010__0000010011111111_0000010101011110, 0b0000010011101111_0000010101011110__0000010011110111_0000010101100101, 0b0000010011111000_0000010101011011__0000010011110110_0000010101101011,
            0b0000010011100001_0000010101011111__0000010011101101_0000010101101100, 0b0000010011111000_0000010101101011__0000010011111111_0000010101011110, 0b0000010011100110_0000010101101010__0000010011111010_0000010101100101, 0b0000010100000010_0000010101011101__0000010100001000_0000010101101110,
            0b0000010011111010_0000010101101000__0000010100001000_0000010101101001, 0b0000010100001100_0000010101111000__0000010100001110_0000010101011111, 0b0000010011101011_0000010101110110__0000010011101001_0000010101100100, 0b0000010011111011_0000010101110111__0000010100000101_0000010101110001,

        // Queen
        // length: 16
            0b0000100111101101_0000101000110101__0000100111100101_0000101001000001, 0b0000100111100101_0000101001001011__0000100111101110_0000101001100000, 0b0000100111100111_0000101001000100__0000100111101111_0000101001011011, 0b0000100111110010_0000101001100100__0000100111110110_0000101001110110,
            0b0000100111100111_0000101001010011__0000100111110000_0000101001101000, 0b0000100111110111_0000101001110001__0000100111110001_0000101001111101, 0b0000100111101110_0000101001100011__0000100111101111_0000101001110111, 0b0000100111110011_0000101010000111__0000100111110010_0000101010010010,
            0b0000100111101010_0000101001011101__0000100111111000_0000101001110100, 0b0000100111110110_0000101010000011__0000100111101111_0000101010001111, 0b0000100111100110_0000101001010100__0000100111110100_0000101001101000, 0b0000100111110000_0000101001101111__0000100111110010_0000101001111011,
            0b0000100111100101_0000101001001000__0000100111110000_0000101001011111, 0b0000100111110100_0000101001100010__0000100111110010_0000101001110010, 0b0000100111101000_0000101000110000__0000100111101000_0000101001000110, 0b0000100111101011_0000101001001111__0000100111101000_0000101001011000,

        // King
        // length: 16
            0b0000110011000111_0000101110111001__0000110011111111_0000101111100101, 0b0000110011000111_0000110000001101__0000110001111110_0000110000000100, 0b0000110011001110_0000101111101101__0000110011100111_0000110000011100, 0b0000110010100010_0000110000111101__0000110001101011_0000110000111111,
            0b0000110001111011_0000110000010000__0000110010111010_0000110000111010, 0b0000110001100001_0000110001100001__0000110000110000_0000110001100111, 0b0000110001011100_0000110000011111__0000110001110110_0000110001010100, 0b0000110001000010_0000110001100100__0000110000011010_0000110001100100,
            0b0000110001010010_0000110000011000__0000110001101011_0000110001011110, 0b0000110000100001_0000110001111111__0000101111111110_0000110001111111, 0b0000110000110011_0000110000010100__0000110001001001_0000110001100100, 0b0000110000001001_0000110001110000__0000101111010111_0000110001110111,
            0b0000110000010000_0000101111100111__0000110000110000_0000110000110001, 0b0000101111111001_0000110000101100__0000101111011001_0000110000111011, 0b0000101111110011_0000101111000011__0000110000010001_0000101111110011, 0b0000101111100101_0000110000000001__0000101110110111_0000110000000110,
    };

    static void GenerateExtractedRawPsqa()
    {
        Console.WriteLine("\r\n    static short[][] ExtractedRawPsqa =\r\n    {");

        for (int i = 0; i < RawPiecePsqa.Length; i++)
        {
            Console.WriteLine("\r\n        new short[]\r\n        {");

            for (int j = 0; j < RawPiecePsqa[i].Length; j++)
            {
                Console.Write(RawPiecePsqa[i][j].Item1);
                Console.Write(",");
                Console.Write(RawPiecePsqa[i][j].Item2);
                Console.Write(",");
            }

            Console.WriteLine("\r\n        },");
        }

        Console.WriteLine("\r\n    };");
    }


    static void GenerateExtractedRawPsqa_MG()
    {
        Console.WriteLine("\r\n    static short[][] ExtractedRawPsqa_MG =\r\n    {");

        for (int i = 0; i < RawPiecePsqa.Length; i++)
        {
            Console.WriteLine("\r\n        new short[]\r\n        {");

            for (int j = 0; j < RawPiecePsqa[i].Length; j++)
            {
                Console.Write(RawPiecePsqa[i][j].Item1);
                Console.Write(",");
            }

            Console.WriteLine("\r\n        },");
        }

        Console.WriteLine("\r\n    };");
    }


    static void GenerateExtractedRawPsqa_EG()
    {
        Console.WriteLine("\r\n    static short[][] ExtractedRawPsqa_EG =\r\n    {");

        for (int i = 0; i < RawPiecePsqa.Length; i++)
        {
            Console.WriteLine("\r\n        new short[]\r\n        {");

            for (int j = 0; j < RawPiecePsqa[i].Length; j++)
            {
                Console.Write(RawPiecePsqa[i][j].Item2);
                Console.Write(",");
            }

            Console.WriteLine("\r\n        },");
        }

        Console.WriteLine("\r\n    };");
    }



    static void GeneratePsqa()
    {
        Console.WriteLine("\r\n    static short[] Psqa =\r\n    {");

        for (int i = 0; i < ExtractedRawPsqa.Length; i++)
        {
            for (int j = 0; j < ExtractedRawPsqa[i].Length; j++)
            {
                Console.Write(ExtractedRawPsqa[i][j]);
                Console.Write(",");
            }
        }

        Console.WriteLine("\r\n    };");
    }

    static void GeneratePsqa_MG()
    {
        Console.WriteLine("\r\n    static short[] Psqa_MG =\r\n    {");

        for (int i = 0; i < ExtractedRawPsqa_MG.Length; i++)
        {
            for (int j = 0; j < ExtractedRawPsqa_MG[i].Length; j++)
            {
                Console.Write(ExtractedRawPsqa_MG[i][j]);
                Console.Write(",");
            }
        }

        Console.WriteLine("\r\n    };");
    }
    static void GeneratePsqa_EG()
    {
        Console.WriteLine("\r\n    static short[] Psqa_EG =\r\n    {");

        for (int i = 0; i < ExtractedRawPsqa_EG.Length; i++)
        {
            for (int j = 0; j < ExtractedRawPsqa_EG[i].Length; j++)
            {
                Console.Write(ExtractedRawPsqa_EG[i][j]);
                Console.Write(",");
            }
        }

        Console.WriteLine("\r\n    };");
    }

    static void GenerateAbsolutePsqa_MG()
    {
        Console.WriteLine("\r\n    static short[] PsqaAbsulute_MG =\r\n    {");

        for (int i = 0; i < Psqa_MG.Length; i++)
        {
            Console.Write(Psqa_MG[i] + Math.Abs(Psqa_MG.Min()));
            Console.Write(",");
        }

        Console.WriteLine("\r\n    };");
    }
    static void GenerateAbsolutePsqa_EG()
    {
        Console.WriteLine("\r\n    static short[] PsqaAbsulute_EG =\r\n    {");

        for (int i = 0; i < Psqa_EG.Length; i++)
        {
            Console.Write(Psqa_EG[i] + Math.Abs(Psqa_EG.Min()));
            Console.Write(",");
        }

        Console.WriteLine("\r\n    };");
    }


    #region https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function

    /// <summary>
    /// For DCT, use a compression threshhold of ~200
    /// </summary>
    static short[] _pesto =
    {
        // pawn mg
          0,   0,   0,   0,   0,   0,  0,   0,
         98, 134,  61,  95,  68, 126, 34, -11,
         -6,   7,  26,  31,  65,  56, 25, -20,
        -14,  13,   6,  21,  23,  12, 17, -23,
        -27,  -2,  -5,  12,  17,   6, 10, -25,
        -26,  -4,  -4, -10,   3,   3, 33, -12,
        -35,  -1, -20, -23, -15,  24, 38, -22,
          0,   0,   0,   0,   0,   0,  0,   0,

          // pawn eg
          0,   0,   0,   0,   0,   0,   0,   0,
        178, 173, 158, 134, 147, 132, 165, 187,
         94, 100,  85,  67,  56,  53,  82,  84,
         32,  24,  13,   5,  -2,   4,  17,  17,
         13,   9,  -3,  -7,  -7,  -8,   3,  -1,
          4,   7,  -6,   1,   0,  -5,  -1,  -8,
         13,   8,   8,  10,  13,   0,   2,  -7,
          0,   0,   0,   0,   0,   0,   0,   0,

          // knight mg
        -167, -89, -34, -49,  61, -97, -15, -107,
         -73, -41,  72,  36,  23,  62,   7,  -17,
         -47,  60,  37,  65,  84, 129,  73,   44,
          -9,  17,  19,  53,  37,  69,  18,   22,
         -13,   4,  16,  13,  28,  19,  21,   -8,
         -23,  -9,  12,  10,  19,  17,  25,  -16,
         -29, -53, -12,  -3,  -1,  18, -14,  -19,
        -105, -21, -58, -33, -17, -28, -19,  -23,

        // knight eg
        -58, -38, -13, -28, -31, -27, -63, -99,
        -25,  -8, -25,  -2,  -9, -25, -24, -52,
        -24, -20,  10,   9,  -1,  -9, -19, -41,
        -17,   3,  22,  22,  22,  11,   8, -18,
        -18,  -6,  16,  25,  16,  17,   4, -18,
        -23,  -3,  -1,  15,  10,  -3, -20, -22,
        -42, -20, -10,  -5,  -2, -20, -23, -44,
        -29, -51, -23, -15, -22, -18, -50, -64,

        // bishop mg
        -29,   4, -82, -37, -25, -42,   7,  -8,
        -26,  16, -18, -13,  30,  59,  18, -47,
        -16,  37,  43,  40,  35,  50,  37,  -2,
         -4,   5,  19,  50,  37,  37,   7,  -2,
         -6,  13,  13,  26,  34,  12,  10,   4,
          0,  15,  15,  15,  14,  27,  18,  10,
          4,  15,  16,   0,   7,  21,  33,   1,
        -33,  -3, -14, -21, -13, -12, -39, -21,

        // bishop eg
        -14, -21, -11,  -8, -7,  -9, -17, -24,
         -8,  -4,   7, -12, -3, -13,  -4, -14,
          2,  -8,   0,  -1, -2,   6,   0,   4,
         -3,   9,  12,   9, 14,  10,   3,   2,
         -6,   3,  13,  19,  7,  10,  -3,  -9,
        -12,  -3,   8,  10, 13,   3,  -7, -15,
        -14, -18,  -7,  -1,  4,  -9, -15, -27,
        -23,  -9, -23,  -5, -9, -16,  -5, -17,

        // rook mg
         32,  42,  32,  51, 63,  9,  31,  43,
         27,  32,  58,  62, 80, 67,  26,  44,
         -5,  19,  26,  36, 17, 45,  61,  16,
        -24, -11,   7,  26, 24, 35,  -8, -20,
        -36, -26, -12,  -1,  9, -7,   6, -23,
        -45, -25, -16, -17,  3,  0,  -5, -33,
        -44, -16, -20,  -9, -1, 11,  -6, -71,
        -19, -13,   1,  17, 16,  7, -37, -26,
        
        // rook eg
        13, 10, 18, 15, 12,  12,   8,   5,
        11, 13, 13, 11, -3,   3,   8,   3,
         7,  7,  7,  5,  4,  -3,  -5,  -3,
         4,  3, 13,  1,  2,   1,  -1,   2,
         3,  5,  8,  4, -5,  -6,  -8, -11,
        -4,  0, -5, -1, -7, -12,  -8, -16,
        -6, -6,  0,  2, -9,  -9, -11,  -3,
        -9,  2,  3, -1, -5, -13,   4, -20,
        
        // queen mg
        -28,   0,  29,  12,  59,  44,  43,  45,
        -24, -39,  -5,   1, -16,  57,  28,  54,
        -13, -17,   7,   8,  29,  56,  47,  57,
        -27, -27, -16, -16,  -1,  17,  -2,   1,
         -9, -26,  -9, -10,  -2,  -4,   3,  -3,
        -14,   2, -11,  -2,  -5,   2,  14,   5,
        -35,  -8,  11,   2,   8,  15,  -3,   1,
         -1, -18,  -9,  10, -15, -25, -31, -50,
        
        // queen eg
         -9,  22,  22,  27,  27,  19,  10,  20,
        -17,  20,  32,  41,  58,  25,  30,   0,
        -20,   6,   9,  49,  47,  35,  19,   9,
          3,  22,  24,  45,  57,  40,  57,  36,
        -18,  28,  19,  47,  31,  34,  39,  23,
        -16, -27,  15,   6,   9,  17,  10,   5,
        -22, -23, -30, -16, -16, -23, -36, -32,
        -33, -28, -22, -43,  -5, -32, -20, -41,
                
        // king mg
        -65,  23,  16, -15, -56, -34,   2,  13,
         29,  -1, -20,  -7,  -8,  -4, -38, -29,
         -9,  24,   2, -16, -20,   6,  22, -22,
        -17, -20, -12, -27, -30, -25, -14, -36,
        -49,  -1, -27, -39, -46, -44, -33, -51,
        -14, -14, -22, -46, -44, -30, -15, -27,
          1,   7,  -8, -64, -43, -16,   9,   8,
        -15,  36,  12, -54,   8, -28,  24,  14,
        
        // king eg
        -74, -35, -18, -18, -11,  15,   4, -17,
        -12,  17,  14,  17,  17,  38,  23,  11,
         10,  17,  23,  15,  20,  45,  44,  13,
         -8,  22,  24,  27,  26,  33,  26,   3,
        -18,  -4,  21,  24,  27,  23,   9, -11,
        -19,  -3,  11,  21,  23,  16,   7,  -9,
        -27, -11,   4,  13,  14,   4,  -5, -17,
        -53, -34, -21, -11, -28, -14, -24, -43
    };

    static short[] mg_pawn_table = {
          0,   0,   0,   0,   0,   0,  0,   0,
         98, 134,  61,  95,  68, 126, 34, -11,
         -6,   7,  26,  31,  65,  56, 25, -20,
        -14,  13,   6,  21,  23,  12, 17, -23,
        -27,  -2,  -5,  12,  17,   6, 10, -25,
        -26,  -4,  -4, -10,   3,   3, 33, -12,
        -35,  -1, -20, -23, -15,  24, 38, -22,
          0,   0,   0,   0,   0,   0,  0,   0,
    };
    static short[] eg_pawn_table = {
          0,   0,   0,   0,   0,   0,   0,   0,
        178, 173, 158, 134, 147, 132, 165, 187,
         94, 100,  85,  67,  56,  53,  82,  84,
         32,  24,  13,   5,  -2,   4,  17,  17,
         13,   9,  -3,  -7,  -7,  -8,   3,  -1,
          4,   7,  -6,   1,   0,  -5,  -1,  -8,
         13,   8,   8,  10,  13,   0,   2,  -7,
          0,   0,   0,   0,   0,   0,   0,   0,
    };

    static short[] mg_knight_table = {
        -167, -89, -34, -49,  61, -97, -15, -107,
         -73, -41,  72,  36,  23,  62,   7,  -17,
         -47,  60,  37,  65,  84, 129,  73,   44,
          -9,  17,  19,  53,  37,  69,  18,   22,
         -13,   4,  16,  13,  28,  19,  21,   -8,
         -23,  -9,  12,  10,  19,  17,  25,  -16,
         -29, -53, -12,  -3,  -1,  18, -14,  -19,
        -105, -21, -58, -33, -17, -28, -19,  -23,
    };
    static short[] eg_knight_table = {
        -58, -38, -13, -28, -31, -27, -63, -99,
        -25,  -8, -25,  -2,  -9, -25, -24, -52,
        -24, -20,  10,   9,  -1,  -9, -19, -41,
        -17,   3,  22,  22,  22,  11,   8, -18,
        -18,  -6,  16,  25,  16,  17,   4, -18,
        -23,  -3,  -1,  15,  10,  -3, -20, -22,
        -42, -20, -10,  -5,  -2, -20, -23, -44,
        -29, -51, -23, -15, -22, -18, -50, -64,
    };

    static short[] mg_bishop_table = {
        -29,   4, -82, -37, -25, -42,   7,  -8,
        -26,  16, -18, -13,  30,  59,  18, -47,
        -16,  37,  43,  40,  35,  50,  37,  -2,
         -4,   5,  19,  50,  37,  37,   7,  -2,
         -6,  13,  13,  26,  34,  12,  10,   4,
          0,  15,  15,  15,  14,  27,  18,  10,
          4,  15,  16,   0,   7,  21,  33,   1,
        -33,  -3, -14, -21, -13, -12, -39, -21,
    };
    static short[] eg_bishop_table = {
        -14, -21, -11,  -8, -7,  -9, -17, -24,
         -8,  -4,   7, -12, -3, -13,  -4, -14,
          2,  -8,   0,  -1, -2,   6,   0,   4,
         -3,   9,  12,   9, 14,  10,   3,   2,
         -6,   3,  13,  19,  7,  10,  -3,  -9,
        -12,  -3,   8,  10, 13,   3,  -7, -15,
        -14, -18,  -7,  -1,  4,  -9, -15, -27,
        -23,  -9, -23,  -5, -9, -16,  -5, -17,
    };

    static short[] mg_rook_table = {
         32,  42,  32,  51, 63,  9,  31,  43,
         27,  32,  58,  62, 80, 67,  26,  44,
         -5,  19,  26,  36, 17, 45,  61,  16,
        -24, -11,   7,  26, 24, 35,  -8, -20,
        -36, -26, -12,  -1,  9, -7,   6, -23,
        -45, -25, -16, -17,  3,  0,  -5, -33,
        -44, -16, -20,  -9, -1, 11,  -6, -71,
        -19, -13,   1,  17, 16,  7, -37, -26,
    };
    static short[] eg_rook_table = {
        13, 10, 18, 15, 12,  12,   8,   5,
        11, 13, 13, 11, -3,   3,   8,   3,
         7,  7,  7,  5,  4,  -3,  -5,  -3,
         4,  3, 13,  1,  2,   1,  -1,   2,
         3,  5,  8,  4, -5,  -6,  -8, -11,
        -4,  0, -5, -1, -7, -12,  -8, -16,
        -6, -6,  0,  2, -9,  -9, -11,  -3,
        -9,  2,  3, -1, -5, -13,   4, -20,
    };

    static short[] mg_queen_table = {
        -28,   0,  29,  12,  59,  44,  43,  45,
        -24, -39,  -5,   1, -16,  57,  28,  54,
        -13, -17,   7,   8,  29,  56,  47,  57,
        -27, -27, -16, -16,  -1,  17,  -2,   1,
         -9, -26,  -9, -10,  -2,  -4,   3,  -3,
        -14,   2, -11,  -2,  -5,   2,  14,   5,
        -35,  -8,  11,   2,   8,  15,  -3,   1,
         -1, -18,  -9,  10, -15, -25, -31, -50,
    };
    static short[] eg_queen_table = {
         -9,  22,  22,  27,  27,  19,  10,  20,
        -17,  20,  32,  41,  58,  25,  30,   0,
        -20,   6,   9,  49,  47,  35,  19,   9,
          3,  22,  24,  45,  57,  40,  57,  36,
        -18,  28,  19,  47,  31,  34,  39,  23,
        -16, -27,  15,   6,   9,  17,  10,   5,
        -22, -23, -30, -16, -16, -23, -36, -32,
        -33, -28, -22, -43,  -5, -32, -20, -41,
    };

    static short[] mg_king_table = {
        -65,  23,  16, -15, -56, -34,   2,  13,
         29,  -1, -20,  -7,  -8,  -4, -38, -29,
         -9,  24,   2, -16, -20,   6,  22, -22,
        -17, -20, -12, -27, -30, -25, -14, -36,
        -49,  -1, -27, -39, -46, -44, -33, -51,
        -14, -14, -22, -46, -44, -30, -15, -27,
          1,   7,  -8, -64, -43, -16,   9,   8,
        -15,  36,  12, -54,   8, -28,  24,  14,
    };
    static short[] eg_king_table = {
        -74, -35, -18, -18, -11,  15,   4, -17,
        -12,  17,  14,  17,  17,  38,  23,  11,
         10,  17,  23,  15,  20,  45,  44,  13,
         -8,  22,  24,  27,  26,  33,  26,   3,
        -18,  -4,  21,  24,  27,  23,   9, -11,
        -19,  -3,  11,  21,  23,  16,   7,  -9,
        -27, -11,   4,  13,  14,   4,  -5, -17,
        -53, -34, -21, -11, -28, -14, -24, -43
    };

    #endregion


    #region Stockfish PSQT

    static short[] PsqaAbsulute_MG =
    {
        26,109,127,128,124,160,174,186,140,184,207,213,166,209,241,250,167,214,245,252,192,223,259,254,134,174,205,238,0,118,145,175,164,197,195,185,190,207,214,204,196,216,197,213,197,209,219,228,193,221,216,223,190,205,202,209,189,191,205,201,167,202,191,185,170,181,187,196,180,188,193,207,176,190,200,204,188,196,197,195,174,186,197,204,179,199,207,213,199,213,217,219,184,182,200,210,204,196,196,205,198,206,209,213,198,207,214,208,205,206,210,209,201,215,213,206,197,211,207,209,196,207,211,209,199,199,202,199,472,528,472,399,479,504,435,380,396,459,370,321,365,391,339,299,355,380,306,271,324,346,282,232,289,321,266,234,260,290,246,200,
    };
    static short[] PsqaAbsulute_EG =
    {
        4,35,51,79,33,46,82,108,60,73,92,129,65,98,113,128,55,84,109,139,49,56,84,117,31,50,49,112,0,12,44,83,60,79,74,92,74,91,88,101,89,99,99,107,86,96,100,112,88,99,90,111,79,104,103,104,78,86,99,101,68,71,74,83,91,87,90,91,88,91,99,98,106,92,98,94,94,101,91,107,95,108,107,94,106,101,93,110,104,105,120,95,118,100,119,113,31,43,53,74,46,69,78,96,61,82,91,103,77,97,113,124,71,94,109,121,62,82,89,101,50,73,76,92,26,48,57,66,101,145,185,176,153,200,233,235,188,230,269,275,203,256,272,272,196,266,299,299,192,272,284,291,147,221,216,231,111,159,173,178,
    };

    static short GetPsq_MG(int piece, int square)
    {
        return Psqa_MG[piece * 32 + square]; 
    }
    static short[] Psqa_MG =
    {
        -175,-92,-74,-73,-77,-41,-27,-15,-61,-17,6,12,-35,8,40,49,-34,13,44,51,-9,22,58,53,-67,-27,4,37,-201,-83,-56,-26,-37,-4,-6,-16,-11,6,13,3,-5,15,-4,12,-4,8,18,27,-8,20,15,22,-11,4,1,8,-12,-10,4,0,-34,1,-10,-16,-31,-20,-14,-5,-21,-13,-8,6,-25,-11,-1,3,-13,-5,-4,-6,-27,-15,-4,3,-22,-2,6,12,-2,12,16,18,-17,-19,-1,9,3,-5,-5,4,-3,5,8,12,-3,6,13,7,4,5,9,8,0,14,12,5,-4,10,6,8,-5,6,10,8,-2,-2,1,-2,271,327,271,198,278,303,234,179,195,258,169,120,164,190,138,98,154,179,105,70,123,145,81,31,88,120,65,33,59,89,45,-1,
    };

    static short GetPsq_EG(int piece, int square)
    {
        return Psqa_EG[piece * 32 + square];
    }
    static short[] Psqa_EG =
    {
        -96,-65,-49,-21,-67,-54,-18,8,-40,-27,-8,29,-35,-2,13,28,-45,-16,9,39,-51,-44,-16,17,-69,-50,-51,12,-100,-88,-56,-17,-40,-21,-26,-8,-26,-9,-12,1,-11,-1,-1,7,-14,-4,0,12,-12,-1,-10,11,-21,4,3,4,-22,-14,-1,1,-32,-29,-26,-17,-9,-13,-10,-9,-12,-9,-1,-2,6,-8,-2,-6,-6,1,-9,7,-5,8,7,-6,6,1,-7,10,4,5,20,-5,18,0,19,13,-69,-57,-47,-26,-54,-31,-22,-4,-39,-18,-9,3,-23,-3,13,24,-29,-6,9,21,-38,-18,-11,1,-50,-27,-24,-8,-74,-52,-43,-34,1,45,85,76,53,100,133,135,88,130,169,175,103,156,172,172,96,166,199,199,92,172,184,191,47,121,116,131,11,59,73,78,
    };

    static short[][] ExtractedRawPsqa_MG =
    {

        new short[]
        {
            -175,-92,-74,-73,-77,-41,-27,-15,-61,-17,6,12,-35,8,40,49,-34,13,44,51,-9,22,58,53,-67,-27,4,37,-201,-83,-56,-26,
        },

        new short[]
        {
            -37,-4,-6,-16,-11,6,13,3,-5,15,-4,12,-4,8,18,27,-8,20,15,22,-11,4,1,8,-12,-10,4,0,-34,1,-10,-16,
        },

        new short[]
        {
            -31,-20,-14,-5,-21,-13,-8,6,-25,-11,-1,3,-13,-5,-4,-6,-27,-15,-4,3,-22,-2,6,12,-2,12,16,18,-17,-19,-1,9,
        },

        new short[]
        {
            3,-5,-5,4,-3,5,8,12,-3,6,13,7,4,5,9,8,0,14,12,5,-4,10,6,8,-5,6,10,8,-2,-2,1,-2,
        },

        new short[]
        {
            271,327,271,198,278,303,234,179,195,258,169,120,164,190,138,98,154,179,105,70,123,145,81,31,88,120,65,33,59,89,45,-1,
        },

    };
    static short[][] ExtractedRawPsqa_EG =
    {

        new short[]
        {
            -96,-65,-49,-21,-67,-54,-18,8,-40,-27,-8,29,-35,-2,13,28,-45,-16,9,39,-51,-44,-16,17,-69,-50,-51,12,-100,-88,-56,-17,
        },

        new short[]
        {
            -40,-21,-26,-8,-26,-9,-12,1,-11,-1,-1,7,-14,-4,0,12,-12,-1,-10,11,-21,4,3,4,-22,-14,-1,1,-32,-29,-26,-17,
        },

        new short[]
        {
            -9,-13,-10,-9,-12,-9,-1,-2,6,-8,-2,-6,-6,1,-9,7,-5,8,7,-6,6,1,-7,10,4,5,20,-5,18,0,19,13,
        },

        new short[]
        {
            -69,-57,-47,-26,-54,-31,-22,-4,-39,-18,-9,3,-23,-3,13,24,-29,-6,9,21,-38,-18,-11,1,-50,-27,-24,-8,-74,-52,-43,-34,
        },

        new short[]
        {
            1,45,85,76,53,100,133,135,88,130,169,175,103,156,172,172,96,166,199,199,92,172,184,191,47,121,116,131,11,59,73,78,
        },

    };
    static short[] Psqa =
    {
        -175,-96,-92,-65,-74,-49,-73,-21,-77,-67,-41,-54,-27,-18,-15,8,-61,-40,-17,-27,6,-8,12,29,-35,-35,8,-2,40,13,49,28,-34,-45,13,-16,44,9,51,39,-9,-51,22,-44,58,-16,53,17,-67,-69,-27,-50,4,-51,37,12,-201,-100,-83,-88,-56,-56,-26,-17,-37,-40,-4,-21,-6,-26,-16,-8,-11,-26,6,-9,13,-12,3,1,-5,-11,15,-1,-4,-1,12,7,-4,-14,8,-4,18,0,27,12,-8,-12,20,-1,15,-10,22,11,-11,-21,4,4,1,3,8,4,-12,-22,-10,-14,4,-1,0,1,-34,-32,1,-29,-10,-26,-16,-17,-31,-9,-20,-13,-14,-10,-5,-9,-21,-12,-13,-9,-8,-1,6,-2,-25,6,-11,-8,-1,-2,3,-6,-13,-6,-5,1,-4,-9,-6,7,-27,-5,-15,8,-4,7,3,-6,-22,6,-2,1,6,-7,12,10,-2,4,12,5,16,20,18,-5,-17,18,-19,0,-1,19,9,13,3,-69,-5,-57,-5,-47,4,-26,-3,-54,5,-31,8,-22,12,-4,-3,-39,6,-18,13,-9,7,3,4,-23,5,-3,9,13,8,24,0,-29,14,-6,12,9,5,21,-4,-38,10,-18,6,-11,8,1,-5,-50,6,-27,10,-24,8,-8,-2,-74,-2,-52,1,-43,-2,-34,271,1,327,45,271,85,198,76,278,53,303,100,234,133,179,135,195,88,258,130,169,169,120,175,164,103,190,156,138,172,98,172,154,96,179,166,105,199,70,199,123,92,145,172,81,184,31,191,88,47,120,121,65,116,33,131,59,11,89,59,45,73,-1,78,
    };

    static short[][] ExtractedRawPsqa =
    {
        new short[]
        {
            -175,-96,-92,-65,-74,-49,-73,-21,-77,-67,-41,-54,-27,-18,-15,8,-61,-40,-17,-27,6,-8,12,29,-35,-35,8,-2,40,13,49,28,-34,-45,13,-16,44,9,51,39,-9,-51,22,-44,58,-16,53,17,-67,-69,-27,-50,4,-51,37,12,-201,-100,-83,-88,-56,-56,-26,-17,
        },

        new short[]
        {
            -37,-40,-4,-21,-6,-26,-16,-8,-11,-26,6,-9,13,-12,3,1,-5,-11,15,-1,-4,-1,12,7,-4,-14,8,-4,18,0,27,12,-8,-12,20,-1,15,-10,22,11,-11,-21,4,4,1,3,8,4,-12,-22,-10,-14,4,-1,0,1,-34,-32,1,-29,-10,-26,-16,-17,
        },

        new short[]
        {
            -31,-9,-20,-13,-14,-10,-5,-9,-21,-12,-13,-9,-8,-1,6,-2,-25,6,-11,-8,-1,-2,3,-6,-13,-6,-5,1,-4,-9,-6,7,-27,-5,-15,8,-4,7,3,-6,-22,6,-2,1,6,-7,12,10,-2,4,12,5,16,20,18,-5,-17,18,-19,0,-1,19,9,13,
        },

        new short[]
        {
            3,-69,-5,-57,-5,-47,4,-26,-3,-54,5,-31,8,-22,12,-4,-3,-39,6,-18,13,-9,7,3,4,-23,5,-3,9,13,8,24,0,-29,14,-6,12,9,5,21,-4,-38,10,-18,6,-11,8,1,-5,-50,6,-27,10,-24,8,-8,-2,-74,-2,-52,1,-43,-2,-34,
        },

        new short[]
        {
            271,1,327,45,271,85,198,76,278,53,303,100,234,133,179,135,195,88,258,130,169,169,120,175,164,103,190,156,138,172,98,172,154,96,179,166,105,199,70,199,123,92,145,172,81,184,31,191,88,47,120,121,65,116,33,131,59,11,89,59,45,73,-1,78,
        },

    };
    static (short, short)[][] RawPiecePsqa =
    {
        new (short, short)[]
        { // Knight
            (-175, -96), (-92, -65), (-74, -49), (-73, -21),
            (-77, -67), (-41, -54), (-27, -18), (-15, 8),
            (-61, -40), (-17, -27), (6, -8), (12, 29),
            (-35, -35), (8, -2), (40, 13), (49, 28),
            (-34, -45), (13, -16), (44, 9), (51, 39),
            (-9, -51), (22, -44), (58, -16), (53, 17),
            (-67, -69), (-27, -50), (4, -51), (37, 12),
            (-201, -100), (-83, -88), (-56, -56), (-26, -17)
        },
        new (short, short)[]
        { // Bishop
            (-37, -40), (-4, -21), (-6, -26), (-16, -8),
            (-11, -26), (6, -9), (13, -12), (3, 1),
            (-5, -11), (15, -1), (-4, -1), (12, 7),
            (-4, -14), (8, -4), (18, 0), (27, 12),
            (-8, -12), (20, -1), (15, -10), (22, 11),
            (-11, -21), (4, 4), (1, 3), (8, 4),
            (-12, -22), (-10, -14), (4, -1), (0, 1),
            (-34, -32), (1, -29), (-10, -26), (-16, -17)
        },
        new (short, short)[]
        { // Rook
            (-31, -9), (-20, -13), (-14, -10), (-5, -9),
            (-21, -12), (-13, -9), (-8, -1), (6, -2),
            (-25, 6), (-11, -8), (-1, -2), (3, -6),
            (-13, -6), (-5, 1), (-4, -9), (-6, 7),
            (-27, -5), (-15, 8), (-4, 7), (3, -6),
            (-22, 6), (-2, 1), (6, -7), (12, 10),
            (-2, 4), (12, 5), (16, 20), (18, -5),
            (-17, 18), (-19, 0), (-1, 19), (9, 13)
        },
        new (short, short)[]
        { // Queen
            (3, -69), (-5, -57), (-5, -47), (4, -26),
            (-3, -54), (5, -31), (8, -22), (12, -4),
            (-3, -39), (6, -18), (13, -9), (7, 3),
            (4, -23), (5, -3), (9, 13), (8, 24),
            (0, -29), (14, -6), (12, 9), (5, 21),
            (-4, -38), (10, -18), (6, -11), (8, 1),
            (-5, -50), (6, -27), (10, -24), (8, -8),
            (-2, -74), (-2, -52), (1, -43), (-2, -34)
        },
        new (short, short)[]
        { // King
            (271,  1), (327, 45), (271, 85), (198, 76) ,
            (278, 53), (303,100), (234,133), (179,135) ,
            (195, 88), (258,130), (169,169), (120,175) ,
            (164,103), (190,156), (138,172), ( 98,172) ,
            (154, 96), (179,166), (105,199), ( 70,199) ,
            (123, 92), (145,172), ( 81,184), ( 31,191) ,
            ( 88, 47), (120,121), ( 65,116), ( 33,131) ,
            ( 59, 11), ( 89, 59), ( 45, 73), ( -1, 78)
        }
    };

    #endregion
}