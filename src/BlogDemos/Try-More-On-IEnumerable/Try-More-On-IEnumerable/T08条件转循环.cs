namespace Try_More_On_IEnumerable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/ftbRfN
    /// </summary>
    public class T08条件转循环
    {
        public static void Run()
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
    }
}