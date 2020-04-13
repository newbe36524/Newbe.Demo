using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Tasks
{
    public class TaskCompletionSourceTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TaskCompletionSourceTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task SuccessTest()
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            await Task.WhenAll(AllTasks());

            IEnumerable<Task> AllTasks()
            {
                // task 1
                yield return RunATask();

                // task 2
                yield return Task.Run(async () =>
                {
                    _testOutputHelper.WriteLine("start task 1");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    var task2Result = 666;
                    _testOutputHelper.WriteLine($"set result to task 2 with result {task2Result}");
                    taskCompletionSource.SetResult(task2Result);
                    _testOutputHelper.WriteLine("task2 result set");
                });

                async Task RunATask()
                {
                    // waiting for inner task result
                    var result = await taskCompletionSource.Task;
                    _testOutputHelper.WriteLine($"task2 return value is {result}");
                }
            }
        }

        [Fact]
        public async Task ExceptionTest()
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            var exceptionMessage = "月老板是唯一的大佬";
            try
            {
                await Task.WhenAll(AllTasks());
            }
            catch (Exception e) when (e.Message == exceptionMessage)
            {
                _testOutputHelper.WriteLine("there is an exception from task1");
            }

            IEnumerable<Task> AllTasks()
            {
                // task 1
                yield return RunATask();

                // task 2
                yield return Task.Run(async () =>
                {
                    _testOutputHelper.WriteLine("start task 1");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    var exception = new Exception(exceptionMessage);
                    _testOutputHelper.WriteLine($"set result to task 2 with Exception {exception}");
                    taskCompletionSource.SetException(exception);
                    _testOutputHelper.WriteLine("task2 exception set");
                });

                async Task RunATask()
                {
                    // waiting for inner task result
                    var result = await taskCompletionSource.Task;
                    _testOutputHelper.WriteLine($"task2 return value is {result}");
                }
            }
        }

        [Fact]
        public async Task CancellationTest()
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            var exceptionMessage = "月老板是唯一的大佬";
            try
            {
                await Task.WhenAll(AllTasks());
            }
            catch (TaskCanceledException)
            {
                _testOutputHelper.WriteLine("task2 is canceled");
            }

            IEnumerable<Task> AllTasks()
            {
                // task 1
                yield return RunATask();

                // task 2
                yield return Task.Run(async () =>
                {
                    _testOutputHelper.WriteLine("start task 1");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    var exception = new Exception(exceptionMessage);
                    _testOutputHelper.WriteLine($"set task2 to canceled");
                    taskCompletionSource.SetCanceled();
                    _testOutputHelper.WriteLine("task2 canceled");
                });

                async Task RunATask()
                {
                    // waiting for inner task result
                    var result = await taskCompletionSource.Task;
                    _testOutputHelper.WriteLine($"task2 return value is {result}");
                }
            }
        }
    }
}