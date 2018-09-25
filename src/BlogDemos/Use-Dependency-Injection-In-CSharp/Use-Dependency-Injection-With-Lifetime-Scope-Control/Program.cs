using System;
using System.Data;

namespace Use_Dependency_Injection_With_Lifetime_Scope_Control
{
    class Program
    {
        static void Main(string[] args)
        {
            Demo1.Run();
            Demo2.Run();
            Demo3.Run();
            Demo4.Run();
            Demo5.Run();
        }
    }

    /// <summary>
    /// 能够直接执行语句的数据库链接
    /// </summary>
    public interface IExecuteSqlDbConnection : IDbConnection
    {
        /// <summary>
        /// 执行数据库语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ps"></param>
        /// <param name="dbTransaction"></param>
        void ExecuteSql(string sql, object[] ps, IDbTransaction dbTransaction = null);
    }

    /// <summary>
    /// 只会向控制台输出内容的数据库连接
    /// </summary>
    public class ConsoleDbConnection : IExecuteSqlDbConnection
    {
        public delegate ConsoleDbConnection Factory();

        public void Dispose()
        {
            Console.WriteLine("数据库连接：释放");
        }

        public IDbTransaction BeginTransaction()
        {
            return new ConsoleOutDbTransaction(this, IsolationLevel.Unspecified);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return new ConsoleOutDbTransaction(this, il);
        }

        public void Close()
        {
            Console.WriteLine("数据库连接：关闭");
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException();
        }

        public IDbCommand CreateCommand()
        {
            throw new NotSupportedException();
        }

        public void Open()
        {
            throw new NotSupportedException();
        }

        public string ConnectionString { get; set; }

        public int ConnectionTimeout
        {
            get { throw new NotSupportedException(); }
        }

        public string Database
        {
            get { throw new NotSupportedException(); }
        }

        public ConnectionState State
        {
            get { throw new NotSupportedException(); }
        }

        public void ExecuteSql(string sql, object[] ps, IDbTransaction dbTransaction = null)
        {
            if (dbTransaction == null)
            {
                Console.WriteLine($"无事务执行：{string.Format(sql, ps)}");
            }
            else
            {
                Console.WriteLine($"有事务执行：{string.Format(sql, ps)}");
            }
        }
    }

    /// <summary>
    /// 只会向控制台输出内容的事务
    /// </summary>
    public class ConsoleOutDbTransaction : IDbTransaction
    {
        public ConsoleOutDbTransaction(IDbConnection connection, IsolationLevel isolationLevel)
        {
            Connection = connection;
            IsolationLevel = isolationLevel;
        }

        public void Dispose()
        {
            Console.WriteLine("事务：释放");
        }

        public void Commit()
        {
            Console.WriteLine("事务：提交");
        }

        public void Rollback()
        {
            Console.WriteLine("事务：回滚");
        }

        public IDbConnection Connection { get; }
        public IsolationLevel IsolationLevel { get; }
    }
}
