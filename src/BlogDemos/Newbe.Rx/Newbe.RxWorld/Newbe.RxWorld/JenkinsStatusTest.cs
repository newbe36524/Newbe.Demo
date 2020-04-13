using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Xunit;
using System;
using Xunit.Abstractions;

namespace Newbe.RxWorld
{
    public class JenkinsStatusTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public JenkinsStatusTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetJenkinsStatusTest()
        {
            var jenkinsMonitor = new Subject<string>();
            jenkinsMonitor
                .Select(url => new
                {
                    status = Observable.FromAsync(() => GetJenkinsStatus(url)),
                    url = url
                })
                .DistinctUntilChanged()
                .Subscribe(item =>
                {
                    item.status
                        .FirstAsync()
                        .Subscribe(status => { SendEmail(item.url, status); });
                });

            jenkinsMonitor.OnNext("https://www.newbe.pro");
            jenkinsMonitor.OnNext("https://www.newbe.pro?id=666");
            jenkinsMonitor.OnNext("https://www.newbe.pro");
            jenkinsMonitor.OnNext("https://www.newbe.pro");
        }

        private Task<bool> GetJenkinsStatus(string url)
        {
            // test code
            if (url.EndsWith("666"))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        private Task SendEmail(string url, bool status)
        {
            _testOutputHelper.WriteLine($"current jenkins status is {status}. url :{url}");
            return Task.CompletedTask;
        }
    }
}