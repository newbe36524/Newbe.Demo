using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Interfaces;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public class EnumRangePropertyValidatorFactory : IPropertyValidatorFactory
    {
        public IEnumerable<Expression> CreateExpression(CreatePropertyValidatorInput input)
        {
            var type = input.PropertyInfo.PropertyType;
            if (type.BaseType != typeof(Enum))
            {
                yield break;
            }

            var enumRangeAttribute = input.PropertyInfo.GetCustomAttribute<EnumRangeAttribute>();
            if (enumRangeAttribute == null)
            {
                yield break;
            }

            var enumValues = Enum.GetValues(type).Cast<object>().ToList();

            var checkbox = CreateValidateExpression(type, enumValues);

            Expression ErrorMessageFuncFactory(Expression inputExp)
            {
                var nameExp = Expression.Parameter(typeof(string), "name");

                var tempExp = Expression.Constant("{0} must be {1} but found {2}.");
                var enumRangeString = string.Join(",", enumValues.Cast<Enum>().Select(x => x.ToString("D")));
                var bodyExp = Expression.Call(typeof(string),
                    nameof(string.Format),
                    Array.Empty<Type>(),
                    tempExp,
                    Expression.Convert(nameExp, typeof(object)),
                    Expression.Convert(Expression.Constant(enumRangeString), typeof(object)),
                    Expression.Convert(Expression.Property(inputExp, input.PropertyInfo), typeof(object)));
                var errorMessageFunc = Expression.Lambda<Func<string, string>>(bodyExp, nameExp);
                return errorMessageFunc;
            }

            yield return ExpressionHelper.CreateValidateExpression(input,
                ExpressionHelper.CreateCheckerExpression(type, checkbox, ErrorMessageFuncFactory));
        }

        private static Expression CreateValidateExpression(Type type, List<object> enumValues)
        {
            var pExp = Expression.Parameter(type, "value");
            var enumValuesExp = Expression.Constant(enumValues);
            var bodyExp =
                Expression.Call(enumValuesExp,
                    nameof(List<int>.Contains),
                    Array.Empty<Type>(),
                    Expression.Convert(pExp, typeof(object)));

            var funcType = Expression.GetFuncType(type, typeof(bool));
            var checkbox = Expression.Lambda(funcType, Expression.IsFalse(bodyExp), pExp);
            return checkbox;
        }
    }
}