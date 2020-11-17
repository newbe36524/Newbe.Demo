using Newbe.ExpressionsTests.Interfaces;

namespace Newbe.ExpressionsTests.Impl
{
    public class MyLogger : IMyLogger
    {
    }

    public class MyConsoleLogger : IMyLogger
    {
        private readonly MyLogger _logger;

        public MyConsoleLogger(
            MyLogger logger)
        {
            _logger = logger;
        }
    }
}