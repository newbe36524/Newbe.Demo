namespace Newbe.ExpressionsTests
{
    /// <summary>
    /// 用户业务
    /// </summary>
    public interface IUserBll
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        bool Login(string phone, string password);
    }
}