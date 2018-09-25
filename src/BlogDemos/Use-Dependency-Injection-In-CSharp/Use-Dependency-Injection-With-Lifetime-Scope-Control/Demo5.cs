using Autofac;
using System;
using System.Collections.Generic;
using System.Data;

namespace Use_Dependency_Injection_With_Lifetime_Scope_Control
{
    public static class Demo5
    {
        public static void Run()
        {
            var cb = new ContainerBuilder();
            cb.RegisterType<AccountBll>().As<IAccountBll>();
            cb.RegisterType<AccountDal>().As<IAccountDal>();
            cb.RegisterType<DbFactory>().As<IDbFactory>()
                .InstancePerLifetimeScope();
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

            private IExecuteSqlDbConnection _connection;
            public IExecuteSqlDbConnection CreateDbConnection()
            {
                return _connection ?? (_connection = new TransactionOnceDbConnection(_factory()));
            }
        }

        /// <summary>
        /// 除非上次事务结束，否则只会开启一次事务的链接
        /// </summary>
        public class TransactionOnceDbConnection : IExecuteSqlDbConnection
        {
            private readonly IExecuteSqlDbConnection _innerConnection;
            private IDbTransaction _innerDbTransaction;
            public TransactionOnceDbConnection(
                IExecuteSqlDbConnection innerConnection)
            {
                _innerConnection = innerConnection;
            }

            public void Dispose()
            {
                _innerConnection.Dispose();
            }

            public IDbTransaction BeginTransaction()
            {
                if (_innerDbTransaction != null)
                {
                    return _innerDbTransaction;
                }
                return _innerDbTransaction = _innerConnection.BeginTransaction();
            }

            public IDbTransaction BeginTransaction(IsolationLevel il)
            {
                if (_innerDbTransaction != null)
                {
                    return _innerDbTransaction;
                }
                return _innerDbTransaction = _innerConnection.BeginTransaction(il);
            }

            public void Close()
            {
                _innerConnection.Close();
            }

            public void ChangeDatabase(string databaseName)
            {
                _innerConnection.ChangeDatabase(databaseName);
            }

            public IDbCommand CreateCommand()
            {
                return _innerConnection.CreateCommand();
            }

            public void Open()
            {
                _innerConnection.Open();
            }

            public string ConnectionString
            {
                get => _innerConnection.ConnectionString;
                set => _innerConnection.ConnectionString = value;
            }

            public int ConnectionTimeout => _innerConnection.ConnectionTimeout;

            public string Database => _innerConnection.Database;

            public ConnectionState State => _innerConnection.State;
            public void ExecuteSql(string sql, object[] ps, IDbTransaction dbTransaction = null)
            {
                _innerConnection.ExecuteSql(sql, ps, _innerDbTransaction ?? dbTransaction);
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
                            _accountDal.UpdateBalance(fromAccountId, fromAmount);
                            _accountDal.UpdateBalance(toAccountId, toAmount);
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
            void UpdateBalance(string id, decimal balance);
        }

        public class AccountDal : IAccountDal
        {
            private readonly IDbFactory _dbFactory;

            public AccountDal(
                IDbFactory dbFactory)
            {
                this._dbFactory = dbFactory;
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
                var dbConnection = _dbFactory.CreateDbConnection();
                dbConnection.ExecuteSql("更新语句：更新 {0} 余额为 {1}", new object[] { id, balance });
                _accounts[id] = balance;
            }
        }
    }
}
