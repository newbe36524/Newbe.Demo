using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Newbe.ExpressionsTests
{
    public class X01CallMethodTest
    {
        private const int Count = 1_000_000;
        private const int Diff = 100;

        [SetUp]
        public void Init()
        {
            _methodInfo = typeof(Claptrap).GetMethod(nameof(Claptrap.LevelUp));
            Debug.Assert(_methodInfo != null, nameof(_methodInfo) + " != null");

            var instance = Expression.Parameter(typeof(Claptrap), "c");
            var levelP = Expression.Parameter(typeof(int), "l");
            var callExpression = Expression.Call(instance, _methodInfo, levelP);
            var lambdaExpression = Expression.Lambda<Action<Claptrap, int>>(callExpression, instance, levelP);
            // lambdaExpression should be as (Claptrap c,int l) =>  { c.LevelUp(l); }
            _func = lambdaExpression.Compile();
        }

        [Test]
        public void RunReflection()
        {
            var claptrap = new Claptrap();
            for (int i = 0; i < Count; i++)
            {
                _methodInfo.Invoke(claptrap, new[] {(object) Diff});
            }

            claptrap.Level.Should().Be(Count * Diff);
        }

        [Test]
        public void RunExpression()
        {
            var claptrap = new Claptrap();
            for (int i = 0; i < Count; i++)
            {
                _func.Invoke(claptrap, Diff);
            }

            claptrap.Level.Should().Be(Count * Diff);
        }

        private MethodInfo _methodInfo;
        private Action<Claptrap, int> _func;

        public class Claptrap
        {
            public int Level { get; set; }

            public void LevelUp(int diff)
            {
                Level += diff;
            }
        }
    }
}