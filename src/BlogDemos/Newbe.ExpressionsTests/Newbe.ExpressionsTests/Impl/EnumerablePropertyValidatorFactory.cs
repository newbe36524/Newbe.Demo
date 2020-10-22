using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Interfaces;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public class EnumerablePropertyValidatorFactory : IPropertyValidatorFactory
    {
        private static Expression<Func<string, IEnumerable<T>, ValidateResult>> CreateValidateIntRangeExp<T>()
        {
            return (name, value) =>
                !value.Any()
                    ? ValidateResult.Error($"{name} must contains more than one element")
                    : ValidateResult.Ok();
        }

        public IEnumerable<Expression> CreateExpression(CreatePropertyValidatorInput input)
        {
            var type = input.PropertyInfo.PropertyType;
            if (type == typeof(string))
            {
                yield break;
            }

            var item = type
                .GetInterfaces()
                .FirstOrDefault(x => x.Name == "IEnumerable`1");
            if (item == null)
            {
                yield break;
            }

            var method = typeof(EnumerablePropertyValidatorFactory)
                .GetMethod(nameof(CreateValidateIntRangeExp), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(item.GenericTypeArguments[0]);
            var exp = method.Invoke(null, null);
            var result = ExpressionHelper.CreateValidateExpression(input, (Expression) exp);

            yield return result;
        }
    }
}