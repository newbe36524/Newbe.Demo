using System;
using Newbe.ExpressionsTests.Interfaces;

namespace Newbe.ExpressionsTests
{
    /// <summary>
    /// 调试·短信API
    /// </summary>
    public class ConsoleSmsSender : ISmsSender
    {
        public delegate ConsoleSmsSender Factory();

        private readonly IMyLogger _myLogger;

        public ConsoleSmsSender(IMyLogger myLogger)
        {
            _myLogger = myLogger;
        }


        public void Send(string phone, string message)
        {
            Console.WriteLine($"已给{phone}发送消息：{message}");
        }
    }
}