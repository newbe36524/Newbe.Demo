using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Try_More_On_IEnumerable
{
    public class EnumerableTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EnumerableTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void T01普通的循环获取偶数()
        {
            // 创建一个集合存放最终结果
            var result = new List<int>();
            for (var i = 0; i <= 10; i++)
            {
                // 如果是偶数的话，将数字放入集合中
                if (i % 2 == 0)
                {
                    result.Add(i);
                }
            }

            // 断言最终的结果
            result.Should().Equal(0, 2, 4, 6, 8, 10);
        }

        [Fact]
        public void T02普通Enumerable()
        {
            // 通过静态方法获取偶数的Enumerable对象
            var result = GetNumber(10);

            // 断言结果
            result.Should().Equal(0, 2, 4, 6, 8, 10);

            /**
             * 这种写法比起 T01 的写法少了一个集合的创建，通常来说是性能更佳的写法
             */
        }

        /// <summary>
        /// 获取小于等于max的偶数
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        private static IEnumerable<int> GetNumber(int max)
        {
            for (var i = 0; i <= max; i++)
            {
                if (i % 2 == 0)
                {
                    // 使用 yield return 返回每次迭代的结果
                    yield return i;
                }
            }
        }

        [Fact]
        public void T02本地函数()
        {
            // 通过本地函数获取结果
            var result = GetNumber(10);

            // 断言结果
            result.Should().Equal(0, 2, 4, 6, 8, 10);

            /**
             * 使用本地函数获取 Enumerable 对象。
             * 此处的效果和静态方法类似。
             * 本地函数是 C# 7.0 才开始支持的语法特性。
             * 关于本地函数详细内容可以参见：https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/local-functions
             */
            IEnumerable<int> GetNumber(int max)
            {
                for (var i = 0; i <= max; i++)
                {
                    if (i % 2 == 0)
                    {
                        yield return i;
                    }
                }
            }
        }

        [Fact]
        public void T03分离条件()
        {
            /**
             * 使用 Where 过滤结果
             * 这样的做法分离了“自增”逻辑和“判断是偶数”的逻辑
             */
            var result = GetNumber(10)
                .Where(x => x % 2 == 0);

            result.Should().Equal(0, 2, 4, 6, 8, 10);

            // 迭代时只是进行了自增操作
            IEnumerable<int> GetNumber(int max)
            {
                for (var i = 0; i <= max; i++)
                {
                    yield return i;
                }
            }
        }

        [Fact]
        public void T04Linq产生数值()
        {
            // 系统内置了一个方法来获取数字的自增序列，因此可以如此简化
            var result = Enumerable
                .Range(0, 11)
                .Where(x => x % 2 == 0);

            result.Should().Equal(0, 2, 4, 6, 8, 10);
        }

        [Fact]
        public void T05输出233()
        {
            // 输出3个数字 233
            var result = Get233().Take(3);
            result.Should().Equal(2, 3, 3);

            // 输出5个数字 23333
            result = Get233().Take(5);
            result.Should().Equal(2, 3, 3, 3, 3);

            IEnumerable<int> Get233()
            {
                // 第一次输出 2
                yield return 2;
                while (true)
                {
                    // 后面都输出 3
                    yield return 3;
                }
            }

            /**
             * 值得注意的是 while(true) 并不会导致程序陷入死循环
             * 因为 yield return 是采用按需供给的方式执行的。
             * 关于这点可以参考右侧文章：https://www.cnblogs.com/SilentCode/p/5014068.html
             */
        }

        [Fact]
        public void T06获取10个小于50的随机数()
        {
            /**
             * 过滤随机数中小于50的数字，只获取其中10个结果
             */
            var result = GetRandom()
                .Where(x => x < 50)
                .Take(10)
                .ToArray();

            // 断言
            result.Should().HaveCount(10);
            result.All(x => x < 50).Should().BeTrue();

            IEnumerable<int> GetRandom()
            {
                var random = new Random();
                // 不断输出 0-100 的随机数
                while (true)
                {
                    yield return random.Next(0, 100);
                }
            }
        }

        [Fact]
        public void T07自动补足随机数()
        {
            // 获取3个数字
            var result1 = GetData().Take(3);

            // 3 个数字分别为 0,1,2
            result1.Should().Equal(0, 1, 2);

            // 获取 10 个数字
            var result2 = GetData().Take(10).ToArray();

            // 前 5 个数字分别为 0,1,2,3,4
            result2.Take(5).Should().Equal(0, 1, 2, 3, 4);
            // 从第 5 个开始的数字都大于 10
            result2.Skip(5).Take(5).Should().Match(x => x.All(a => a > 10));

            /**
             * 获取一组数据，前5个数字是 0,1,2,3,4 。后面继续获取则为随机数
             */
            IEnumerable<int> GetData()
            {
                var staticData = new[] {0, 1, 2, 3, 4};
                foreach (var i in staticData)
                {
                    yield return i;
                }

                var random = new Random();
                // 不断输出 10-100 的随机数
                while (true)
                {
                    yield return random.Next(10, 100);
                }
            }
        }

        [Fact]
        public void T08条件转循环()
        {
            var book1 = GetReadingBook(DateTime.Parse("2019-08-30")).First();
            book1.Should().Be("每个周五都是快乐的一天");

            var book2 = GetReadingBook(DateTime.Parse("2016-02-29")).First();
            book2.Should().Be("四年一次的邂逅");

            var book3 = GetReadingBook(DateTime.Parse("2019-09-01")).First();
            book3.Should().BeSameAs("月老板软件开发小妙招三十二则");

            // 获取给定时间需要阅读的书籍名称
            IEnumerable<string> GetReadingBook(DateTime time)
            {
                // 周五
                if (time.DayOfWeek == DayOfWeek.Friday)
                {
                    yield return "每个周五都是快乐的一天";
                }

                // 2月29日
                if (time.Date.Month == 2 && time.Date.Day == 29)
                {
                    yield return "四年一次的邂逅";
                }

                // 其他时间阅读名著
                yield return "月老板软件开发小妙招三十二则";
            }
        }

        [Fact]
        public void T09获取一个链接字符串()
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

        [Fact]
        public async Task T10测试网络连接()
        {
            var httpClient = new HttpClient();
            try
            {
                await Task.WhenAll(SendRequests());
                _testOutputHelper.WriteLine("当前网络连接正常");
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine("当前网络不正常，请检查网络连接");
            }

            IEnumerable<Task> SendRequests()
            {
                yield return Task.Run(() => httpClient.GetAsync("http://www.baidu.com"));
                yield return Task.Run(() => httpClient.GetAsync("http://www.bing.com"));
                yield return Task.Run(() => httpClient.GetAsync("http://www.taobao.com"));
            }
        }
    }
}