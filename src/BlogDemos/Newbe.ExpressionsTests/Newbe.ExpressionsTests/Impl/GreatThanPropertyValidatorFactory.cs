using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public class GreatThanPropertyValidatorFactory : PropertyValidatorFactoryBase<int>
    {
        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var propertyInfo = input.PropertyInfo;
            var greatThanAttribute = propertyInfo.GetCustomAttribute<GreatThanAttribute>();
            if (greatThanAttribute == null)
            {
                yield break;
            }

            Expression CheckFuncFactory(Expression inputExp)
            {
                var rightExp = Expression.Property(inputExp, greatThanAttribute.Name);
                var valueExp = Expression.Parameter(typeof(int), "value");

                var bodyExp = Expression.LessThanOrEqual(valueExp, rightExp);
                var checkFuncExp = Expression.Lambda<Func<int, bool>>(bodyExp, valueExp);
                return checkFuncExp;
            }

            Expression ErrorMessageFuncFactory(Expression inputExp)
            {
                var nameExp = Expression.Parameter(typeof(string), "name");

                var stringFormatArrayExp = Expression.NewArrayInit(
                    typeof(object),
                    Expression.Convert(nameExp, typeof(object)),
                    Expression.Convert(Expression.Constant(greatThanAttribute.Name), typeof(object)),
                    Expression.Convert(Expression.Property(inputExp, propertyInfo), typeof(object)),
                    Expression.Convert(Expression.Property(inputExp, greatThanAttribute.Name), typeof(object)));
                var formatExp =
                    Expression.Constant("Value of {0} must be greater than {1}. But found: {0}: {2}, {1}: {3}");
                var bodyExp = Expression.Call(StringFormatMethod, formatExp, stringFormatArrayExp);

                var checkFuncExp = Expression.Lambda<Func<string, string>>(bodyExp, nameExp);
                return checkFuncExp;
            }

            yield return ExpressionHelper.CreateValidateExpression(input,
                ExpressionHelper.CreateCheckerExpression(typeof(int), CheckFuncFactory, ErrorMessageFuncFactory));
        }

        private static readonly MethodInfo StringFormatMethod = typeof(string).GetMethods().First(x =>
            x.Name == nameof(string.Format) && x.GetParameters().Length == 2 &&
            x.GetParameters()[0].ParameterType == typeof(string) &&
            x.GetParameters()[1].ParameterType == typeof(object[]));
    }
}