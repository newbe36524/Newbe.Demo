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

        public static Expression CreateCheckerExpression(Type valueType,
            Expression checkBodyFunc,
            Expression errorMessageFunc)
        {
            var nameExp = Expression.Parameter(typeof(string), "name");
            var valueExp = Expression.Parameter(valueType, "value");

            var checkBodyExp = Expression.Invoke(checkBodyFunc, valueExp);

            var errorMessageExp =
                errorMessageFunc is Expression<Func<string, string>>
                    ? Expression.Invoke(errorMessageFunc, nameExp)
                    : Expression.Invoke(errorMessageFunc, nameExp, valueExp);
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
            var funcType = Expression.GetFuncType(typeof(string), valueType, typeof(ValidateResult));
            var finalExp = Expression.Lambda(funcType, bodyExp, nameExp, valueExp);
            return finalExp;
        }
    }
}