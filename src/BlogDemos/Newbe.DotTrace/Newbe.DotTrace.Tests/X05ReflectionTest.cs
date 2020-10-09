using System;
using System.Diagnostics;
using System.Linq.Expressions;
using NUnit.Framework;

namespace Newbe.DotTrace.Tests
{
    public class X05ReflectionTest
    {
        [Test]
        public void RunReflection()
        {
            var methodInfo = GetType().GetMethod(nameof(MoYue));
            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");
            for (int i = 0; i < 1_000_000; i++)
            {
                methodInfo.Invoke(null, null);
            }

            Console.WriteLine(_count);
        }

        [Test]
        public void RunExpression()
        {
            var methodInfo = GetType().GetMethod(nameof(MoYue));
            Debug.Assert(methodInfo != null, nameof(methodInfo) + " != null");
            var methodCallExpression = Expression.Call(methodInfo);
            var lambdaExpression = Expression.Lambda<Action>(methodCallExpression);
            var func = lambdaExpression.Compile();
            for (int i = 0; i < 1_000_000; i++)
            {
                func.Invoke();
            }

            Console.WriteLine(_count);
        }

        private static int _count = 0;

        public static void MoYue()
        {
            _count++;
        }
    }
}