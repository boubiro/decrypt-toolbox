using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace dtDecrypt
{
    class Program
    {
        private static readonly string[] HexStringTable = new[]
            {
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0a", "0b", "0c", "0d", "0e", "0f",
                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1a", "1b", "1c", "1d", "1e", "1f",
                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2a", "2b", "2c", "2d", "2e", "2f",
                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3a", "3b", "3c", "3d", "3e", "3f",
                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4a", "4b", "4c", "4d", "4e", "4f",
                "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5a", "5b", "5c", "5d", "5e", "5f",
                "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6a", "6b", "6c", "6d", "6e", "6f",
                "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7a", "7b", "7c", "7d", "7e", "7f",
                "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8a", "8b", "8c", "8d", "8e", "8f",
                "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9a", "9b", "9c", "9d", "9e", "9f",
                "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af",
                "b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf",
                "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf",
                "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "da", "db", "dc", "dd", "de", "df",
                "e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9", "ea", "eb", "ec", "ed", "ee", "ef",
                "f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "fa", "fb", "fc", "fd", "fe", "ff"
            };

        static int Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                var exeName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.WriteLine("USAGE: <KEYS_FILE.txt {0} <encrypted_file> <algo> [true for all_padding]", exeName);
                Console.WriteLine();
                Console.WriteLine("Symmetric key algorithms:");
                Console.WriteLine("    AES, Blowfish, Camellia, CAST5, CAST6, DESede, DES, GOST28147, HC-128,");
                Console.WriteLine("    HC-256, IDEA, NaccacheStern, RC2, RC4, RC5-32, RC5-64, RC6, Rijndael,");
                Console.WriteLine("    Serpent, Skipjack, TEA/XTEA, Twofish, and VMPC");
                Console.WriteLine();
                Console.WriteLine("Symmetric key paddings:");
                Console.WriteLine("    ISO10126d2, ISO7816d4, PKCS#5/7, TBC, X.923, and Zero Byte.");
                if (args.Length > 0)
                    Console.Error.WriteLine("Invalid number of arguments.");
                return 1;
            }

            Func<IBufferedCipher> algo;
            string encryptedFileName;
            bool allPaddings;
            try
            {
                // Get the encrypted file.
                encryptedFileName = args[0];
                if (!File.Exists(encryptedFileName))
                {
                    Console.Error.WriteLine("Encrypted file not found: " + encryptedFileName);
                }

                // Get the algo.
                algo = () => CipherUtilities.GetCipher(args[1]);
                if (algo() == null)
                {
                    Console.Error.WriteLine("Invalid algorithm name '{0}'.", args[1]);
                    return 1;
                }

                // Generate outputs for all paddings?
                allPaddings = (args.Length >= 3 && Convert.ToBoolean(args[2]));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to process command line: " + e.Message);
                return 1;
            }

            // Run the program.
            try
            {
                Decrypt(algo, encryptedFileName, allPaddings);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unhandled Exception:");
                Console.Error.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        private static void Decrypt(Func<IBufferedCipher> engine, string encryptedFileName, bool allPaddings)
        {
            // Load the encrypted file.
            var encrpted = File.ReadAllBytes(encryptedFileName);

            // IProducerConsumerCollection
            using (var producerConsumerCollection = new BlockingCollection<string>(50000))
            {
                // Consumer.
                var tasks = new List<Task>();
                for (int workingThread = 0; workingThread < Environment.ProcessorCount; workingThread++)
                {
                    tasks.Add(Task.Factory.StartNew(() => DecryptThread(engine(), encrpted, producerConsumerCollection, allPaddings)));
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

        private static void DecryptThread(IBufferedCipher cipher, byte[] input, BlockingCollection<string> producerConsumerCollection, bool allPaddings = false)
        {
            var taskOutputs = new StringBuilder();
            int lineCount = 0;

            IBlockCipherPadding[] paddings;

            if (allPaddings)
            {
                paddings = new IBlockCipherPadding[]
                               {
                                   new ZeroBytePadding(),
                                   new ISO10126d2Padding(),
                                   new ISO7816d4Padding(),
                                   new Pkcs7Padding(),
                                   new TbcPadding(),
                                   new X923Padding(),
                                   new X923Padding(),
                               };
            }
            else
            {
                paddings = new IBlockCipherPadding[] {new Pkcs7Padding()};
            }

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

                // Use that line as the key.
                byte[] key = Encoding.ASCII.GetBytes(line);

                // For each possible padding...
                foreach (IBlockCipherPadding padding in paddings)
                {
                    // Decrypt
                    byte[] output;
                    try
                    {
                        cipher.Init(false, new KeyParameter(key));
                        output = cipher.DoFinal(input);
                        cipher.Reset();
                    }
                    catch (InvalidCipherTextException)
                    {
                        // TODO: Possibly filder out those inputs before and/or tell the user not to generate them.
                        continue;
                    }

                    // Convert to hexadecimal.
                    foreach (byte b in output)
                    {
                        taskOutputs.Append(HexStringTable[b]);
                    }
                    taskOutputs.AppendFormat(" `{0}` [{1}]", line, padding.PaddingName);
                    taskOutputs.AppendLine();
                    ++lineCount;
                }

                // Output the hashed values in a batch.
                if (taskOutputs.Length > 100000)
                {
                    Console.Write(taskOutputs.ToString());
                    taskOutputs.Clear();
                    lineCount = 0;
                }
            }

            // Output the last hashed values.
            Console.Write(taskOutputs.ToString());
        }
    }
}
