using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.RxWorld
{
    public class EventSourcingTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EventSourcingTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void BalanceChangeTest()
        {
            var account = new AccountState();

            var accountSeq = Observable.Return(account);
            var balanceChangeEventSeq = new Subject<BalanceChangeEvent>();

            var disposable = accountSeq
                .CombineLatest(balanceChangeEventSeq, (state, @event) => new
                {
                    state, @event
                })
                .Scan(account, (acc, newState) =>
                {
                    newState.state.Balance = newState.state.Balance + newState.@event.BalanceDiff;
                    return newState.state;
                })
                .Do(state => _testOutputHelper.WriteLine($"balance changed! now : {state.Balance}"))
                .Subscribe(acc => { _testOutputHelper.WriteLine($"balance now is {acc.Balance}"); });
            
            balanceChangeEventSeq.OnNext(new BalanceChangeEvent(+100));
            balanceChangeEventSeq.OnNext(new BalanceChangeEvent(+100));
            balanceChangeEventSeq.OnNext(new BalanceChangeEvent(-50));

            disposable.Dispose();

            account.Balance.Should().Be(150);
        }

        public class AccountState
        {
            public decimal Balance { get; set; }
        }

        public class BalanceChangeEvent
        {
            public BalanceChangeEvent(decimal balanceDiff)
            {
                BalanceDiff = balanceDiff;
            }

            public decimal BalanceDiff { get; set; }
        }
    }
}