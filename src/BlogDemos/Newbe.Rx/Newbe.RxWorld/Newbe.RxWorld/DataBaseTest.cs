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
    public class DataBaseTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DataBaseTest(
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