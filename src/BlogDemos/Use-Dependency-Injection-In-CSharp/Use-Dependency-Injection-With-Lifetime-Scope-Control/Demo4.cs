using Autofac;
using System;
using System.Collections.Generic;
using System.Data;

namespace Use_Dependency_Injection_With_Lifetime_Scope_Control
{
    public static class Demo4
    {
        public static void Run()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<AccountBll>().As<IAccountBll>();
            cb.RegisterType<AccountDal>().As<IAccountDal>();
            cb.RegisterType<DbFactory>().As<IDbFactory>();
            cb.RegisterType<ConsoleDbConnection>().AsSelf();
            var container = cb.Build();

            using (var beginLifetimeScope = container.BeginLifetimeScope())
            {
                var accountBll = beginLifetimeScope.Resolve<IAccountBll>();
                accountBll.Transfer("yueluo", "newbe", 333);
            }
        }

        public interface IDbFactory
        {
            IExecuteSqlDbConnection CreateDbConnection();
        }

        public class DbFactory : IDbFactory
        {
            private readonly ConsoleDbConnection.Factory _factory;

            public DbFactory(
                ConsoleDbConnection.Factory factory)
            {
                this._factory = factory;
            }

            public IExecuteSqlDbConnection CreateDbConnection()
            {
                return _factory();
            }
        }

        public interface IAccountBll
        {
            void Transfer(string fromAccountId, string toAccountId, decimal amount);
        }

        public class AccountBll : IAccountBll
        {
            private readonly IDbFactory _dbFactory;
            private readonly IAccountDal _accountDal;

            public AccountBll(
                IDbFactory dbFactory,
                IAccountDal accountDal)
            {
                _dbFactory = dbFactory;
                _accountDal = accountDal;
            }

            public void Transfer(string fromAccountId, string toAccountId, decimal amount)
            {
                using (var dbConnection = _dbFactory.CreateDbConnection())
                {
                    using (var transaction = dbConnection.BeginTransaction())
                    {
                        try
                        {
                            var fromAmount = _accountDal.GetBalance(fromAccountId);
                            var toAmount = _accountDal.GetBalance(toAccountId);
                            fromAmount -= amount;
                            toAmount += amount;
                            _accountDal.UpdateBalance(fromAccountId, fromAmount, dbConnection, transaction);
                            _accountDal.UpdateBalance(toAccountId, toAmount, dbConnection, transaction);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
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
            /// <param name="dbConnection"></param>
            /// <param name="dbTransaction"></param>
            void UpdateBalance(string id, decimal balance, IExecuteSqlDbConnection dbConnection = null, IDbTransaction dbTransaction = null);
        }

        public class AccountDal : IAccountDal
        {
            private readonly IDbFactory _dbFactory;

            public AccountDal(
                IDbFactory dbFactory)
            {
                _dbFactory = dbFactory;
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

            public void UpdateBalance(string id, decimal balance, IExecuteSqlDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
            {
                if (dbConnection == null)
                {
                    dbConnection = _dbFactory.CreateDbConnection();
                    dbConnection.ExecuteSql("更新语句：更新 {0} 余额为 {1}", new object[] { id, balance });
                    _accounts[id] = balance;

                }
                else
                {
                    dbConnection.ExecuteSql("更新语句：更新 {0} 余额为 {1}", new object[] { id, balance }, dbTransaction);
                    _accounts[id] = balance;
                }

            }
        }
    }
}
