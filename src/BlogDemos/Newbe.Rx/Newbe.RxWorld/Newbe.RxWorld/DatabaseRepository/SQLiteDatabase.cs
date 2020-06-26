using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Newbe.RxWorld.DatabaseRepository
{
    public class SQLiteDatabase : IDatabase
    {
        private readonly string _dbFileName;

        public SQLiteDatabase(string dbFileName)
        {
            _dbFileName = dbFileName;
            CreateDatabase().Wait();
        }

        public async Task<int> InsertOne(int item)
        {
            await using var db = CreateConnection();
            await db.ExecuteAsync("INSERT INTO TestTable (id,value) VALUES (@id,@value)",
                new {id = item, value = item});
            var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM TestTable");
            return count;
        }

        public async Task<int> InsertMany(IEnumerable<int> items)
        {
            var array = items.ToArray();
            var ps = new DynamicParameters();

            var sqlBuilder = new StringBuilder("INSERT INTO TestTable (id,value) VALUES");
            for (var i = 0; i < array.Length; i++)
            {
                var name = $"@id{i},@value{i}";
                sqlBuilder.Append(i == array.Length - 1 ? $"({name});" : $"({name}),");
                ps.Add($"@id{i}", array[i]);
                ps.Add($"@value{i}", array[i]);
            }

            var sql = sqlBuilder.ToString();
            await using var db = CreateConnection();
            await db.ExecuteAsync(sql, ps);

            var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM TestTable");
            return count;
        }

        public async Task<int> UpsertOne(int key, int value)
        {
            await using var db = CreateConnection();
            await db.ExecuteAsync("INSERT OR REPLACE INTO TestTable (id,value) VALUES (@id,@value)",
                new {id = key, value = value});
            return 0;
        }

        public async Task<int> UpsertMany(Dictionary<int, int> values)
        {
            var array = values.ToArray();
            var ps = new DynamicParameters();
            var sqlBuilder = new StringBuilder("INSERT OR REPLACE INTO TestTable (id,value) VALUES");

            for (var i = 0; i < array.Length; i++)
            {
                var name = $"@id{i},@value{i}";
                sqlBuilder.Append(i == array.Length - 1 ? $"({name});" : $"({name}),");
                ps.Add($"@id{i}", array[i].Key);
                ps.Add($"@value{i}", array[i].Value);
            }

            var sql = sqlBuilder.ToString();
            await using var db = CreateConnection();
            await db.ExecuteAsync(sql, ps);
            return 0;
        }

        private async Task CreateDatabase()
        {
            if (File.Exists(DbFilePath))
            {
                File.Delete(DbFilePath);
            }

            await using var db = CreateConnection();
            await db.ExecuteAsync(
                @"CREATE TABLE IF NOT EXISTS TestTable (id int PRIMARY KEY,value int NOT NULL) WITHOUT ROWID;");
        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection
                {ConnectionString = $"Data Source={DbFilePath};Cache Size=5000;Journal Mode=WAL;Pooling=True;"};
        }

        private string DbFilePath => _dbFileName;
    }
}