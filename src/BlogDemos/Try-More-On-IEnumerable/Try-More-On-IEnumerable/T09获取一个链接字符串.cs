namespace Try_More_On_IEnumerable
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/vShbOW
    /// </summary>
    public class T09获取一个链接字符串
    {
        public static void Run()
        {
            // 获取一个当前可用的链接字符串
            var conn = GetConnectionString().FirstOrDefault();
            conn.Should().Be("Source=赛博坦;UID=月x;Password=******");

            IEnumerable<string> GetConnectionString()
            {
                // 从一个文件中获取链接字符串
                var connFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conn.txt");
                if (File.Exists(connFilename))
                {
                    var fileContent = File.ReadAllText(connFilename);
                    yield return fileContent;
                }

                // 从配置文件中读取链接字符串
                var dbConnectionString = ConfigurationManager.ConnectionStrings["db"]?.ConnectionString;
                if (!string.IsNullOrEmpty(dbConnectionString))
                {
                    yield return dbConnectionString;
                }

                // 返回默认的字符串
                yield return "Source=赛博坦;UID=月x;Password=******";
            }
        }
    }
}