using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsqtCompression
{
    class FileDictionary : ConcurrentDictionary<decimal, string>
    {
        // size of decimal in bytes
        private const int DEC_SIZE = sizeof(decimal);
        // string can only be as long as the max decimal 
        private static readonly int STR_SIZE = decimal.MinValue.ToString().Length * sizeof(char);
        // the size of a key value pair (<decimal, string>) in bytes
        private static readonly int PAIR_SIZE = DEC_SIZE + STR_SIZE;
        private const string DISC_FOLDER = "C:\\Users\\User\\AppData\\Local\\FileDictionary";

        private static readonly FileStream _stream = new FileStream(DISC_FOLDER, FileMode.CreateNew);
        private static readonly StreamWriter streamWriter = new StreamWriter(_stream);

        int _count;
        List<int> _paths = new();

        public void Add(decimal key, string value)
        {
            _count++;
            if (_count == 1_000_000)
            {
                // Save to disc
                _paths.Add(SaveToDisc());

                // delete key-value pairs memory
                base.Clear();
            }

            base[key] = value;
        }

        public string Get(decimal key)
        {
            // Lookup on disc


            return base[key];
        }

        public bool ContainsKeyCompleteLookup(decimal key)
        {
            // Lookup on disc

            return base.ContainsKey(key);
        }

        private int SaveToDisc()
        {
            int ID = GetHashCode() + _paths.Count;
            var data = base.ToArray().Select(ToByteArray);

            streamWriter.Write(data);

            return ID;
        }

        private void LoadFromDisc()
        {
            // use mmf

            foreach (var path in _paths)
            {
                using (var mmf = MemoryMappedFile.OpenExisting("..."))
                {

                }
            }
        }

        private byte[] ToByteArray(KeyValuePair<decimal, string> values)
        {
            byte[] decData = Decimal
                .GetBits(values.Key)
                .SelectMany(BitConverter.GetBytes)
                .ToArray();

            byte[] strData = values.Value
                .ToArray()
                .SelectMany(BitConverter.GetBytes)
                .ToArray();

            var data = new byte[PAIR_SIZE];
            Array.Copy(decData, data, DEC_SIZE);
            Array.Copy(strData, 0, data, DEC_SIZE, STR_SIZE);

            return data;
        }
    }

    internal class DecimalTesting
    {
        public static void Main()
        {

            FileDictionary nums = new();

            Parallel.For(Int32.MinValue, Int32.MaxValue, i =>
            {
                for (int j = Int32.MinValue; j <= Int32.MaxValue; j++)
                    for (int k = Int32.MinValue; k <= Int32.MaxValue; k++)
                    {
                        int[] bits =
                        {
                        i, j, k, 0
                    };

                        var d = new Decimal(bits);
                        var s = d.ToString();

                        if (nums.ContainsKeyCompleteLookup(d) && nums.Get(d) == s)
                        {
                            Console.WriteLine("Duplicate found: " + s);
                            Console.WriteLine(bits[0]);
                            Console.WriteLine(bits[1]);
                            Console.WriteLine(bits[2]);
                            Console.WriteLine(bits[3]);
                        }

                        nums.Add(d, s);

                        if (d % 1_000_000 == 0)
                        {
                            Console.WriteLine("State: " + d);
                        }
                    }
            });
        }
    }
}
