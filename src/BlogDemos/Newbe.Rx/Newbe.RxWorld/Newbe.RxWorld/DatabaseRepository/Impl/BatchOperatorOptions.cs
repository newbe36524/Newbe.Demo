using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository.Impl
{
    public class BatchOperatorOptions<TInput, TOutput>
    {
        public TimeSpan? BufferTime { get; set; } = TimeSpan.FromMilliseconds(20);
        public int? BufferCount { get; set; } = 1000;
        public Func<IEnumerable<TInput>, Task<TOutput>> DoManyFunc { get; set; } = null!;
    }
}