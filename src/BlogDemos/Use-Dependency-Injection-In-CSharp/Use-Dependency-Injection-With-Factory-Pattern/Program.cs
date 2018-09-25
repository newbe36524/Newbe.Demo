namespace Use_Dependency_Injection_With_Factory_Pattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Demo1.Run();
            Demo2.Run();
            Demo3.Run();
            Demo4.Run();
            Demo5.Run();
        }
    }

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

    public interface IUserDal
    {
        bool Exists(string phone, string password);
    }

    public class UserDal : IUserDal
    {
        private const string StaticPassword = "newbe";
        private const string StaticPhone = "yueluo";

        public bool Exists(string phone, string password)
        {
            // 使用固定的账号密码验证
            return phone == StaticPhone && password == StaticPassword;
        }
    }
}
