using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public class IntRangePropertyValidatorFactory : PropertyValidatorFactoryBase<int>
    {
        private static Expression<Func<string, int, ValidateResult>> CreateValidateIntRangeExp(int minValue,
            int maxValue)
        {
            return (name, value) =>
                value < minValue || value > maxValue
                    ? ValidateResult.Error($"Value of {name} should be in [{minValue},{maxValue}]")
                    : ValidateResult.Ok();
        }

        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var propertyInfo = input.PropertyInfo;
            var rangeAttribute = propertyInfo.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute != null)
            {
                yield return CreateValidateExpression(input,
                    CreateValidateIntRangeExp((int) rangeAttribute.Minimum, (int) rangeAttribute.Maximum));
            }
        }
    }
}