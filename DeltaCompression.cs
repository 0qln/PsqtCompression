
public static class DeltaCompression
{
    public static byte[] Compress(short[] data)
    {
        if (data == null || data.Length == 0)
            throw new ArgumentException("Input data cannot be null or empty.");

        List<byte> compressedData = new();
        compressedData.AddRange(BitConverter.GetBytes(data.Length));

        short prevValue = data[0];
        foreach (var value in data)
        {
            short diff = (short)(value - prevValue);
            compressedData.AddRange(BitConverter.GetBytes(diff));
            prevValue = value;
        }

        return compressedData.ToArray();
    }

    public static short[] Decompress(byte[] compressedData)
    {
        if (compressedData == null || compressedData.Length < 2)
            throw new ArgumentException("Compressed data is invalid.");

        int dataLength = BitConverter.ToInt32(compressedData, 0);
        short[] decompressedData = new short[dataLength];

        int dataIndex = 0;
        short prevValue = 0;
        for (int i = 4; i < compressedData.Length; i += 2)
        {
            short diff = BitConverter.ToInt16(compressedData, i);
            prevValue += diff;
            decompressedData[dataIndex] = prevValue;
            dataIndex++;
        }

        return decompressedData;
    }
}