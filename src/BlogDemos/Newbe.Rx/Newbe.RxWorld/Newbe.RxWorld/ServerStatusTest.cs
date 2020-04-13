using System;
using System.Collections.Generic;
using System.Linq;
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

            var serverStatus =
                dataBaseStatus
                    .CombineLatest(redisStatus, CombineServerStatus)
                    .DistinctUntilChanged()
                    .Subscribe(ShowCurrentServerStatus);

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

            static bool CombineServerStatus(bool dbStatus, bool redisStatus)
                => dbStatus && redisStatus;

            void ShowCurrentServerStatus(bool status)
                => _testOutputHelper.WriteLine($"server is health : {status}");
        }

        [Fact]
        public void MultipleComponentStatus()
        {
            var dataBaseStatus = new Subject<bool>();
            var redisStatus = new Subject<bool>();
            var elasticSearchStatus = new Subject<bool>();
            elasticSearchStatus.OnNext(true);

            var allComponentStatus = new IObservable<bool>[]
            {
                dataBaseStatus, redisStatus, elasticSearchStatus
            };

            var serverStatus =
                allComponentStatus
                    .Aggregate(CombineAllComponentStatus)
                    .DistinctUntilChanged()
                    .Subscribe(ShowCurrentServerStatus);

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

            static IObservable<bool>
                CombineAllComponentStatus(IObservable<bool> statusOb1, IObservable<bool> statusOb2) =>
                statusOb1.CombineLatest(statusOb2, CombineServerStatus);

            static bool CombineServerStatus(bool status1, bool status2)
                => status1 && status2;

            void ShowCurrentServerStatus(bool status)
                => _testOutputHelper.WriteLine($"server is health : {status}");
        }
    }
}