using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Humanizer;
using JetBrains.dotMemoryUnit;
using Microsoft.Toolkit.HighPerformance.Buffers;
using NUnit.Framework;

namespace Newbe.StringPools
{
    public class ListStringPoolTest
    {
        [Test]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public async Task LoadFromDbAsync()
        {
            var beforeStart = dotMemory.Check();
            var dict = new Dictionary<int, ProductInfo>(HelperTest.ProductCount);
            await DbReadingTest.LoadCoreAsync(dict);
            foreach (var (_, p) in dict)
            {
                var colorIndex = HelperTest.Colors.BinarySearch(p.Color);
                var color = HelperTest.Colors[colorIndex];
                p.Color = color;
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