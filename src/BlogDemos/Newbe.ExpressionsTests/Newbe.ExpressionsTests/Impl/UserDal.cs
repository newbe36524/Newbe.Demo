namespace Newbe.ExpressionsTests
{
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