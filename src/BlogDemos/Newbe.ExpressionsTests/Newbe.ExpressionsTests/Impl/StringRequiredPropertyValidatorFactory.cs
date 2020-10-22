using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public class StringRequiredPropertyValidatorFactory : PropertyValidatorFactoryBase<string>
    {
        private static Expression<Func<string, string, ValidateResult>> CreateValidateStringRequiredExp()
        {
            var nameExp = Expression.Parameter(typeof(string), "name");
            var valueExp = Expression.Parameter(typeof(string), "value");
            var checkBodyExp = Expression.Call(typeof(string),
                nameof(string.IsNullOrEmpty),
                Array.Empty<Type>(),
                valueExp);
            var errorMessageExp = Expression.Call(typeof(string),
                nameof(string.Format),
                Array.Empty<Type>(),
                Expression.Constant("missing {0}"),
                nameExp);
            var errorResultExp = Expression.Call(typeof(ValidateResult),
                nameof(ValidateResult.Error),
                Array.Empty<Type>(),
                errorMessageExp);
            var okResultExp = Expression.Call(typeof(ValidateResult),
                nameof(ValidateResult.Ok),
                Array.Empty<Type>());

            var resultExp = Expression.Variable(typeof(ValidateResult), "result");
            var body1Exp = Expression.IfThenElse(checkBodyExp,
                Expression.Assign(resultExp, errorResultExp),
                Expression.Assign(resultExp, okResultExp));
            var bodyExp = Expression.Block(new[] {resultExp}, body1Exp, resultExp);
            var finalExp = Expression.Lambda<Func<string, string, ValidateResult>>(bodyExp, nameExp, valueExp);
            return finalExp;
        }

        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var propertyInfo = input.PropertyInfo;
            if (propertyInfo.GetCustomAttribute<RequiredAttribute>() != null)
            {
                yield return CreateValidateExpression(input, CreateValidateStringRequiredExp());
            }
        }
    }
}