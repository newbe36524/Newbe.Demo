using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Newbe.ExpressionsTests.Interfaces;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public abstract class PropertyValidatorFactoryBase<TValue> : IPropertyValidatorFactory
    {
        public virtual IEnumerable<Expression> CreateExpression(CreatePropertyValidatorInput input)
        {
            if (input.PropertyInfo.PropertyType != typeof(TValue))
            {
                return Enumerable.Empty<Expression>();
            }

            var expressionCore = CreateExpressionCore(input);
            return expressionCore;
        }

        protected abstract IEnumerable<Expression> CreateExpressionCore(CreatePropertyValidatorInput input);

        protected Expression CreateValidateExpression(
            CreatePropertyValidatorInput input,
            Expression<Func<string, TValue, ValidateResult>> validateFuncExpression)
        {
            var propertyInfo = input.PropertyInfo;
            var isOkProperty = typeof(ValidateResult).GetProperty(nameof(ValidateResult.IsOk));
            Debug.Assert(isOkProperty != null, nameof(isOkProperty) + " != null");

            var convertedExp = Expression.Convert(input.InputExpression, input.InputType);
            var propExp = Expression.Property(convertedExp, propertyInfo);
            var nameExp = Expression.Constant(propertyInfo.Name);

            var requiredMethodExp = Expression.Invoke(
                validateFuncExpression,
                nameExp,
                propExp);
            var assignExp = Expression.Assign(input.ResultExpression, requiredMethodExp);
            var resultIsOkPropertyExp = Expression.Property(input.ResultExpression, isOkProperty);
            var conditionExp = Expression.IsFalse(resultIsOkPropertyExp);
            var ifThenExp =
                Expression.IfThen(conditionExp,
                    Expression.Return(input.ReturnLabel, input.ResultExpression));
            var re = Expression.Block(
                new[] {input.ResultExpression},
                assignExp,
                ifThenExp);
            return re;
        }
    }
}