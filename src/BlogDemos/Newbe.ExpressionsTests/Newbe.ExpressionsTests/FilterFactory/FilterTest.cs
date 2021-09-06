using System;
using System.Linq.Expressions;
using AgileObjects.ReadableExpressions;
using FluentAssertions;
using Newbe.ExpressionsTests.FilterFactory.Impl;
using NUnit.Framework;
using DD = System.Collections.Generic.Dictionary<string, object>;

namespace Newbe.ExpressionsTests.FilterFactory
{
    public class FilterTest
    {
        [Test]
        public void Test()
        {
            var root = new FilterNodeRelation<DD>(
                new FilterNodeRelation<DD>(
                    new IsDalaoFilterNode(),
                    new LikeGongliangFilterNode(),
                    nameof(Expression.AndAlso)
                ),
                new ValueEqFilterNode<int>("Age", 18),
                nameof(Expression.AndAlso));

            var exp = root.CreateExpression();
            Console.WriteLine(exp.ToReadableString());
            var func = exp.Compile();

            func.Invoke(new DD
            {
                { "Name", "40W" },
                { "Age", 18 },
                { "GongliangFans", "Level99" }
            }).Should().BeTrue();

            func.Invoke(new DD
            {
                { "Name", "40W2" },
                { "Age", 18 },
                { "GongliangFans", "Level99" }
            }).Should().BeFalse("Name false");
        }
    }
}