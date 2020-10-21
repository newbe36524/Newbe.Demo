using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Newbe.ExpressionsTests.Old
{
    public class X02PropertyTest
    {
        private const int Count = 1_000_000;
        private const int Diff = 100;

        [SetUp]
        public void Init()
        {
            _propertyInfo = typeof(Claptrap).GetProperty(nameof(Claptrap.Level));
            Debug.Assert(_propertyInfo != null, nameof(_propertyInfo) + " != null");

            var instance = Expression.Parameter(typeof(Claptrap), "c");
            var levelProperty = Expression.Property(instance, _propertyInfo);
            var levelP = Expression.Parameter(typeof(int), "l");
            var addAssignExpression = Expression.AddAssign(levelProperty, levelP);
            var lambdaExpression = Expression.Lambda<Action<Claptrap, int>>(addAssignExpression, instance, levelP);
            // lambdaExpression should be as (Claptrap c,int l) =>  { c.Level += l; }
            _func = lambdaExpression.Compile();
        }

        [Test]
        public void RunReflection()
        {
            var claptrap = new Claptrap();
            for (int i = 0; i < Count; i++)
            {
                var value = (int) _propertyInfo.GetValue(claptrap);
                _propertyInfo.SetValue(claptrap, value + Diff);
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
        
        [Test]
        public void Directly()
        {
            var claptrap = new Claptrap();
            for (int i = 0; i < Count; i++)
            {
                claptrap.Level += Diff;
            }

            claptrap.Level.Should().Be(Count * Diff);
        }

        private PropertyInfo _propertyInfo;
        private Action<Claptrap, int> _func;

        public class Claptrap
        {
            public int Level { get; set; }
        }
    }
}