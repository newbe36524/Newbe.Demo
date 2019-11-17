namespace Try_More_On_IEnumerable
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;

    /// <summary>
    /// https://dotnetfiddle.net/Widget/KbtqSi
    /// </summary>
    public class T11分组合并
    {
        public static void Run()
        {
            var array1 = new[] {0, 1, 2, 3, 4};
            var array2 = new[] {5, 6, 7, 8, 9};

            // 通过本地方法合并两个数组为一个数据
            var result1 = ConcatArray(array1, array2).ToArray();

            // 使用 Linq 中的 Concat 来合并两个 IEnumerable 对象
            var result2 = array1.Concat(array2).ToArray();

            // 使用 Linq 中的 SelectMany 将 “二维数据” 拉平合并为一个数组
            var result3 = new[] {array1, array2}.SelectMany(x => x).ToArray();

            /**
             *  使用 Enumerable.Range 生成一个数组，这个数据的结果为
             *  0,1,2,3,4,5,6,7,8,9
             */
            var result = Enumerable.Range(0, 10).ToArray();

            // 通过以上三种方式合并的结果时相同的
            result1.Should().Equal(result);
            result2.Should().Equal(result);
            result3.Should().Equal(result);

            IEnumerable<T> ConcatArray<T>(IEnumerable<T> source1, IEnumerable<T> source2)
            {
                foreach (var item in source1)
                {
                    yield return item;
                }

                foreach (var item in source2)
                {
                    yield return item;
                }
            }
        }
    }
}