using Autofac;
using System;
using System.Collections.Generic;

namespace Use_Dependency_Injection_With_Lifetime_Scope_Control
{
    public static class Demo3
    {
        public static void Run()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<AccountBll>().As<IAccountBll>();
            cb.RegisterType<AccountDal>().As<IAccountDal>();
            cb.RegisterType<ConsoleLogger>().As<ILogger>()
                .InstancePerLifetimeScope();
            var container = cb.Build();

            using (var beginLifetimeScope = container.BeginLifetimeScope())
            {
                var accountBll = beginLifetimeScope.Resolve<IAccountBll>();
                accountBll.Transfer("yueluo", "newbe", 333);
                accountBll.Transfer("yueluo", "newbe", 333);
            }
        }

        public interface ILogger
        {
            void BeginScope(string scopeTag);
            void Log(string message);
        }

        public class ConsoleLogger : ILogger
        {
            private string _currenctScopeTag;

            public void BeginScope(string scopeTag)
            {
                _currenctScopeTag = scopeTag;
            }

            public void Log(string message)
            {
                Console.WriteLine(string.IsNullOrEmpty(_currenctScopeTag)
                    ? $"输出日志：{message}"
                    : $"输出日志：{message}[scope:{_currenctScopeTag}]");
            }
        }

        public interface IAccountBll
        {
            /// <summary>
            /// 转账
            /// </summary>
            /// <param name="fromAccountId">来源账号Id</param>
            /// <param name="toAccountId">目标账号Id</param>
            /// <param name="amount">转账数额</param>
            void Transfer(string fromAccountId, string toAccountId, decimal amount);
        }

        public class AccountBll : IAccountBll
        {
            private readonly ILogger _logger;
            private readonly IAccountDal _accountDal;

            public AccountBll(
                ILogger logger,
                IAccountDal accountDal)
            {
                _logger = logger;
                _accountDal = accountDal;
            }

            public void Transfer(string fromAccountId, string toAccountId, decimal amount)
            {
                _logger.BeginScope(Guid.NewGuid().ToString());
                var fromAmount = _accountDal.GetBalance(fromAccountId);
                var toAmount = _accountDal.GetBalance(toAccountId);
                fromAmount -= amount;
                toAmount += amount;
                _accountDal.UpdateBalance(fromAccountId, fromAmount);
                _accountDal.UpdateBalance(toAccountId, toAmount);
            }
        }

        public interface IAccountDal
        {
            /// <summary>
            /// 获取账户的余额
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            decimal GetBalance(string id);

            /// <summary>
            /// 更新账户的余额
            /// </summary>
            /// <param name="id"></param>
            /// <param name="balance"></param>
            void UpdateBalance(string id, decimal balance);
        }

        public class AccountDal : IAccountDal
        {
            private readonly ILogger _logger;

            public AccountDal(
                ILogger logger)
            {
                _logger = logger;
            }

            private readonly Dictionary<string, decimal> _accounts = new Dictionary<string, decimal>
            {
                {"newbe",1000},
                {"yueluo",666},
            };

            public decimal GetBalance(string id)
            {
                return _accounts.TryGetValue(id, out var balance) ? balance : 0;
            }

            public void UpdateBalance(string id, decimal balance)
            {
                _logger.Log($"更新了 {id} 的余额为 {balance}");
                _accounts[id] = balance;
            }
        }
    }
}
