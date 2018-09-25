using System;
using System.Threading;

namespace Use_Dependency_Injection_With_Lifetime_Scope_Control
{
    public static class Demo1
    {
        public static void Run()
        {
            Console.WriteLine($"第一次获取时间：{DateTime.Now}");
            Thread.Sleep(1000);
            Console.WriteLine($"第二次获取时间：{DateTime.Now}");
            Thread.Sleep(1000);
            Console.WriteLine($"第三次获取时间：{DateTime.Now}");
        }
    }
}
