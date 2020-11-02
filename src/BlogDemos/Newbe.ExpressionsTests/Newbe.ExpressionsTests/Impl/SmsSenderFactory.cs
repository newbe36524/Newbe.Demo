using System.Linq;

namespace Newbe.ExpressionsTests
{
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
}