using Autofac;
using System;

namespace Use_Dependency_Injection_With_Factory_Pattern
{
    public static class Demo2
    {
        public static void Run()
        {
            Console.WriteLine($"开始运行{nameof(Demo2)}");
            var cb = new ContainerBuilder();
            cb.RegisterType<UserDal>().As<IUserDal>();
            cb.RegisterType<UserBll>().As<IUserBll>();
            // 使用预编译命令，使得 Release 和 Debug 版本注册的对象不同，从而实现调用的短信API不同
#if DEBUG
            cb.RegisterType<ConsoleSmsSender>().As<ISmsSender>();
#else
            cb.RegisterType<HttpApiSmsSender>().As<ISmsSender>();
#endif
            var container = cb.Build();

            var userBll = container.Resolve<IUserBll>();
            var login = userBll.Login("yueluo", "newbe");
            Console.WriteLine(login);

            login = userBll.Login("newbe", "yueluo");
            Console.WriteLine(login);
            Console.WriteLine($"结束运行{nameof(Demo2)}");
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
}
