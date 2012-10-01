using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace dtCompareHash
{
    class Program
    {
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

            if (args.Length != 2)
            {
                var exeName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.WriteLine("USAGE: <DICTIONARY_FILE.txt {0} <algo> <encrpyted_key>", exeName);
                Console.WriteLine();
                Console.WriteLine("Algorithms supported:");
                foreach (var hashAlgorithm in algos)
                {
                    Console.WriteLine("  - {0}", hashAlgorithm.Name);
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

            // Get the encrypted value.
            string encrypted = args[1].ToLowerInvariant();

            // Check if.
            if (!Regex.IsMatch(encrypted, "[0-9a-f]{" + algo.HashedValuesLength + "}"))
            {
                Console.Error.WriteLine("Invalid hashed values, should be {0} hexadecimal characters.", algo.HashedValuesLength);
                return 1;
            }

            // Run the program.
            try
            {
                Decrypt(algo.Hash, encrypted);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unhandled Exception:");
                Console.Error.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        private static void Decrypt(Func<string, string> hash, string encrypted)
        {
            // IProducerConsumerCollection
            var producerConsumerCollection = new ConcurrentBag<string>();
            bool running = true;

            // Consumer.
            var tasks = new List<Task>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            string line;
                            if (producerConsumerCollection.TryTake(out line))
                            {
#if DEBUG
                                Console.WriteLine("{0} == {1}", hash(line), encrypted);
#endif
                                if (hash(line) == encrypted)
                                {
                                    Console.WriteLine("Match found for: `{0}`.", line);
                                }
                            }
                            else
                            {
                                Thread.Sleep(50);
                                if (!running && producerConsumerCollection.IsEmpty)
                                    break;
                            }
                        }
                    }));
            }

            // Producer.
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null)
                    break;
                producerConsumerCollection.Add(line);
            }

            // Wait until processing is done.
            running = false;
            foreach (Task task in tasks)
            {
                task.Wait();
            }
        }
    }
}
