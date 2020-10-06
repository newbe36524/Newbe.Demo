using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Newbe.DotTrace.Tests
{
    public class X04SleepTest
    {
        [Test]
        public Task TaskDelay()
        {
            return Task.Delay(TimeSpan.FromSeconds(3));
        }

        [Test]
        public Task ThreadSleep()
        {
            return Task.Run(() => { Thread.Sleep(TimeSpan.FromSeconds(3)); });
        }

        [Test]
        public Task RunManyDelay()
        {
            return Task.WhenAny(GetTasks(50));

            IEnumerable<Task> GetTasks(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    var i1 = i;
                    yield return Task.Run(() =>
                    {
                        Console.WriteLine($"Task {i1}");
                        Thread.Sleep(int.MaxValue);
                    });
                }

                yield return Task.Run(() => { Console.WriteLine("yueluo is the only one dalao"); });
            }
        }
    }
}