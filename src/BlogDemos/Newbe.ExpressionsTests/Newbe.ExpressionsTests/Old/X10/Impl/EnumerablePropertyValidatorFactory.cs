using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newbe.ExpressionsTests.Old.X10.Interfaces;
using Newbe.ExpressionsTests.Old.X10.Model;

namespace Newbe.ExpressionsTests.Old.X10.Impl
{
    public class EnumerablePropertyValidatorFactory : IPropertyValidatorFactory
    {
        public IEnumerable<Expression> CreateExpression(CreatePropertyValidatorInput input)
        {
            var type = input.PropertyInfo.PropertyType;
            if (type == typeof(string))
            {
                yield break;
            }

            var interfaces = type
                .GetInterfaces();
            var allInterface = interfaces.Concat(new[] {type});
            var item = allInterface
                .FirstOrDefault(x => x.Name == "IEnumerable`1");
            if (item == null)
            {
                yield break;
            }

            yield return CreateValidateAtLeastOneElementExpression(input, item);
            yield return CreateValidateIsArrayOrListExpression(input, item);
        }

        private static Expression CreateValidateIsArrayOrListExpression(
            CreatePropertyValidatorInput input,
            Type enumerableInterfaceType)
        {
            var valueExp = Expression.Parameter(enumerableInterfaceType, "value");
            var body = Expression.TypeIs(valueExp, typeof(ICollection));
            var bodyExp = Expression.IsFalse(body);

            var funcType = Expression.GetFuncType(enumerableInterfaceType, typeof(bool));
            var finalExp = Expression.Lambda(funcType, bodyExp, valueExp);

            return ExpressionHelper.CreateValidateExpression(input,
                ExpressionHelper.CreateCheckerExpression(enumerableInterfaceType, finalExp,
                    ArrayOrListErrorMessageFunc));
        }


        private static Expression CreateValidateAtLeastOneElementExpression(
            CreatePropertyValidatorInput input,
            Type item)
        {
            var valueExp = Expression.Parameter(item, "value");
            var anyExp = Expression.Call(typeof(Enumerable), nameof(Enumerable.Any), new[]
            {
                item.GenericTypeArguments[0]
            }, valueExp);
            var bodyExp = Expression.IsFalse(anyExp);

            var funcType = Expression.GetFuncType(item, typeof(bool));
            var finalExp = Expression.Lambda(funcType, bodyExp, valueExp);

            return ExpressionHelper.CreateValidateExpression(input,
                ExpressionHelper.CreateCheckerExpression(item, finalExp, AtLeastElementErrorMessageFunc));
        }

        private static readonly Expression<Func<string, string>> ArrayOrListErrorMessageFunc =
            name => $"{name} must be type of Array or List";

        private static readonly Expression<Func<string, string>> AtLeastElementErrorMessageFunc =
            name => $"{name} must contain more than one element";
    }
}