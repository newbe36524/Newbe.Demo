using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;

namespace Newbe.StringPools
{
    public class HelperTest
    {
        public const int ProductCount = 1_000_000;

        #region Colors

        public static readonly List<string> Colors = new[]
        {
            "amber",
            "amethyst",
            "antique",
            "antique",
            "antique",
            "antique",
            "apricot",
            "aqua",
            "aquamarine",
            "aquamarine",
            "auburn",
            "august",
            "autumn",
            "azure",
            "azure",
            "baby",
            "baby",
            "bark",
            "begin",
            "beige",
            "benzo",
            "berry",
            "biscuit",
            "bisque",
            "black",
            "blue",
            "blue",
            "blueviolet",
            "bluish",
            "blush",
            "bone",
            "bottle",
            "brick",
            "bronze",
            "brown",
            "buff",
            "burgundy",
            "burly",
            "butter",
            "butter",
            "cadet",
            "calamineblue",
            "camel",
            "camouflage",
            "caramel",
            "carmine",
            "carnation",
            "celery",
            "celeste",
            "cerise",
            "chalky",
            "chambray",
            "charcoal",
            "charcoal",
            "chartreuse",
            "cherry",
            "chestnut",
            "chocolate",
            "chost",
            "cinnamon",
            "citrine",
            "citrus",
            "classic",
            "clay",
            "clear",
            "cobalt",
            "cobalt",
            "cochineal",
            "cocoa",
            "coffee",
            "cold",
            "colour",
            "colour",
            "colour",
            "complementary",
            "contracting",
            "contrast",
            "copper",
            "copper",
            "coral",
            "coral",
            "cornflower",
            "cornsilk",
            "cream",
            "cream",
            "crepe",
            "crimson",
            "crystal",
            "crystaline"
        }.OrderBy(x => x).ToList();

        #endregion

        [Test]
        public async Task CreateDb()
        {
            var fileName = "data.db";
            if (File.Exists(fileName))
            {
                return;
            }

            var connectionString = GetConnectionString(fileName);
            await using var sqlConnection = new SQLiteConnection(connectionString);
            await sqlConnection.OpenAsync();
            await using var transaction = await sqlConnection.BeginTransactionAsync();
            await sqlConnection.ExecuteAsync(@"
        CREATE TABLE Product(
            ProductId int PRIMARY KEY,
            Color TEXT
        )", transaction);

            var dict = CreateDict();
            foreach (var (_, p) in dict)
            {
                await sqlConnection.ExecuteAsync(@"
        INSERT INTO Product(ProductId,Color)
        VALUES(@ProductId,@Color)", p, transaction);
            }

            await transaction.CommitAsync();
        }

        public static string GetConnectionString()
        {
            var fileName = "data.db";
            return GetConnectionString(fileName);
        }

        public static string GetConnectionString(string filename)
        {
            var re =
                $"Data Source={filename};Cache Size=5000;Journal Mode=WAL;Pooling=True;Default IsolationLevel=ReadCommitted";
            return re;
        }

        public static Dictionary<int, ProductInfo> CreateDict()
        {
            var random = new Random(36524);
            var dict = new Dictionary<int, ProductInfo>(ProductCount);
            for (int i = 0; i < ProductCount; i++)
            {
                dict.Add(i, new ProductInfo
                {
                    ProductId = i,
                    Color = Colors[random.Next(0, Colors.Count)]
                });
            }

            return dict;
        }
    }

    public record ProductInfo
    {
        public int ProductId { get; set; }
        public string Color { get; set; }
    }
}