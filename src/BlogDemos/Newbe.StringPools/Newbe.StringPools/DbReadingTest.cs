using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using Dapper;
using Humanizer;
using JetBrains.dotMemoryUnit;
using NUnit.Framework;

namespace Newbe.StringPools
{
    public class DbReadingTest
    {
        [Test]
        [DotMemoryUnit(FailIfRunWithoutSupport = false)]
        public async Task LoadFromDbAsync()
        {
            var beforeStart = dotMemory.Check();
            var dict = new Dictionary<int, ProductInfo>(HelperTest.ProductCount);
            await LoadCoreAsync(dict);
            GC.Collect();
            dotMemory.Check(memory =>
            {
                var snapshotDifference = memory.GetDifference(beforeStart);
                Console.WriteLine(snapshotDifference.GetNewObjects().SizeInBytes.Bytes());
            });
        }

        public static async Task LoadCoreAsync(Dictionary<int, ProductInfo> dict)
        {
            var connectionString = HelperTest.GetConnectionString();
            await using var sqlConnection = new SQLiteConnection(connectionString);
            await sqlConnection.OpenAsync();
            await using var reader = await sqlConnection.ExecuteReaderAsync(
                "SELECT ProductId, Color FROM Product");
            var rowParser = reader.GetRowParser<ProductInfo>();
            while (await reader.ReadAsync())
            {
                var productInfo = rowParser.Invoke(reader);
                dict[productInfo.ProductId] = productInfo;
            }
        }
    }
}