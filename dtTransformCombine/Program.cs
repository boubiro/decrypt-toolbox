using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace dtTransformCombine
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                // Load all items.
                var items = GetItems();

                // Generate all combination.
                GenerateCombinations(items);
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
            Debug.Assert(numberOfElements >= 0);
            if (numberOfElements > 0)
            {
                //var subset = new HashSet<string>(items);
                foreach (Item item in items.Where(x => x.Enabled = true))
                {
                    item.Enabled = false;
                    GenerateItems(items, numberOfElements - 1, prepend + item);
                    item.Enabled = true;
                }
            }
            else
            {
                var sb = new StringBuilder();
                foreach (Item item in items.Where(x => x.Enabled = true))
                    sb.AppendLine(prepend + item.Line);
                Console.Write(sb.ToString());
            }
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
