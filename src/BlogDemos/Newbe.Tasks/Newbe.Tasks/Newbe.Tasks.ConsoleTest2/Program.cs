using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Newbe.Tasks.ConsoleTest2
{
    class Program
    {
        /**
         * this console will exit if you create a file at d:/1.txt
         */
        static async Task Main(string[] args)
        {
            var tasks = Enumerable.Range(0, 10)
                .Select(RunTask);

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
                        var items = new List<TaskItem<int>>();
                        while (TaskQueue.TryDequeue(out var item))
                        {
                            items.Add(item);
                        }

                        Console.WriteLine($"items completed : {string.Join(",", items.Select(x => x.Input))}");
                        foreach (var taskItem in items)
                        {
                            taskItem.Tcs.SetResult(true);
                        }

                        break;
                    }

                    Console.WriteLine($"{DateTime.Now:s} : file not found");
                }
            });
#pragma warning restore 4014

            await Task.WhenAll(tasks);
        }

        private static readonly ConcurrentQueue<TaskItem<int>> TaskQueue
            = new ConcurrentQueue<TaskItem<int>>();

        private static Task<bool> RunTask(int data)
        {
            var tcs = new TaskCompletionSource<bool>();
            TaskQueue.Enqueue(new TaskItem<int>
            {
                Input = data,
                Tcs = tcs
            });
            return tcs.Task;
        }

        public struct TaskItem<T>
        {
            public T Input { get; set; }
            public TaskCompletionSource<bool> Tcs { get; set; }
        }
    }
}