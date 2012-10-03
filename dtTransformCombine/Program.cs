using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace dtTransformCombine
{
    class Program
    {
        private const int FlushLineCount = 1000;
        private static readonly StringBuilder StringBuilder = new StringBuilder();
        private static int _lineCount = 0;
        private static int _maxLength;

        static int Main(string[] args)
        {
            if (args.Length >= 1)
            {
                _maxLength = Convert.ToInt16(args[0]);
            }
            else
            {
                _maxLength = 0;
            }

            try
            {
                // Load all items.
                var items = GetItems();

                // Generate all combination.
                GenerateCombinations(items);

                FlushBuffer();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }

            return 0;
        }

        private static List<Item> GetItems()
        {
            // Keep a list of all items.
            var items = new List<Item>();
            while (true)
            {
                string line = Console.ReadLine();
                if (line == null)
                    break;

                items.Add(new Item(line));
            }
            return items;
        }

        private static void GenerateCombinations(List<Item> items)
        {
            // Include the empty item.
            Console.WriteLine();

            // Generate all possible combinations.
            for (int i = 0; i < items.Count; ++i)
                GenerateItems(items, i);
        }

        private static void GenerateItems(List<Item> items, int numberOfElements, string prepend = "")
        {
            if (_maxLength > 0 && prepend.Length > _maxLength)
                return;

            Debug.Assert(numberOfElements >= 0);
            if (numberOfElements > 0)
            {
                foreach (Item item in items.Where(x => x.Enabled = true))
                {
                    item.Enabled = false;
                    GenerateItems(items, numberOfElements - 1, prepend + item);
                    item.Enabled = true;
                }
            }
            else
            {
                foreach (Item item in items.Where(x => x.Enabled = true))
                {
                    string line = prepend + item.Line;
                    if (_maxLength <= 0 || line.Length <= _maxLength)
                    {
                        StringBuilder.AppendLine(line);
                        ++_lineCount;
                    }
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

        class Item : ICloneable
        {
            public Item(string line)
            {
                Enabled = true;
                Line = line;
            }

            public bool Enabled { get; set; }

            public string Line { get; private set; }

            #region Implementation of ICloneable

            public object Clone()
            {
                return new Item(Line)
                    {
                        Enabled = Enabled,
                    };
            }

            #endregion
        }
    }
}
