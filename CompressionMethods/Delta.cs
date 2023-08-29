using System.Numerics;


/// <summary>
/// This is untested.
/// </summary>
public static class Delta
{
    public static DeltaCode<double> Encode(double[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentException("Input array cannot be null or empty.");

        double[] encodedData = new double[input.Length];
        encodedData[0] = input[0];

        for (int i = 1; i < input.Length; i++)
            encodedData[i] = input[i] - input[i - 1];

        DeltaCode<double> code = new DeltaCode<double>(encodedData[0], encodedData.Skip(1).ToArray());

        return code;
    } 

    public static T[] Decode<T>(DeltaCode<T> code)
        where T : IBinaryFloatingPointIeee754<T>
    {
        if (code == null) throw new ArgumentException("Encoded data array cannot be null or empty.");

        T[] decodedData = new T[code.deltas.Length + 1];
        decodedData[0] = code.shift;

        for (int i = 0; i < code.deltas.Length; i++)
            decodedData[i] = decodedData[i] + code.deltas[i];

        return decodedData;
    }
}

public record class DeltaCode<T>(T shift, T[] deltas)
    where T : IBinaryFloatingPointIeee754<T>;



public static class DeltaCompression
{
    public static double[] Encode<T>(T[] input)
    {
        if (input == null || input.Length == 0)
            throw new ArgumentException("Input array cannot be null or empty.");

        var encodedData = new double[input.Length];
        encodedData[0] = (double)(dynamic)input[0];

        for (int i = 1; i < input.Length; i++)
            encodedData[i] = (double)((dynamic)input[i] - (dynamic)input[i - 1]);

        return encodedData;
    }

    public static TOut[] Decode<TOut>(double[] encodedData)
    {
        if (encodedData == null || encodedData.Length == 0)
            throw new ArgumentException("Encoded data array cannot be null or empty.");

        var decodedData = new TOut[encodedData.Length];
        decodedData[0] = (TOut)(dynamic)encodedData[0];

        for (int i = 1; i < encodedData.Length; i++)
            decodedData[i] = (TOut)((dynamic)decodedData[i - 1] + (dynamic)encodedData[i]);

        return decodedData;
    }
}