using System;
using System.Collections.Generic;
using System.Text;

namespace dtTransformAlternatives
{
    class Program
    {
        private const int FlushLineCount = 1000;
        private static readonly StringBuilder StringBuilder = new StringBuilder();
        private static int _lineCount = 0;

        static int Main(string[] args)
        {
            try
            {
                while (true)
                {
                    string line = Console.ReadLine();
                    if (line == null)
                        break;

                    // Now print all possible combinations.
                    if (line == "")
                    {
                        StringBuilder.AppendLine();
                        ++_lineCount;
                    }
                    else
                    {
                        Generate("", line, 0);
                    }

                    if (_lineCount > FlushLineCount)
                    {
                        FlushBuffer();
                    }
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

        private static void Generate(string start, string line, int i)
        {
            int next = i + 1;
            if (next < line.Length)
            {
                foreach (char ch in SimpleAccentAlternatives(line[i]))
                {
                    Generate(start + ch, line, next);
                }

                if (_lineCount > FlushLineCount)
                {
                    FlushBuffer();
                }
            }
            else
            {
                foreach (char ch in SimpleAccentAlternatives(line[i]))
                {
                    StringBuilder.AppendLine(start + ch);
                    ++_lineCount;
                }
            }
        }

        private static void FlushBuffer()
        {
            Console.Write(StringBuilder.ToString());
            StringBuilder.Clear();
            _lineCount = 0;
        }

        static IEnumerable<char> SimpleAccentAlternatives(char ch)
        {
            if (!char.IsLetter(ch))
            {
                yield return ch;
                yield break;
            }

            char upperCaseLetter = char.IsUpper(ch) ? ch : char.ToUpper(ch);

            foreach (char c in AlternativesLetterToDigits(upperCaseLetter))
                yield return c;

            switch (upperCaseLetter)
            {
                case 'Á':
                case 'À':
                case 'Â':
                case 'Ä':
                    foreach (char c in AlternativesLetterToDigits('A'))
                        yield return c;
                    break;
                case 'É':
                case 'È':
                case 'Ê':
                case 'Ë':
                    foreach (char c in AlternativesLetterToDigits('E'))
                        yield return c;
                    break;
                case 'Í':
                case 'Ì':
                case 'Î':
                case 'Ï':
                    foreach (char c in AlternativesLetterToDigits('E'))
                        yield return c;
                    break;
                case 'Ó':
                case 'Ò':
                case 'Ô':
                case 'Ö':
                    foreach (char c in AlternativesLetterToDigits('O'))
                        yield return c;
                    break;
                case 'Ú':
                case 'Ù':
                case 'Û':
                case 'Ü':
                case 'Μ':
                    foreach (char c in AlternativesLetterToDigits('U'))
                        yield return c;
                    break;
                case 'ß':
                    foreach (char c in AlternativesLetterToDigits('S'))
                        yield return c;
                    break;
                case 'Ç':
                    foreach (char c in AlternativesLetterToDigits('C'))
                        yield return c;
                    break;
                case 'Ñ':
                    foreach (char c in AlternativesLetterToDigits('N'))
                        yield return c;
                    break;
            }
        }

        static IEnumerable<char> AlternativesLetterToDigits(char upperCaseLetter)
        {
            yield return char.ToLower(upperCaseLetter);
            yield return upperCaseLetter;

            switch (upperCaseLetter)
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
