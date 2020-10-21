using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace Newbe.ExpressionsTests.Old
{
    public class LiveTest
    {
        [Test]
        public void Normal()
        {
            var list = new List<Person>
            {
                new Person
                {
                    Name = "Traceless",
                    Level = int.MaxValue,
                    Age = 18
                },
                new Person
                {
                    Name = "newbe36524",
                    Level = 666,
                    Age = 50
                }
            };

            var input = new Input
            {
                Query = new Dictionary<string, string>
                {
                    {nameof(Person.Level), "1000"},
                    {nameof(Person.Age), "10"}
                }
            };

            var filterList = new List<Expression>();
            var inputExp = Expression.Parameter(typeof(Person), "x");
            foreach (var (k, v) in input.Query)
            {
                if (int.TryParse(v, out var value))
                {
                    var item = CreateRangeMinFilterInnerBlock(inputExp, k, value);
                    filterList.Add(item);
                }
            }

            Expression seed = Expression.Constant(true);
            foreach (var item in filterList)
            {
                seed = Expression.And(seed, item);
            }

            var filter = Expression.Lambda<Func<Person, bool>>(seed, inputExp);

            // Expression<Func<Person, bool>> filter2 = x => x.Name == "newbe36524";
            // x => x.Level > 1000 && x.Name == "newbe36524";
            var array1 = list.AsEnumerable().Where(filter.Compile()).ToArray();
            var array2 = list.AsQueryable().Where(filter).ToArray();
            array1.Single().Name.Should().Be("Traceless");
            array2.Single().Name.Should().Be("Traceless");
        }

        [Test]
        public void Normal1()
        {
            // 1 + 2
            var binaryExpression = Expression.Add(Expression.Constant(1), Expression.Constant(2));
            // ()=> 1 + 2
            var expression = Expression.Lambda<Func<int>>(binaryExpression);
            var func = expression.Compile();
            var re = func.Invoke();
            re.Should().Be(3);
        }


        [Test]
        public void Normal2()
        {
            // false && true
            var binaryExpression = Expression.And(Expression.Constant(false), Expression.Constant(true));
            // ()=> false && true
            var expression = Expression.Lambda<Func<bool>>(binaryExpression);
            var func = expression.Compile();
            var re = func.Invoke();
            re.Should().Be(false);
        }

        public static Expression<Func<Person, bool>> CreateRangeMinFilter(string propertyName, int minValue)
        {
            // Expression<Func<Person, bool>> filter = x => x.Level > level;
            var pExp = Expression.Parameter(typeof(Person), "x");
            var levelExp = Expression.Property(pExp, propertyName);
            var rightExp = Expression.Constant(minValue);
            var bodyExp = Expression.GreaterThan(levelExp, rightExp);
            var finalExp = Expression.Lambda<Func<Person, bool>>(bodyExp, pExp);
            return finalExp;
        }

        public static Expression CreateRangeMinFilterInnerBlock(Expression pExp, string propertyName, int minValue)
        {
            // x.Level > level;
            var levelExp = Expression.Property(pExp, propertyName);
            var rightExp = Expression.Constant(minValue);
            var bodyExp = Expression.GreaterThan(levelExp, rightExp);
            return bodyExp;
        }

        public class Input
        {
            public int? MinLevel { get; set; }

            public Dictionary<string, string> Query { get; set; }
        }

        public class Person
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public int Age { get; set; }
        }
    }
}