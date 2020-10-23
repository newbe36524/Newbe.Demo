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
        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var propertyInfo = input.PropertyInfo;
            if (propertyInfo.GetCustomAttribute<RequiredAttribute>() != null)
            {
                Expression<Func<string, bool>> checkbox = value => string.IsNullOrEmpty(value);
                Expression<Func<string, string>> errorMessageFunc = name => $"missing {name}";
                yield return ExpressionHelper.CreateValidateExpression(input,
                    ExpressionHelper.CreateCheckerExpression(typeof(string), checkbox, errorMessageFunc));
            }
        }
    }
}