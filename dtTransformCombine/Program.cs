using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace dtTransformCombine
{
    class Program
    {
        /// <summary>
        /// Number of lines after which we should flush the <see cref="StringBuilder"/> to the output.
        /// </summary>
        private const int FlushLineCount = 1000;

        /// <summary>
        /// Output buffer.
        /// </summary>
        private static readonly StringBuilder StringBuilder = new StringBuilder();

        /// <summary>
        /// Number of lines currently in the <see cref="StringBuilder"/>.
        /// </summary>
        private static int _lineCount;

        /// <summary>
        /// Maximum length of the combined lines to output.
        /// </summary>
        private static int _maxLength;

        /// <summary>
        /// Length of the shortest line in the input set.
        /// </summary>
        private static int _shortestLineLength;

        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                var exeName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.WriteLine("Usage: {0} <max_length>", exeName);
                Console.WriteLine();
                Console.Error.WriteLine("You must give a maximum length.");
                return 1;
            }

            _maxLength = Convert.ToInt16(args[0]);

            try
            {
                // Load all items.
                var items = GetItems();

                // Sort them by line length,
                // and remove empty lines.
                items = items.Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x.Length).ToList();

                // Include the empty item.
                Console.WriteLine();

                // Generate all possible combinations.
                if (items.Any())
                {
                    _shortestLineLength = items.First().Length;
                    for (int i = 1; i <= _maxLength/_shortestLineLength; ++i)
                        GenerateItems(items, i);
                }

                FlushBuffer();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }

            return 0;
        }

        private static List<string> GetItems()
        {
            // Keep a list of all items.
            var items = new List<string>();
            while (true)
            {
                string line = Console.ReadLine();
                if (line == null)
                    break;

                items.Add(line);
            }
            return items;
        }

        private static void GenerateItems(List<string> items, int numberOfElements, string prepend = "")
        {
            Debug.Assert(numberOfElements > 0);
            if (numberOfElements > 1)
            {
                foreach (string item in items)
                {
                    var nextPrepend = prepend + item;
                    if (nextPrepend.Length + _shortestLineLength > _maxLength)
                        // There can be no other item less than this length.
                        break;
                    GenerateItems(items, numberOfElements - 1, nextPrepend);
                }
            }
            else
            {
                foreach (string item in items)
                {
                    string line = prepend + item;
                    if (line.Length > _maxLength)
                        // There can be no other item less than this length.
                        break;
                    StringBuilder.AppendLine(line);
                    ++_lineCount;
                }

                // Periodically flush the output.
                if (_lineCount > FlushLineCount)
                {
                    FlushBuffer();
                }
            }
        }

        private static void FlushBuffer()
        {
            Console.Write(StringBuilder.ToString());
            StringBuilder.Clear();
            _lineCount = 0;
        }
    }
}
