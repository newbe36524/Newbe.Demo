using System.Collections.Generic;
using System.Linq;

namespace Newbe.ExpressionsTests
{
    public class SmsSenderFactory : ISmsSenderFactory
    {
        private readonly IConfigProvider _configProvider;
        private readonly ISmsSenderFactoryHandler[] _smsSenderFactoryHandlers;
        private readonly IMyIndex<SmsSenderType, ISmsSenderFactoryHandler> _indexedHandlers;

        public SmsSenderFactory(
            IConfigProvider configProvider,
            ISmsSenderFactoryHandler[] smsSenderFactoryHandlers,
            IEnumerable<ISmsSenderFactoryHandler> handlers,
            List<ISmsSenderFactoryHandler> handlers1,
            HashSet<ISmsSenderFactoryHandler> set,
            IMyIndex<SmsSenderType, ISmsSenderFactoryHandler> indexedHandlers)
        {
            _configProvider = configProvider;
            _smsSenderFactoryHandlers = smsSenderFactoryHandlers;
            _indexedHandlers = indexedHandlers;
        }

        public ISmsSender Create()
        {
            // 短信发送者创建，从配置管理中读取当前的发送方式，并创建实例
            var smsConfig = _configProvider.GetSmsConfig();
            // 通过工厂方法的方式，将如何创建具体短信发送者的逻辑从这里移走，实现了这个方法本身的稳定。
            var factoryHandler = _indexedHandlers[smsConfig.SmsSenderType];
            var smsSender = factoryHandler.Create();
            return smsSender;
        }
    }
}