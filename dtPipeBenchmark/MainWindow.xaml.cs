using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace dtPipeBenchmark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Check if we're benchmarking inputs or outputs.
            bool inputs;
            try
            {
                Console.In.Peek();
                inputs = true;
            }
            catch (IOException)
            {
                inputs = false;
            }

            // Benchmark another process output speed?
            Thread thread;
            if (inputs)
            {
                thread = new Thread(StartInputBenchmark);
            }
            else
            {
                thread = new Thread(StartOutputBenchmark);
            }
            thread.IsBackground = true;
            thread.Start();
        }

        private void StartInputBenchmark()
        {
            DateTime startTime = DateTime.Now;
            int count = 0;

            while (true)
            {
                for (int i = 0; i < 100000; ++i)
                {
                    string readLine = Console.ReadLine();
                    if (readLine == null)
                        return;
                    ++count;
                }

                var performance = (int) (count/(1000.0*(DateTime.Now - startTime).TotalSeconds) + 0.5);
                Inputs.Dispatcher.BeginInvoke(
                    (ThreadStart) delegate { Inputs.Text = string.Format("{0} K lines/second", performance); });
            }
        }

        private void StartOutputBenchmark()
        {
            DateTime startTime = DateTime.Now;
            int count = 0;

            while (true)
            {
                for (int i = 0; i < 100000; ++i)
                {
                    Console.WriteLine("abc0123456789ABC");
                    ++count;
                }

                var performance = (int) (count/(1000.0*(DateTime.Now - startTime).TotalSeconds) + 0.5);
                Outputs.Dispatcher.BeginInvoke(
                    (ThreadStart) delegate { Outputs.Text = string.Format("{0} K writes/second", performance); });
            }
        }
    }
}