using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public class NullableIntPropertyValidatorFactory : PropertyValidatorFactoryBase<int?>
    {
        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var requiredAttribute = input.PropertyInfo.GetCustomAttribute<RequiredAttribute>();
            if (requiredAttribute == null)
            {
                yield break;
            }

            Expression<Func<int?, bool>> checkbox = value =>
                value == null;
            Expression<Func<string, string>> errorMessageFunc =
                name => $"{name} must be not null";
            yield return ExpressionHelper.CreateValidateExpression(input,
                ExpressionHelper.CreateCheckerExpression(typeof(int?), checkbox, errorMessageFunc));
        }
    }
}