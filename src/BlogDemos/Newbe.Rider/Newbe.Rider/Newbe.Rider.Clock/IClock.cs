using System;

namespace Newbe.Rider.Clock
{
    public interface IClock
    {
        DateTime Now { get; }
    }

    class FuncClock : IClock
    {
        private readonly Func<DateTime> _func;

        public FuncClock(
            Func<DateTime> func)
        {
            _func = func;
        }

        public DateTime Now => _func();
    }

    class SystemClock : IClock
    {
        public DateTime Now => DateTime.Now;
    }
}