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
    public class UnitTest1
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UnitTest1(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Test1()
        {
            var httpClient = new HttpClient();
            var source = new[]
            {
                1, 2, 3, 4, 5, 6, 7
            };
            var ob = source.ToObservable();
            var task = ob
                .Select(x => x * 2)
                .Where(x => x > 10)
                .ForEachAsync(async x =>
                {
                    var httpResponseMessage = await httpClient.SendAsync(new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"https://www.baidu.com?q={x}")
                    });
                    _testOutputHelper.WriteLine(httpResponseMessage.StatusCode.ToString());
                });
            await task;
        }

        [Fact]
        public void Test2()
        {
            var observable = Observable.Generate(0, x => true, i => i + 1, i => i, i => TimeSpan.FromSeconds(0.5));
            using (observable.Subscribe(x => _testOutputHelper.WriteLine(x.ToString())))
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}