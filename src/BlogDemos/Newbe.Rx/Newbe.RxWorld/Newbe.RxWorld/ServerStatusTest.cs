using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.RxWorld
{
    public class ServerStatusTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ServerStatusTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ServerStatusChangeTest()
        {
            var dataBaseStatus = new Subject<bool>();
            var redisStatus = new Subject<bool>();

            var serverStatus = dataBaseStatus
                .CombineLatest(redisStatus, (db, redis) => db && redis)
                .DistinctUntilChanged()
                .Subscribe(status => { _testOutputHelper.WriteLine($"server is health : {status}"); });

            // status change to true
            dataBaseStatus.OnNext(true);
            redisStatus.OnNext(true);
            
            // status change to false
            redisStatus.OnNext(false);
            
            // status still false
            dataBaseStatus.OnNext(true);
            dataBaseStatus.OnNext(false);
            dataBaseStatus.OnNext(true);
            
            // status change to true
            dataBaseStatus.OnNext(true);
            redisStatus.OnNext(true);

            serverStatus.Dispose();
        }
    }
}