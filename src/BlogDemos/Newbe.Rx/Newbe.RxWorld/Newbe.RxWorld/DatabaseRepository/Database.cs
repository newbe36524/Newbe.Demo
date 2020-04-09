using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Newbe.RxWorld.DatabaseRepository
{
    public class Database : IDatabase
    {
        public Database()
        {
            CreateDatabase().Wait();
        }

        public async Task<int> InsertOne(int item)
        {
            await using var db = CreateConnection();
            await db.ExecuteAsync("INSERT INTO TestTable (data) VALUES (@data)", new {data = item});
            var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM TestTable");
            return count;
        }

        public async Task<int> InsertMany(IEnumerable<int> items)
        {
            await using var db = CreateConnection();
            var array = items.ToArray();
            var ps = new DynamicParameters();

            var sb = new StringBuilder("INSERT INTO TestTable (data) VALUES");
            for (var i = 0; i < array.Length; i++)
            {
                var name = $"data{i}";
                sb.Append(i == array.Length - 1 ? $"(@{name});" : $"(@{name}),");
                ps.Add(name, array[i]);
            }

            await db.ExecuteAsync(sb.ToString(), ps);
            var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM TestTable");
            return count;
        }

        private async Task CreateDatabase()
        {
            if (File.Exists(DbFilePath))
            {
                File.Delete(DbFilePath);
            }

            await using var db = CreateConnection();
            await db.ExecuteAsync(@$"CREATE TABLE IF NOT EXISTS TestTable (data int PRIMARY KEY) WITHOUT ROWID;");
        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection {ConnectionString = $"Data Source={DbFilePath};"};
        }

        private string DbFilePath => "testdb.db";
    }
}