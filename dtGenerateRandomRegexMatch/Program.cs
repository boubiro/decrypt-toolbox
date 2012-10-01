using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Fare;

namespace dtGenerateRandomRegexMatch
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                var exeName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
                Console.WriteLine("SYNTAX: {0} <regex>", exeName);
                Console.Error.WriteLine("This program takes a single argument: The regular expression.");
                return 1;
            }

            var regex = args[0];

            try
            {
                // Empty match (just in case).
                Console.WriteLine();

                // Produce regex matches.
                Parallel.For(0, Environment.ProcessorCount, i => Produce(regex));

                // Should never return.
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unhandled Exception:");
                Console.Error.WriteLine(ex);
                return 1;
            }
        }

        private static void Produce(string regex)
        {
            var xeger = new Xeger(regex);
            while (true)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < 1000; ++i)
                    sb.AppendLine(xeger.Generate());
                Console.Write(sb.ToString());
            }
        }
    }
}