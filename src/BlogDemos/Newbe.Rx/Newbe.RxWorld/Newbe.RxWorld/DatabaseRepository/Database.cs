using System;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository
{
    public class Database : IDatabase
    {
        public async Task InsertOne()
        {
            await using var db = new SQLiteConnection();
            db.ConnectionString = "Data Source=testdb1.db;";
            await db.OpenAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(1));
        }

        public async Task InsertMany(int count)
        {
            await using var db = new SQLiteConnection();
            db.ConnectionString = "Data Source=testdb2.db;";
            await db.OpenAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(1 * count));
        }
    }
}