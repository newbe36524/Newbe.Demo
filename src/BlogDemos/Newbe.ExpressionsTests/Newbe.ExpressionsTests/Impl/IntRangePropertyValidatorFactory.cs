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
        protected override IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input)
        {
            var propertyInfo = input.PropertyInfo;
            var rangeAttribute = propertyInfo.GetCustomAttribute<RangeAttribute>();
            if (rangeAttribute != null)
            {
                var minValue = (int) rangeAttribute.Minimum;
                var maxValue = (int) rangeAttribute.Maximum;
                Expression<Func<int, bool>> checkbox = value =>
                    value < minValue || value > maxValue;
                Expression<Func<string, string>> errorMessageFunc =
                    name => $"Value of {name} should be in [{minValue},{maxValue}]";
                yield return ExpressionHelper.CreateValidateExpression(input,
                    ExpressionHelper.CreateCheckerExpression(typeof(int), checkbox, errorMessageFunc));
            }
        }
    }
}