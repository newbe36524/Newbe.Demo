using Autofac;
using System;
using System.Threading;

namespace Use_Dependency_Injection_With_Lifetime_Scope_Control
{
    public static class Demo2
    {
        public static void Run()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<StaticClockByOneTime>()
                .As<IClock>()
                .SingleInstance();
            var container = cb.Build();
            var clock = container.Resolve<IClock>();
            Console.WriteLine($"第一次获取时间：{clock.Now}");
            Thread.Sleep(1000);
            clock = container.Resolve<IClock>();
            Console.WriteLine($"第二次获取时间：{clock.Now}");
            Thread.Sleep(1000);
            clock = container.Resolve<IClock>();
            Console.WriteLine($"第三次获取时间：{clock.Now}");
        }

        public interface IClock
        {
            /// <summary>
            /// 获取当前系统时间
            /// </summary>
            DateTime Now { get; }
        }

        public class StaticClockByOneTime : IClock
        {
            private DateTime _firstTime = DateTime.MinValue;
            public DateTime Now
            {
                get
                {
                    if (_firstTime == DateTime.MinValue)
                    {
                        _firstTime = DateTime.Now;
                    }

                    return _firstTime;
                }
            }
        }
    }
}
