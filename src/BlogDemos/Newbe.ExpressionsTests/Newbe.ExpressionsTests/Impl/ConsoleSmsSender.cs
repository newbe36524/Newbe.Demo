using System;

namespace Newbe.ExpressionsTests
{
    /// <summary>
    /// 调试·短信API
    /// </summary>
    public class ConsoleSmsSender : ISmsSender
    {
        public void Send(string phone, string message)
        {
            Console.WriteLine($"已给{phone}发送消息：{message}");
        }
    }
}