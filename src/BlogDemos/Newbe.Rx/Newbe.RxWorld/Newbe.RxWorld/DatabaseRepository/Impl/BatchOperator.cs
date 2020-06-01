using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public class BatchOperator<TInput, TOutput> : IBatchOperator<TInput, TOutput>
    {
        private readonly Subject<SavingItem> _subject = new Subject<SavingItem>();

        public BatchOperator(
            BatchOperatorOptions<TInput, TOutput> options)
        {
            if (options.BufferTime == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.BufferCount == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _subject
                .Buffer(options.BufferTime.Value, options.BufferCount.Value)
                .Where(x => x.Count > 0)
                .Select(x => Observable.FromAsync(async () =>
                {
                    try
                    {
                        var result = await options.DoManyFunc.Invoke(x.Select(a => a.Input)).ConfigureAwait(false);
                        foreach (var savingItem in x)
                        {
                            savingItem.Tcs.SetResult(result);
                        }
                    }
                    catch (Exception e)
                    {
                        foreach (var savingItem in x)
                        {
                            savingItem.Tcs.SetException(e);
                        }
                    }
                }))
                .Merge()
                .Subscribe();
        }

        public Task<TOutput> CreateTask(TInput input)
        {
            var savingItem = new SavingItem
            {
                Tcs = new TaskCompletionSource<TOutput>(),
                Input = input
            };
            _subject.OnNext(savingItem);
            return savingItem.Tcs.Task;
        }

        private struct SavingItem
        {
            public TInput Input { get; set; }
            public TaskCompletionSource<TOutput> Tcs { get; set; }
        }
    }
}