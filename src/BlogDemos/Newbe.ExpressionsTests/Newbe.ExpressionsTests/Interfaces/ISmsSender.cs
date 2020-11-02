namespace Newbe.ExpressionsTests
{
    /// <summary>
    /// 发送短信
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone">手机</param>
        /// <param name="message">短信  </param>
        void Send(string phone, string message);
    }
}