using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="validateFuncExpression">Expression{Func{string,TValue,ValidateResult}}</param>
        /// <returns></returns>
        public static Expression CreateValidateExpression(
            CreatePropertyValidatorInput input,
            Expression validateFuncExpression)
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