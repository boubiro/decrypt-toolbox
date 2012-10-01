using System;
using System.Collections.Generic;
using System.Text;

namespace dtTransformAlternatives
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                while (true)
                {
                    string line = Console.ReadLine();
                    if (line == null)
                        return 0;

                    // Now print all possible combinations.
                    if (line == "")
                        Console.WriteLine("");
                    else
                    {
                        var sb = new StringBuilder();
                        Generate("", line, 0, sb);
                        Console.Write(sb.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return 1;
            }
        }

        private static void Generate(string start, string line, int i, StringBuilder stringBuilder)
        {
            int next = i + 1;
            if (next < line.Length)
                foreach (char ch in SimpleAccentAlternatives(line[i]))
                {
                    Generate(start + ch, line, next, stringBuilder);
                }
            else
                foreach (char ch in SimpleAccentAlternatives(line[i]))
                {
                    stringBuilder.AppendLine(start + ch);
                }
        }

        static IEnumerable<char> SimpleAccentAlternatives(char ch)
        {
            foreach (char c in Alternatives(ch))
                yield return c;

            switch (char.ToLower(ch))
            {
                case 'á':
                case 'à':
                case 'â':
                case 'ä':
                    foreach (char c in Alternatives('a'))
                        yield return c;
                    break;
                case 'é':
                case 'è':
                case 'ê':
                case 'ë':
                    foreach (char c in Alternatives('e'))
                        yield return c;
                    break;
                case 'í':
                case 'ì':
                case 'î':
                case 'ï':
                    foreach (char c in Alternatives('e'))
                        yield return c;
                    break;
                case 'ó':
                case 'ò':
                case 'ô':
                case 'ö':
                    foreach (char c in Alternatives('o'))
                        yield return c;
                    break;
                case 'ú':
                case 'ù':
                case 'û':
                case 'ü':
                case 'µ':
                    foreach (char c in Alternatives('u'))
                        yield return c;
                    break;
                case 'ß':
                    foreach (char c in Alternatives('s'))
                        yield return c;
                    break;
                case 'ç':
                    foreach (char c in Alternatives('c'))
                        yield return c;
                    break;
                case 'ñ':
                    foreach (char c in Alternatives('n'))
                        yield return c;
                    break;
            }
        }

        static IEnumerable<char> Alternatives(char ch)
        {
            yield return char.ToLower(ch);
            var upper = char.ToUpper(ch);
            yield return upper;

            switch (upper)
            {
                case 'O':
                    yield return '0';
                    break;
                case 'I':
                    yield return '1';
                    break;
                case 'Z':
                case 'R':
                    yield return '2';
                    break;
                case 'T':
                case 'L':
                    yield return '1';
                    yield return '7';
                    break;
                case 'E':
                    yield return '3';
                    break;
                case 'A':
                    yield return '4';
                    break;
                case 'S':
                    yield return '5';
                    yield return '2';
                    break;
                case 'G':
                    yield return '6';
                    break;
                case 'B':
                    yield return '8';
                    break;
                case 'H':
                    yield return '#';
                    break;
            }
        }
    }
}
