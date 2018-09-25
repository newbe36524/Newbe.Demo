using Autofac;
using System;

namespace Use_Dependency_Injection_With_Factory_Pattern
{
    public static class Demo1
    {
        public static void Run()
        {
            Console.WriteLine($"开始运行{nameof(Demo1)}");
            var cb = new ContainerBuilder();
            cb.RegisterType<UserDal>().As<IUserDal>();
            cb.RegisterType<UserBll>().As<IUserBll>();
            cb.RegisterType<ConsoleSmsSender>().As<ISmsSender>();
            var container = cb.Build();

            var userBll = container.Resolve<IUserBll>();
            var login = userBll.Login("yueluo", "newbe");
            Console.WriteLine(login);

            login = userBll.Login("newbe", "yueluo");
            Console.WriteLine(login);
            Console.WriteLine($"结束运行{nameof(Demo1)}");
        }

        public class UserBll : IUserBll
        {
            private readonly IUserDal _userDal;
            private readonly ISmsSender _smsSender;

            public UserBll(
                IUserDal userDal,
                ISmsSender smsSender)
            {
                _userDal = userDal;
                _smsSender = smsSender;
            }

            public bool Login(string phone, string password)
            {
                var re = _userDal.Exists(phone, password);
                if (re)
                {
                    _smsSender.Send(phone, "您已成功登录系统");
                }

                return re;
            }
        }

        public class ConsoleSmsSender : ISmsSender
        {
            public void Send(string phone, string message)
            {
                Console.WriteLine($"已给{phone}发送消息：{message}");
            }
        }
    }
}
