using System.Collections.Concurrent;

namespace Newbe.LongRunningJob;

public class NamedThreadTests
{
    [Test]
    public void ShowThreadMessage()
    {
        new Thread(() => { ShowCurrentThread("Custom thread work"); })
        {
            IsBackground = true,
            Name = "Custom thread"
        }.Start();

        Task.Run(() => { ShowCurrentThread("Task.Run work"); });
        Task.Factory.StartNew(() => { ShowCurrentThread("Task.Factory.StartNew work"); },
            TaskCreationOptions.LongRunning);

        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

    [Test]
    public void ShortThread()
    {
        new Thread(async () =>
        {
            ShowCurrentThread("before await");
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            ShowCurrentThread("after await");
        })
        {
            IsBackground = true,
            Name = "Custom thread"
        }.Start();
        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

    [Test]
    public void ThreadWaitTask()
    {
        new Thread(async () =>
        {
            ShowCurrentThread("before await");
            Task.Run(() => { ShowCurrentThread("inner task"); }).Wait();
            ShowCurrentThread("after await");
        })
        {
            IsBackground = true,
            Name = "Custom thread"
        }.Start();
        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

    [Test]
    public void AlwaysLogRunning()
    {
        new Thread(async () =>
        {
            ShowCurrentThread("before await");
            Task.Factory.StartNew(() =>
            {
                ShowCurrentThread("LongRunning task");
                Task.Run(() => { ShowCurrentThread("inner task"); }).Wait();
            }, TaskCreationOptions.LongRunning).Wait();
            ShowCurrentThread("after await");
        })
        {
            IsBackground = true,
            Name = "Custom thread"
        }.Start();
        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

    [Test]
    public void TestThread()
    {
        new Thread(() =>
        {
            ShowCurrentThread("async in thread");
            Task.Factory.StartNew(() =>
            {
                ShowCurrentThread("thread before Task.Run");
                Task.Run(() => { ShowCurrentThread("inner task"); }).Wait();
                ShowCurrentThread("thread after Task.Run");
            }, TaskCreationOptions.LongRunning).Wait();
            ShowCurrentThread("thread after async");
        })
        {
            IsBackground = true,
            Name = "TestThread"
        }.Start();

        Thread.Sleep(TimeSpan.FromSeconds(1));
    }

    [Test]
    public async Task TestLongRunning()
    {
        await Task.Factory.StartNew(() =>
        {
            ShowCurrentThread("BeforeWait");
            RunSomethingAsync().Wait();
            ShowCurrentThread("AfterWait");
        }, TaskCreationOptions.LongRunning);
    }

    [Test]
    public async Task TestLongRunningConfigureAwait()
    {
        var scheduler = new MyScheduler();
        await Task.Factory.StartNew(() =>
        {
            ShowCurrentThread("BeforeWait");
            Task.Factory
                .StartNew(() =>
                    {
                        ShowCurrentThread("AfterWait");
                    }
                    , CancellationToken.None, TaskCreationOptions.None, scheduler)
                .Wait();
            ShowCurrentThread("AfterWait");
        }, CancellationToken.None, TaskCreationOptions.None, scheduler);
    }

    private async Task RunSomethingAsync()
    {
        ShowCurrentThread("RunSomethingAsync");
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    private static void ShowCurrentThread(string work)
    {
        Console.WriteLine($"{work} - {Thread.CurrentThread.Name} - {Thread.CurrentThread.ManagedThreadId}");
    }
}

class MyScheduler : TaskScheduler
{
    private readonly Thread _thread;
    private readonly ConcurrentQueue<Task> _tasks = new();

    public MyScheduler()
    {
        _thread = new Thread(() =>
        {
            while (true)
            {
                while (_tasks.TryDequeue(out var task))
                {
                    TryExecuteTask(task);
                }
            }
        })
        {
            IsBackground = true,
            Name = "MyScheduler"
        };
        _thread.Start();
    }

    protected override IEnumerable<Task> GetScheduledTasks()
    {
        return _tasks;
    }

    protected override void QueueTask(Task task)
    {
        _tasks.Enqueue(task);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return false;
    }
}