using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            var finalMethod = typeof(EnumerablePropertyValidatorFactory)
                .GetMethod(nameof(CreateValidateExpression), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(item);

            var result = (Expression) finalMethod.Invoke(null, new[] {input, exp});
            yield return result;
        }

        private static Expression CreateValidateExpression<TValue>(
            CreatePropertyValidatorInput input,
            Expression<Func<string, TValue, ValidateResult>> validateFuncExpression)
        {
            var propertyInfo = input.PropertyInfo;
            var isOkProperty = typeof(ValidateResult).GetProperty(nameof(ValidateResult.IsOk));
            Debug.Assert(isOkProperty != null, nameof(isOkProperty) + " != null");

            var convertedExp = Expression.Convert(input.InputExpression, input.InputType);
            var propExp = Expression.Property(convertedExp, propertyInfo);
            var nameExp = Expression.Constant(propertyInfo.Name);

            var requiredMethodExp = Expression.Invoke(
                validateFuncExpression,
                nameExp,
                propExp);
            var assignExp = Expression.Assign(input.ResultExpression, requiredMethodExp);
            var resultIsOkPropertyExp = Expression.Property(input.ResultExpression, isOkProperty);
            var conditionExp = Expression.IsFalse(resultIsOkPropertyExp);
            var ifThenExp =
                Expression.IfThen(conditionExp,
                    Expression.Return(input.ReturnLabel, input.ResultExpression));
            var re = Expression.Block(
                new[] {input.ResultExpression},
                assignExp,
                ifThenExp);
            return re;
        }
    }
}