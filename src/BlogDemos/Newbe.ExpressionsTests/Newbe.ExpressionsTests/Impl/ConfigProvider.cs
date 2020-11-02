namespace Newbe.ExpressionsTests
{
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