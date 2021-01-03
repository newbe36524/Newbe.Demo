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
            await using var db = CreateConnection();
            await db.OpenAsync();
            var trans = await db.BeginTransactionAsync();

            foreach (var t in array)
            {
                await db.ExecuteAsync("INSERT INTO TestTable (id,value) VALUES (@id,@value)",
                    new {id = t, value = t});
            }

            await trans.CommitAsync();
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
            await using var db = CreateConnection();
            await db.OpenAsync();
            var trans = await db.BeginTransactionAsync();

            foreach (var (key, value) in array)
            {
                await db.ExecuteAsync("INSERT OR REPLACE INTO TestTable (id,value) VALUES (@id,@value)",
                    new {id = key, value = value});
            }

            await trans.CommitAsync();
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
            return new() {ConnectionString = $"Data Source={DbFilePath};Cache Size=5000;Journal Mode=WAL;Pooling=True;"};
        }

        private string DbFilePath => _dbFileName;
    }
}