using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Newbe.ExpressionsTests.FilterFactory;
using NUnit.Framework;

namespace Newbe.ExpressionsTests.Examples
{
    public class Z01SingleWhereTest
    {
        [Test]
        public void Normal()
        {
            var re = Enumerable.Range(0, 10).AsQueryable() // 0-9
                .Where(x => x >= 1 && x < 5).ToList(); // 1 2 3 4
            var expectation = Enumerable.Range(1, 4); // 1 2 3 4
            re.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public void Expression00()
        {
            Expression<Func<int, bool>> filter = x => x >= 1 && x < 5;
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public void Expression01()
        {
            var minValue = 1;
            var maxValue = 5;
            Expression<Func<int, bool>> filter = x => x >= minValue && x < maxValue;
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);
        }

        [Test]
        public void Expression02()
        {
            var filter = CreateFilter(1, 5);
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Expression<Func<int, bool>> CreateFilter(int minValue, int maxValue)
            {
                return x => x >= minValue && x < maxValue;
            }
        }

        [Test]
        public void Expression03()
        {
            var filter = CreateFilter(x => x >= 1, x => x < 5);
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Expression<Func<int, bool>> CreateFilter(Func<int, bool> leftFunc, Func<int, bool> rightFunc)
            {
                return x => leftFunc.Invoke(x) && rightFunc.Invoke(x);
            }
        }

        [Test]
        public void Expression04()
        {
            var filter = CreateFilter(x => x >= 1, x => x < 5);
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Expression<Func<int, bool>> CreateFilter(Expression<Func<int, bool>> leftFunc,
                Expression<Func<int, bool>> rightFunc)
            {
                // x
                var pExp = Expression.Parameter(typeof(int), "x");
                // (a => leftFunc(a))(x)
                var leftExp = Expression.Invoke(leftFunc, pExp);
                // (a => rightFunc(a))(x)
                var rightExp = Expression.Invoke(rightFunc, pExp);
                // (a => leftFunc(a))(x) && (a => rightFunc(a))(x)
                var bodyExp = Expression.AndAlso(leftExp, rightExp);
                // x => (a => leftFunc(a))(x) && (a => rightFunc(a))(x)
                var resultExp = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                return resultExp;
            }
        }

        [Test]
        public void Expression05()
        {
            var filter = CreateFilter(x => x >= 1, x => x < 5);
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Expression<Func<int, bool>> CreateFilter(Expression<Func<int, bool>> leftFunc,
                Expression<Func<int, bool>> rightFunc)
            {
                // x
                var pExp = Expression.Parameter(typeof(int), "x");
                // leftFunc(x)
                var leftExp = leftFunc.Unwrap(pExp);
                // rightFunc(x)
                var rightExp = rightFunc.Unwrap(pExp);
                // leftFunc(x) && rightFunc(x)
                var bodyExp = Expression.AndAlso(leftExp, rightExp);
                // x => leftFunc(x) && rightFunc(x)
                var resultExp = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                return resultExp;
            }
        }

        [Test]
        public void Expression06()
        {
            var filter = JoinSubFilters(Expression.AndAlso, x => x >= 1, x => x < 5);
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Expression<Func<int, bool>> JoinSubFilters(Func<Expression, Expression, Expression> expJoiner,
                params Expression<Func<int, bool>>[] subFilters)
            {
                // x
                var pExp = Expression.Parameter(typeof(int), "x");
                var result = subFilters[0];
                foreach (var sub in subFilters[1..])
                {
                    var leftExp = result.Unwrap(pExp);
                    var rightExp = sub.Unwrap(pExp);
                    var bodyExp = expJoiner(leftExp, rightExp);

                    result = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                }

                return result;
            }
        }

        [Test]
        public void Expression07()
        {
            var filter = JoinSubFilters(Expression.AndAlso,
                CreateMinValueFilter(1),
                x => x < 5);
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Expression<Func<int, bool>> CreateMinValueFilter(int minValue)
            {
                return x => x >= minValue;
            }

            Expression<Func<int, bool>> JoinSubFilters(Func<Expression, Expression, Expression> expJoiner,
                params Expression<Func<int, bool>>[] subFilters)
            {
                // x
                var pExp = Expression.Parameter(typeof(int), "x");
                var result = subFilters[0];
                foreach (var sub in subFilters[1..])
                {
                    var leftExp = result.Unwrap(pExp);
                    var rightExp = sub.Unwrap(pExp);
                    var bodyExp = expJoiner(leftExp, rightExp);

                    result = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                }

                return result;
            }
        }

        [Test]
        public void Expression08()
        {
            var filter = JoinSubFilters(Expression.AndAlso,
                CreateMinValueFilter(1),
                x => x < 5);
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Expression<Func<int, bool>> CreateMinValueFilter(int minValue)
            {
                // x
                var pExp = Expression.Parameter(typeof(int), "x");
                // minValue
                var rightExp = Expression.Constant(minValue);
                // x >= minValue
                var bodyExp = Expression.GreaterThanOrEqual(pExp, rightExp);
                var result = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                return result;
            }

            Expression<Func<int, bool>> JoinSubFilters(Func<Expression, Expression, Expression> expJoiner,
                params Expression<Func<int, bool>>[] subFilters)
            {
                // x
                var pExp = Expression.Parameter(typeof(int), "x");
                var result = subFilters[0];
                foreach (var sub in subFilters[1..])
                {
                    var leftExp = result.Unwrap(pExp);
                    var rightExp = sub.Unwrap(pExp);
                    var bodyExp = expJoiner(leftExp, rightExp);

                    result = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                }

                return result;
            }
        }

        [Test]
        public void Expression09()
        {
            var filter = JoinSubFilters(Expression.AndAlso,
                CreateValueCompareFilter(Expression.GreaterThanOrEqual, 1),
                CreateValueCompareFilter(Expression.LessThan, 5));
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Expression<Func<int, bool>> CreateValueCompareFilter(Func<Expression, Expression, Expression> comparerFunc,
                int rightValue)
            {
                var pExp = Expression.Parameter(typeof(int), "x");
                var rightExp = Expression.Constant(rightValue);
                var bodyExp = comparerFunc(pExp, rightExp);
                var result = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                return result;
            }

            Expression<Func<int, bool>> JoinSubFilters(Func<Expression, Expression, Expression> expJoiner,
                params Expression<Func<int, bool>>[] subFilters)
            {
                // x
                var pExp = Expression.Parameter(typeof(int), "x");
                var result = subFilters[0];
                foreach (var sub in subFilters[1..])
                {
                    var leftExp = result.Unwrap(pExp);
                    var rightExp = sub.Unwrap(pExp);
                    var bodyExp = expJoiner(leftExp, rightExp);

                    result = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                }

                return result;
            }
        }

        [Test]
        public void Expression10()
        {
            var config = new Dictionary<string, int>
            {
                { ">=", 1 },
                { "<", 5 }
            };
            var subFilters = config.Select(x => CreateValueCompareFilter(MapConfig(x.Key), x.Value)).ToArray();
            var filter = JoinSubFilters(Expression.AndAlso, subFilters);
            var re = Enumerable.Range(0, 10).AsQueryable()
                .Where(filter).ToList();
            var expectation = Enumerable.Range(1, 4);
            re.Should().BeEquivalentTo(expectation);

            Func<Expression, Expression, Expression> MapConfig(string op)
            {
                return op switch
                {
                    ">=" => Expression.GreaterThanOrEqual,
                    "<" => Expression.LessThan,
                    _ => throw new ArgumentOutOfRangeException(nameof(op))
                };
            }

            Expression<Func<int, bool>> CreateValueCompareFilter(Func<Expression, Expression, Expression> comparerFunc,
                int rightValue)
            {
                var pExp = Expression.Parameter(typeof(int), "x");
                var rightExp = Expression.Constant(rightValue);
                var bodyExp = comparerFunc(pExp, rightExp);
                var result = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                return result;
            }

            Expression<Func<int, bool>> JoinSubFilters(Func<Expression, Expression, Expression> expJoiner,
                params Expression<Func<int, bool>>[] subFilters)
            {
                // x
                var pExp = Expression.Parameter(typeof(int), "x");
                var result = subFilters[0];
                foreach (var sub in subFilters[1..])
                {
                    var leftExp = result.Unwrap(pExp);
                    var rightExp = sub.Unwrap(pExp);
                    var bodyExp = expJoiner(leftExp, rightExp);

                    result = Expression.Lambda<Func<int, bool>>(bodyExp, pExp);
                }

                return result;
            }
        }
    }
}