using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public class StringLengthPropertyValidatorFactory : PropertyValidatorFactoryBase<string>
    {
        private static Expression<Func<string, string, ValidateResult>> CreateValidateStringMinLengthExp(
            int minLength)
        {
            return (name, value) =>
                string.IsNullOrEmpty(value) || value.Length < minLength
                    ? ValidateResult.Error($"Length of {name} should be great than {minLength}")
                    : ValidateResult.Ok();
        }

        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var propertyInfo = input.PropertyInfo;
            var minlengthAttribute = propertyInfo.GetCustomAttribute<MinLengthAttribute>();
            if (minlengthAttribute != null)
            {
                yield return CreateValidateExpression(input,
                    CreateValidateStringMinLengthExp(minlengthAttribute.Length));
            }
        }
    }
}