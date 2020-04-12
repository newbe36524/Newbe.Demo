using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public class AllObservableDatabaseRepository : IDatabaseRepository
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IDatabase _database;
        private readonly Subject<BatchItem> _subject;

        public AllObservableDatabaseRepository(
            ITestOutputHelper testOutputHelper,
            IDatabase database)
        {
            _testOutputHelper = testOutputHelper;
            _database = database;
            _subject = CreateSubject();
        }

        private Subject<BatchItem> CreateSubject()
        {
            var subject = new Subject<BatchItem>();
            subject.Buffer(TimeSpan.FromMilliseconds(50), 100)
                .Where(x => x.Count > 0)
                .Select(items => new
                {
                    insertResult = Observable.FromAsync(() => _database.InsertMany(items.Select(x => x.Item))),
                    tasks = items.Select(x => x.TaskCompletionSource)
                })
                .Subscribe(ob =>
                {
                    var tasks = ob.tasks.ToObservable();
                    ob.insertResult
                        .Subscribe(insertResult =>
                        {
                            _testOutputHelper.WriteLine($"{ob.tasks.Count()} items data inserted");
                            tasks.Subscribe(x => x.SetResult(insertResult));
                        }, ex =>
                        {
                            _testOutputHelper.WriteLine($"there is an error when data insertion, exception : {ex}");
                            tasks.Subscribe(x => x.SetException(ex));
                        });
                });
            return subject;
        }

        public Task<int> InsertData(int item)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            _subject.OnNext(new BatchItem
            {
                Item = item,
                TaskCompletionSource = taskCompletionSource
            });
            return taskCompletionSource.Task;
        }

        private struct BatchItem
        {
            public TaskCompletionSource<int> TaskCompletionSource { get; set; }
            public int Item { get; set; }
        }
    }
}