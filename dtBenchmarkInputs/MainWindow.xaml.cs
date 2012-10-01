using System;
using System.Text;
using System.Threading;
using System.Windows;

namespace dtBenchmark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Benchmark another process output speed?
            var thread = new Thread(StartOutputBenchmark) {IsBackground = true};
            thread.Start();
        }

        private void StartOutputBenchmark()
        {
            // Build an output string.
            const int outLineCount = 10000;
            string outData = GenerateSampleOutput(outLineCount);

            // Output them as fast as possible.
            DateTime startTime = DateTime.Now;
            int count = 0;

            while (true)
            {
                Console.Write(outData);
                count += outLineCount;

                DateTime endTime = DateTime.Now;
                TimeSpan timeSpan = endTime - startTime;
                if (timeSpan.TotalSeconds > 2.0)
                {
                    var performance = count / (int)timeSpan.TotalMilliseconds;
                    Outputs.Dispatcher.BeginInvoke(
                        (ThreadStart) delegate { Outputs.Text = string.Format("{0:D} K writes/second", performance); });
                    
                    count = 0;
                    startTime = endTime;
                }
            }
        }

        private static string GenerateSampleOutput(int lineCount)
        {
            var sb = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < lineCount; ++i)
            {
                for (int j = 0; j < random.Next(4, 12); j++)
                {
                    var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*random.NextDouble() + 65)));
                    sb.Append(ch);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}