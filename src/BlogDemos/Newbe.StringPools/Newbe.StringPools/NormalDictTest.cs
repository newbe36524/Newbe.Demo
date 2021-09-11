using System;
using Humanizer;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace Newbe.StringPools
{
    public class NormalDictTest
    {
        [Test]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public void CreateDictTest()
        {
            var beforeStart = dotMemory.Check();
            var dict = HelperTest.CreateDict();
            GC.Collect();
            dotMemory.Check(memory =>
            {
                var snapshotDifference = memory.GetDifference(beforeStart);
                Console.WriteLine(snapshotDifference.GetNewObjects().SizeInBytes.Bytes());
            });
        }
    }
}