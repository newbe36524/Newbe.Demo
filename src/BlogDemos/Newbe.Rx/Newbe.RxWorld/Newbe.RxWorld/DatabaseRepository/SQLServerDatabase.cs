using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Newbe.RxWorld.DatabaseRepository
{
    public class SQLServerDatabase : IDatabase
    {
        public SQLServerDatabase()
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

            var sqlBuilder = new StringBuilder("INSERT INTO TestTable (data) VALUES");
            for (var i = 0; i < array.Length; i++)
            {
                var name = $"data{i}";
                sqlBuilder.Append(i == array.Length - 1 ? $"(@{name});" : $"(@{name}),");
                ps.Add(name, array[i]);
            }

            var sql = sqlBuilder.ToString();
            await db.ExecuteAsync(sql, ps);

            var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM TestTable");
            return count;
        }

        private async Task CreateDatabase()
        {
            await using var db = CreateConnection();
            await db.ExecuteAsync(@"
DROP TABLE IF EXISTS TestTable;
CREATE TABLE TestTable (data int PRIMARY KEY);");
        }

        private static SqlConnection CreateConnection()
        {
            var sqlConnection = new SqlConnection
            {
                ConnectionString = "Data Source=localhost;User ID=sa;Password=sapwd;Persist Security Info=True;"
            };
            return sqlConnection;
        }
    }
}