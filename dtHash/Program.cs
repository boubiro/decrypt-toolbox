using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace dtHash
{
    class Program
    {
        private static readonly string[] HexStringTable = new string[]
            {
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
                "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
                "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
                "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
                "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
                "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
                "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
                "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
                "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
                "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
                "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
                "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF"
            };

        static int Main(string[] args)
        {
            var algos = new List<IHashAlgorithm>
                            {
                                new Md5HashAlgorithm(),
                                new Sha1HashAlgorithm(),
                                new Sha256HashAlgorithm(),
                                new Sha384HashAlgorithm(),
                                new Sha512HashAlgorithm(),
                            };

            if (args.Length != 1)
            {
                var exeName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.WriteLine("USAGE: <DICTIONARY_FILE.txt {0} <algo>", exeName);
                Console.WriteLine();
                Console.WriteLine("Algorithms supported:");
                foreach (var hashAlgorithm in algos)
                {
                    Console.WriteLine("  - {0} - {1} hexadecimal characters.", hashAlgorithm.HashedValuesLength, hashAlgorithm.Name);
                }
                Console.Error.WriteLine("Invalid number of arguments.");
                return 1;
            }

            // Get the algo.
            IHashAlgorithm algo = algos.FirstOrDefault(x => string.Compare(args[0], x.Name, true) == 0);
            if (algo == null)
            {
                Console.Error.WriteLine("Invalid algorithm name '{0}', must be one of: {1}", args[0], string.Join(", ", algos.Select(x => x.Name)));
                return 1;
            }

            // Run the program.
            try
            {
                Decrypt(algo);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unhandled Exception:");
                Console.Error.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        private static void Decrypt(IHashAlgorithm algo)
        {
            // IProducerConsumerCollection
            using (var producerConsumerCollection = new BlockingCollection<string>())
            {
                // Consumer.
                var tasks = new List<Task>();
                for (int workingThread = 0; workingThread < Environment.ProcessorCount; workingThread++)
                {
                    tasks.Add(Task.Factory.StartNew(() => HashThread(algo, producerConsumerCollection)));
                }

                // Producer.
                while (true)
                {
                    var line = Console.ReadLine();
                    if (line == null)
                    {
                        producerConsumerCollection.CompleteAdding();
                        break;
                    }
                    producerConsumerCollection.Add(line);
                }

                // Wait until processing is done.
                foreach (Task task in tasks)
                {
                    task.Wait();
                }
            }
        }

        private static void HashThread(IHashAlgorithm algo, BlockingCollection<string> producerConsumerCollection)
        {
            var hashAlgo = algo.CreateHashAlgorithm();

            var taskOutputs = new StringBuilder();
            int count = 0;

            while (!producerConsumerCollection.IsCompleted)
            {
                string line;
                try
                {
                    line = producerConsumerCollection.Take();
                }
                catch (InvalidOperationException)
                {
                    if (producerConsumerCollection.IsCompleted)
                        break;
                    throw;
                }

                // Hash that line.
                byte[] inputBytes = Encoding.ASCII.GetBytes(line);
                byte[] hash = hashAlgo.ComputeHash(inputBytes);

                // Convert to hexadecimal.
                foreach (byte b in hash)
                {
                    taskOutputs.Append(HexStringTable[b]);
                }
                taskOutputs.Append(' ');
                taskOutputs.AppendLine(line);

                // Output the hashed values in a batch.
                if (++count > 10000)
                {
                    Console.Write(taskOutputs.ToString());
                    taskOutputs.Clear();
                    count = 0;
                }
            }

            // Output the last hashed values.
            Console.Write(taskOutputs.ToString());
        }
    }
}
