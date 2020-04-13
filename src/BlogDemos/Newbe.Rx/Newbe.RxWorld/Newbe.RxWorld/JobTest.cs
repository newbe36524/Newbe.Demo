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
            IJob job = new Job(timeSpan, () => _testOutputHelper.WriteLine($"now : {DateTime.Now}"));
            Thread.Sleep(TimeSpan.FromSeconds(3));
            job.RunManually();
            job.RunManually();
            job.RunManually();
        }

        [Fact]
        public void Concurrent()
        {
            var timeSpan = TimeSpan.FromMilliseconds(500);
            _ = new Job(timeSpan, () =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                _testOutputHelper.WriteLine($"now : {DateTime.Now}");
            });
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }

        private interface IJob
        {
            void RunManually();
        }

        private class Job : IJob
        {
            private readonly Action _handler;

            public Job(
                TimeSpan timeSpan,
                Action handler)
            {
                _handler = handler;
                var observable = Observable.Interval(timeSpan);
                observable.Subscribe(l => _handler());
            }

            public void RunManually()
            {
                _handler();
            }
        }
    }
}