using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Newbe.DotTrace.Tests
{
    public class X02ClassAndStruct
    {
        [Test]
        public void UsingClass()
        {
            Console.WriteLine($"memory in bytes before execution: {GC.GetGCMemoryInfo().TotalAvailableMemoryBytes}");
            const int count = 1_000_000;
            var list = new List<Student>(count);
            for (var i = 0; i < count; i++)
            {
                list.Add(new Student
                {
                    Level = int.MinValue
                });
            }

            list.Clear();

            var gcMemoryInfo = GC.GetGCMemoryInfo();
            Console.WriteLine($"heap size: {gcMemoryInfo.HeapSizeBytes}");
            Console.WriteLine($"memory in bytes end of execution: {gcMemoryInfo.TotalAvailableMemoryBytes}");
        }

        [Test]
        public void UsingStruct()
        {
            Console.WriteLine($"memory in bytes before execution: {GC.GetGCMemoryInfo().TotalAvailableMemoryBytes}");
            const int count = 1_000_000;
            var list = new List<Yueluo>(count);
            for (var i = 0; i < count; i++)
            {
                list.Add(new Yueluo
                {
                    Level = int.MinValue
                });
            }

            list.Clear();

            var gcMemoryInfo = GC.GetGCMemoryInfo();
            Console.WriteLine($"heap size: {gcMemoryInfo.HeapSizeBytes}");
            Console.WriteLine($"memory in bytes end of execution: {gcMemoryInfo.TotalAvailableMemoryBytes}");
        }

        public class Student
        {
            public int Level { get; set; }
        }

        public struct Yueluo
        {
            public int Level { get; set; }
        }
    }
}