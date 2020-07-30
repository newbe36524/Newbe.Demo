using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Newbe.Tasks.ConsoleTest1
{
    class Program
    {
        /**
         * this console will exit if you create a file at d:/1.txt
         */
        static async Task Main(string[] args)
        {
            var tcs = new TaskCompletionSource<DateTime>();

            // start a task to check file at d:/1.txt
#pragma warning disable 4014
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    if (File.Exists(Path.Combine("d:/", "1.txt")))
                    {
                        Console.WriteLine("file found!");
                        tcs.SetResult(DateTime.Now);
                        break;
                    }

                    Console.WriteLine($"{DateTime.Now:s} : file not found");
                }
            });
#pragma warning restore 4014

            Console.WriteLine("start to await task.");
            await tcs.Task;
            Console.WriteLine("console exit.");
        }
    }
}