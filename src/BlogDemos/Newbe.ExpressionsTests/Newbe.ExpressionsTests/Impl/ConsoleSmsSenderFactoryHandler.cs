namespace Newbe.ExpressionsTests
{
    public class ConsoleSmsSenderFactoryHandler : ISmsSenderFactoryHandler
    {
        private readonly ConsoleSmsSender.Factory _factory;

        public ConsoleSmsSenderFactoryHandler(
            ConsoleSmsSender.Factory factory)
        {
            _factory = factory;
        }

        public SmsSenderType SmsSenderType { get; } = SmsSenderType.Console;

        public ISmsSender Create()
        {
            return _factory.Invoke();
            // return new ConsoleSmsSender(default);
        }
    }
}