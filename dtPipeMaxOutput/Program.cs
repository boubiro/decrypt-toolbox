using System;
using System.Text;

namespace dtPipeMaxOutput
{
    class Program
    {
        static void Main(string[] args)
        {
            var sb = new StringBuilder();
            
            for (int i = 0; i < 1000; ++i)
                sb.AppendLine("AAAAAAAA");
            
            var data = sb.ToString();
            
            while (true)
            {
                Console.Write(data);
            }
        }
    }
}
