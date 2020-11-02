namespace Newbe.ExpressionsTests
{
    public class ConsoleSmsSenderFactoryHandler : ISmsSenderFactoryHandler
    {
        public SmsSenderType SmsSenderType { get; } = SmsSenderType.Console;

        public ISmsSender Create()
        {
            return new ConsoleSmsSender();
        }
    }
}