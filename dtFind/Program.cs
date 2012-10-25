using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace dtFind
{
    class Program
    {
        private static readonly Dictionary<string, int> ValidInputs = new Dictionary<string, int>
                {
                    {"MD5", 32},
                    {"SHA-1", 40},
                    {"SHA-128", 128},
                    {"SHA-384", 384},
                    {"SHA-256", 256},
                };

        static int Main(string[] args)
        {
            // Process command-line arguments.
            if (args.Length != 1)
            {
                var exeName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.WriteLine("Usage: {0} <string>", exeName);
                Console.WriteLine();
                return 1;
            }

            string search = args[0];
            Console.WriteLine("Searching for any stdin string starting by '{0}'...", search);

            // Check the argument seems valid.
            string errorMessage = CheckStringIsHash(search);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.Error.WriteLine("WARNING: Possible invalid argument:");
                Console.Error.WriteLine(errorMessage);
                Console.Error.WriteLine();
                foreach (KeyValuePair<string, int> validInput in ValidInputs)
                {
                    Console.Error.WriteLine("  {0} - {1} lower case hexadacimal characters.", validInput.Key, validInput.Value);
                }
                Console.Error.WriteLine();
            }

            // Compare inputs.
            var matches = new List<string>();
            DateTime startTime = DateTime.Now;
            DateTime lastOutputTime = startTime;
            int millionLinesProcessed = 0;
            int linesProcessed = 0;
            int lastProgressLineLength = 0;
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null)
                    break;

                if (++linesProcessed > 1000000)
                {
                    ++millionLinesProcessed;
                    linesProcessed -= 1000000;

                    // Periodically display progress update.
                    var timeSpan = DateTime.Now - lastOutputTime;
                    if (timeSpan.TotalSeconds > 5.0)
                    {
                        lastOutputTime = DateTime.Now;

                        Console.Write("\rProcessed {0} M lines in {1}, last compared: {2}",
                                        millionLinesProcessed,
                                        (DateTime.Now - startTime).ToReadableString(),
                                        line.PadRight(lastProgressLineLength));

                        lastProgressLineLength = line.Length;
                    }
                }

                // Compare against input string.
                if (string.Compare(line, 0, search, 0, search.Length) == 0)
                {
                    // Match found.
                    matches.Add(line);

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Match found (after {0} M lines and {1}):",
                                      millionLinesProcessed,
                                      (DateTime.Now - startTime).ToReadableString());
                    Console.WriteLine(line);
                    Console.WriteLine();
                }
            }

            // Display summary.
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Processed {0} M lines in {1}.", millionLinesProcessed, (DateTime.Now - startTime).ToReadableString());
            if (matches.Any())
            {
                Console.WriteLine("Found {0} matches found starting by '{1}':", matches.Count, search);
                foreach (string match in matches)
                {
                    Console.WriteLine(match);
                }
            }
            else
            {
                Console.WriteLine("No match found starting by '{0}'.", search);
            }
            return 0;
        }

        private static string CheckStringIsHash(string search)
        {
            if (!Regex.IsMatch(search, @"^[0-9a-fA-F]*$"))
            {
                return "dtHash hashed values only hexadecimal characters [0-9a-z].";
            }

            if (!ValidInputs.Values.Contains(search.Length))
            {
                return
                    "The input string length doesn't match any of the known encryption algorithm hashed values length.";
            }

            if (search.Any(char.IsUpper))
            {
                return "dtHash hashed values are usually in lower case.";
            }

            return null;
        }
    }
}
