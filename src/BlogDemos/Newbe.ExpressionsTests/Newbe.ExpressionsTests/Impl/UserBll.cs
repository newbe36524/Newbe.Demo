namespace Newbe.ExpressionsTests
{
    public class UserBll : IUserBll
    {
        private readonly IUserDal _userDal;
        private readonly ISmsSenderFactory _smsSenderFactory;

        public UserBll(
            IUserDal userDal,
            ISmsSenderFactory smsSenderFactory)
        {
            _userDal = userDal;
            _smsSenderFactory = smsSenderFactory;
        }

        public bool Login(string phone, string password)
        {
            var re = _userDal.Exists(phone, password);
            if (re)
            {
                var smsSender = _smsSenderFactory.Create();
                smsSender.Send(phone, "您已成功登录系统");
            }

            return re;
        }
    }
}