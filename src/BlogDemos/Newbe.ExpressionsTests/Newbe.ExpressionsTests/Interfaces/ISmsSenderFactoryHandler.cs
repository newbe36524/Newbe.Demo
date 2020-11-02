namespace Newbe.ExpressionsTests
{
    /// <summary>
    /// 工厂方法接口
    /// </summary>
    public interface ISmsSenderFactoryHandler
    {
        SmsSenderType SmsSenderType { get; }
        ISmsSender Create();
    }
}