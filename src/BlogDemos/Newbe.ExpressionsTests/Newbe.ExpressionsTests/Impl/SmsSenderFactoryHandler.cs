namespace Newbe.ExpressionsTests
{
    public class SmsSenderFactoryHandler : ISmsSenderFactoryHandler
    {
        public SmsSenderType SmsSenderType { get; } = SmsSenderType.HttpAPi;

        public ISmsSender Create()
        {
            return new HttpApiSmsSender();
        }
    }
}