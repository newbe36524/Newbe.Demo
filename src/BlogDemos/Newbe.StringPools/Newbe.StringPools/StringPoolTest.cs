using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Humanizer;
using JetBrains.dotMemoryUnit;
using Microsoft.Toolkit.HighPerformance.Buffers;
using NUnit.Framework;

namespace Newbe.StringPools
{
    public class StringPoolTest
    {
        [Test]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public async Task LoadFromDbAsync()
        {
            var beforeStart = dotMemory.Check();
            var dict = new Dictionary<int, ProductInfo>(HelperTest.ProductCount);
            await DbReadingTest.LoadCoreAsync(dict);
            var stringPool = StringPool.Shared;
            foreach (var (_, p) in dict)
            {
                p.Color = stringPool.GetOrAdd(p.Color);
            }
            GC.Collect();
            dotMemory.Check(memory =>
            {
                var snapshotDifference = memory.GetDifference(beforeStart);
                Console.WriteLine(snapshotDifference.GetNewObjects().SizeInBytes.Bytes());
            });
        }

    }
}