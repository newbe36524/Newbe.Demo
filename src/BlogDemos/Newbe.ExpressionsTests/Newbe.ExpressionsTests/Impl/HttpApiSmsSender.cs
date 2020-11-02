using System;

namespace Newbe.ExpressionsTests
{
    /// <summary>
    /// 真·短信API
    /// </summary>
    public class HttpApiSmsSender : ISmsSender
    {
        public void Send(string phone, string message)
        {
            Console.WriteLine($"已调用API给{phone}发送消息：{message}");
        }
    }
}