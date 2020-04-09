using System;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.RxWorld
{
    public class TimeTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TimeTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var observable = Observable.Generate(0, x => true, i => i + 1, i => i, i => TimeSpan.FromSeconds(0.5));
            using (observable.Subscribe(x => _testOutputHelper.WriteLine(x.ToString())))
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}