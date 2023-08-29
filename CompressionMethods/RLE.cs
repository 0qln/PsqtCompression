
public static class RleCompression
{
    public static double[] Compress(double[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentException("Input array cannot be null or empty.");

        List<double> compressedData = new List<double>();

        int count = 1;
        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] == input[i - 1])
            {
                count++;
            }
            else
            {
                compressedData.Add(input[i - 1]);
                compressedData.Add(count);
                count = 1;
            }
        }

        // Add the last element and its count
        compressedData.Add(input[input.Length - 1]);
        compressedData.Add(count);

        return compressedData.ToArray();
    }

    public static double[] Decompress(double[] compressedData)
    {
        if (compressedData == null || compressedData.Length == 0)
            throw new ArgumentException("Compressed data array cannot be null or empty.");

        List<double> decompressedData = new List<double>();

        for (int i = 0; i < compressedData.Length; i += 2)
        {
            double value = compressedData[i];
            int count = (int)compressedData[i + 1];

            for (int j = 0; j < count; j++)
            {
                decompressedData.Add(value);
            }
        }

        return decompressedData.ToArray();
    }
}