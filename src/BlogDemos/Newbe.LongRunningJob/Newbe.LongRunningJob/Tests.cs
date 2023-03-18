using System.Runtime.CompilerServices;

namespace Newbe.LongRunningJob;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        _count = 0;
    }

    private int _count = 0;

    [Test]
    public void TestTaskRun_Error()
    {
        ProcessTest(token =>
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    _count++;
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }
            }, token);
        });
        // TestTaskRun_Error: count = 1
    }

    [Test]
    public void TestAsyncTaskLongRunning_Error()
    {
        ProcessTest(token =>
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    _count++;
                    await Task.Delay(TimeSpan.FromSeconds(1), token);
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        });
        // TestAsyncTaskLongRunning_Error: count = 1
    }

    [Test]
    public void TestSyncTaskLongRunning_Success()
    {
        ProcessTest(token =>
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    _count++;
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
        });
        // TestSyncTaskLongRunning_Success: count = 6
    }


    [Test]
    public void TestThread_Success()
    {
        ProcessTest(token =>
        {
            new Thread(() =>
            {
                while (true)
                {
                    _count++;
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            })
            {
                IsBackground = true,
            }.Start();
        });
        // TestThread_Success: count = 6
    }

    [Test]
    public void TestThreadWithTask_Success()
    {
        ProcessTest(token =>
        {
            Task CountUp(CancellationToken c)
            {
                _count++;
                return Task.CompletedTask;
            }

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        CountUp(token).Wait(token);
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                    catch (OperationCanceledException e)
                    {
                        return;
                    }
                }
            })
            {
                IsBackground = true,
            }.Start();
        });
        // TestThreadWithTask_Success: count = 6
    }

    [Test]
    public void TestThreadWithDelayTask_Error()
    {
        ProcessTest(token =>
        {
            Task CountUp(CancellationToken c)
            {
                _count++;
                return Task.Delay(TimeSpan.FromSeconds(1), c);
            }

            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        CountUp(token).Wait(token);
                        token.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException e)
                    {
                        return;
                    }
                }
            })
            {
                IsBackground = true,
            }.Start();
        });
        // TestThreadWithDelayTask_Error: count = 1
    }

    private void ProcessTest(Action<CancellationToken> action, [CallerMemberName] string methodName = "")
    {
        var cts = new CancellationTokenSource();
        action.Invoke(cts.Token);
        YanjiaIsComing(cts.Token);

        Thread.Sleep(TimeSpan.FromSeconds(5));
        cts.Cancel();

        Console.WriteLine($"{methodName}: count = {_count}");
    }

    private void YanjiaIsComing(CancellationToken token)
    {
        Parallel.ForEachAsync(Enumerable.Range(0, 1_000_000), token, (i, c) =>
        {
            while (true)
            {
                // do something
                c.ThrowIfCancellationRequested();
            }
        });
    }
}