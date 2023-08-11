
public static class RleCompression
{
    public static byte[] Compress(short[] data)
    {
        var compressedBytes = new List<byte>();
        int count = 1;
        short prevValue = data[0];

        for (int i = 1; i < data.Length; i++)
        {
            if (data[i] == prevValue && count < 255)
            {
                count++;
            }
            else
            {
                compressedBytes.Add((byte)count);
                compressedBytes.AddRange(BitConverter.GetBytes(prevValue));
                prevValue = data[i];
                count = 1;
            }
        }

        compressedBytes.Add((byte)count);
        compressedBytes.AddRange(BitConverter.GetBytes(prevValue));
        return compressedBytes.ToArray();
    }

    public static short[] Decompress(byte[] compressedData)
    {
        var decompressedData = new List<short>();
        int i = 0;

        while (i < compressedData.Length)
        {
            int count = compressedData[i];
            short value = BitConverter.ToInt16(compressedData, i + 1);

            for (int j = 0; j < count; j++)
            {
                decompressedData.Add(value);
            }

            i += 3; // Each RLE segment is 3 bytes: 1 byte for count, 2 bytes for value.
        }

        return decompressedData.ToArray();
    }
}