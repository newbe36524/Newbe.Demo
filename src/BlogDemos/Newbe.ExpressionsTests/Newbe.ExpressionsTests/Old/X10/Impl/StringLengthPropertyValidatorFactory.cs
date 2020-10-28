using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Old.X10.Model;

namespace Newbe.ExpressionsTests.Old.X10.Impl
{
    public class StringLengthPropertyValidatorFactory : PropertyValidatorFactoryBase<string>
    {
        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var propertyInfo = input.PropertyInfo;
            var minlengthAttribute = propertyInfo.GetCustomAttribute<MinLengthAttribute>();
            if (minlengthAttribute != null)
            {
                var minLength = minlengthAttribute.Length;
                Expression<Func<string, bool>> checkbox = value =>
                    string.IsNullOrEmpty(value) || value.Length < minLength;
                Expression<Func<string, string>> errorMessageFunc =
                    name => $"Length of {name} should be great than {minLength}";
                yield return ExpressionHelper.CreateValidateExpression(input,
                    ExpressionHelper.CreateCheckerExpression(typeof(string), checkbox, errorMessageFunc));
            }

            var maxLengthAttribute = propertyInfo.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLengthAttribute != null)
            {
                var maxLength = maxLengthAttribute.Length;
                Expression<Func<string, bool>> checkbox = value =>
                    !string.IsNullOrEmpty(value) && value.Length > maxLength;
                Expression<Func<string, string>> errorMessageFunc =
                    name => $"Length of {name} should be less than {maxLength}";
                yield return ExpressionHelper.CreateValidateExpression(input,
                    ExpressionHelper.CreateCheckerExpression(typeof(string), checkbox, errorMessageFunc));
            }
        }
    }
}