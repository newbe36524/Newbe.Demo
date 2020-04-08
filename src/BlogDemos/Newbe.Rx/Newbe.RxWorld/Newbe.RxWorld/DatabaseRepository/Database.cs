using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace Newbe.RxWorld.DatabaseRepository
{
    public class Database : IDatabase
    {
        private static readonly object Locker = new object();
        private int _count = 0;

        public async Task<int> InsertOne(int item)
        {
            await using var db = new SQLiteConnection();
            db.ConnectionString = "Data Source=testdb1.db;";
            await db.OpenAsync();
            await Task.Delay(TimeSpan.FromMilliseconds(1));
            lock (Locker)
            {
                _count++;
            }

            return _count;
        }

        public async Task<int> InsertMany(IEnumerable<int> items)
        {
            await using var db = new SQLiteConnection();
            db.ConnectionString = "Data Source=testdb2.db;";
            await db.OpenAsync();
            var length = items.ToArray().Length;
            await Task.Delay(TimeSpan.FromMilliseconds(1 * length));
            lock (Locker)
            {
                _count += length;
            }

            return _count;
        }
    }
}