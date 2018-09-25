using Autofac;
using System;
using System.Linq;

namespace Use_Dependency_Injection_With_Factory_Pattern
{
    public static class Demo4
    {
        public static void Run()
        {
            Console.WriteLine($"开始运行{nameof(Demo4)}");
            var cb = new ContainerBuilder();
            cb.RegisterType<UserDal>().As<IUserDal>();
            cb.RegisterType<UserBll>().As<IUserBll>();
            cb.RegisterType<SmsSenderFactory>().As<ISmsSenderFactory>();
            cb.RegisterType<ConsoleSmsSenderFactoryHandler>()
                .As<ISmsSenderFactoryHandler>();
            cb.RegisterType<SmsSenderFactoryHandler>()
                .As<ISmsSenderFactoryHandler>();
            cb.RegisterType<ConfigProvider>().As<IConfigProvider>();
            var container = cb.Build();

            var userBll = container.Resolve<IUserBll>();
            var login = userBll.Login("yueluo", "newbe");
            Console.WriteLine(login);

            login = userBll.Login("newbe", "yueluo");
            Console.WriteLine(login);
            Console.WriteLine($"结束运行{nameof(Demo4)}");
        }

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

        public enum SmsSenderType
        {
            /// <summary>
            /// 控制台发送短信
            /// </summary>
            Console,

            /// <summary>
            /// 通过HttpApi进行发送短信
            /// </summary>
            HttpAPi
        }

        public interface ISmsSenderFactory
        {
            ISmsSender Create();
        }

        public class SmsSenderFactory : ISmsSenderFactory
        {

            private readonly IConfigProvider _configProvider;
            private readonly ISmsSenderFactoryHandler[] _smsSenderFactoryHandlers;

            public SmsSenderFactory(
                IConfigProvider configProvider,
                ISmsSenderFactoryHandler[] smsSenderFactoryHandlers)
            {
                _configProvider = configProvider;
                _smsSenderFactoryHandlers = smsSenderFactoryHandlers;
            }

            public ISmsSender Create()
            {
                // 短信发送者创建，从配置管理中读取当前的发送方式，并创建实例
                var smsConfig = _configProvider.GetSmsConfig();
                // 通过工厂方法的方式，将如何创建具体短信发送者的逻辑从这里移走，实现了这个方法本身的稳定。
                var factoryHandler = _smsSenderFactoryHandlers.Single(x => x.SmsSenderType == smsConfig.SmsSenderType);
                var smsSender = factoryHandler.Create();
                return smsSender;
            }
        }

        /// <summary>
        /// 工厂方法接口
        /// </summary>
        public interface ISmsSenderFactoryHandler
        {
            SmsSenderType SmsSenderType { get; }
            ISmsSender Create();
        }

        #region 控制台发送短信接口实现

        public class ConsoleSmsSenderFactoryHandler : ISmsSenderFactoryHandler
        {
            public SmsSenderType SmsSenderType { get; } = SmsSenderType.Console;
            public ISmsSender Create()
            {
                return new ConsoleSmsSender();
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

        #endregion

        #region API发送短信接口实现

        public class SmsSenderFactoryHandler : ISmsSenderFactoryHandler
        {
            public SmsSenderType SmsSenderType { get; } = SmsSenderType.HttpAPi;
            public ISmsSender Create()
            {
                return new HttpApiSmsSender();
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

        #endregion

        public class SmsConfig
        {
            public SmsSenderType SmsSenderType { get; set; }
        }

        public interface IConfigProvider
        {
            SmsConfig GetSmsConfig();
        }

        public class ConfigProvider : IConfigProvider
        {
            private readonly SmsConfig _smsConfig = new SmsConfig
            {
                SmsSenderType = SmsSenderType.Console
            };

            public SmsConfig GetSmsConfig()
            {
                // 此处直接使用了写死的短信发送配置，实际项目中往往是通过配置读取的方式，实现该配置的加载。
                return _smsConfig;
            }
        }
    }
}
