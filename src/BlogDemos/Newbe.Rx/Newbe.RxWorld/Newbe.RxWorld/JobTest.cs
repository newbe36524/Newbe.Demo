using System;
using System.Reactive.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.RxWorld
{
    public class JobTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public JobTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void RunManually()
        {
            var timeSpan = TimeSpan.FromMilliseconds(500);
            var handler = new ActionJobHandler(() => _testOutputHelper.WriteLine($"now : {DateTime.Now}"));
            var job = new Job(timeSpan, handler);
            Thread.Sleep(TimeSpan.FromSeconds(3));
            job.RunManually();
            job.RunManually();
            job.RunManually();
        }

        [Fact]
        public void Concurrent()
        {
            var timeSpan = TimeSpan.FromMilliseconds(500);
            var handler = new ActionJobHandler(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                _testOutputHelper.WriteLine($"now : {DateTime.Now}");
            });
            var job = new Job(timeSpan, handler);
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }


        public interface IJob
        {
            void RunManually();
        }

        private class Job : IJob
        {
            private readonly IJobHandler _handler;

            public Job(
                TimeSpan timeSpan,
                IJobHandler handler)
            {
                _handler = handler;
                var observable = Observable.Interval(timeSpan);
                observable.Subscribe(l => _handler.Run());
            }

            public void RunManually()
            {
                _handler.Run();
            }
        }

        public interface IJobHandler
        {
            void Run();
        }

        private class ActionJobHandler : IJobHandler
        {
            private readonly Action _action;

            public ActionJobHandler(
                Action action)
            {
                _action = action;
            }

            public void Run()
            {
                _action();
            }
        }
    }
}