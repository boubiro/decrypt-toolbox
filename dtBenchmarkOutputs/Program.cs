using System;

namespace dtBenchmarkOutputs
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime fullStartTime = DateTime.Now;
            int fullCount = 0;

            DateTime startTime = fullStartTime;
            int count = 0;
            while (Console.ReadLine() != null)
            {
                if ((++count % 10000) == 0)
                {
                    DateTime endTime = DateTime.Now;
                    var dt = endTime - startTime;
                    if (dt.TotalSeconds > 2.0)
                    {
                        Console.WriteLine("{0:D} K rows/second", count / (int)dt.TotalMilliseconds);
                        fullCount += count;

                        count = 0;
                        startTime = endTime;
                    }
                }
            }
            fullCount += count;

            Console.WriteLine();
            Console.WriteLine("Average: {0:D} K rows/second", fullCount / (int)(DateTime.Now - fullStartTime).TotalMilliseconds);
        }
    }
}
