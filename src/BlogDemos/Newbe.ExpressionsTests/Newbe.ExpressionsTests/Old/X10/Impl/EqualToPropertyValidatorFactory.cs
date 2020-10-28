using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Old.X10.Model;

namespace Newbe.ExpressionsTests.Old.X10.Impl
{
    public class EqualToPropertyValidatorFactory : PropertyValidatorFactoryBase<string>
    {
        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var equalToAttribute = input.PropertyInfo.GetCustomAttribute<EqualToAttribute>();
            if (equalToAttribute == null)
            {
                yield break;
            }

            Expression CheckFuncFactory(Expression inputExp)
            {
                var valueExp = Expression.Parameter(input.PropertyInfo.PropertyType, "value");
                var rightExp = Expression.Property(inputExp, equalToAttribute.Name);
                var bodyExp = Expression.NotEqual(valueExp, rightExp);
                var funcType = Expression.GetFuncType(input.PropertyInfo.PropertyType, typeof(bool));
                var checkFuncExp = Expression.Lambda(funcType, bodyExp, valueExp);
                return checkFuncExp;
                // x.NewPwd != x.OldPwd
            }

            Expression ErrorMessageFuncFactory(Expression inputExp)
            {
                // "NewPwd must be the same as OldPwd"

                var nameExp = Expression.Parameter(typeof(string), "name");
                var bodyExp = Expression.Call(typeof(string),
                    nameof(string.Format),
                    Array.Empty<Type>(),
                    Expression.Constant("{0} must be the same as {1}"),
                    Expression.Convert(nameExp, typeof(object)),
                    Expression.Convert(Expression.Constant(equalToAttribute.Name), typeof(object)));
                var errorMessageFuncExp = Expression.Lambda<Func<string, string>>(bodyExp, nameExp);
                return errorMessageFuncExp;
            }

            yield return ExpressionHelper.CreateValidateExpression(input,
                ExpressionHelper.CreateCheckerExpression(typeof(string), CheckFuncFactory, ErrorMessageFuncFactory));
        }
    }
}