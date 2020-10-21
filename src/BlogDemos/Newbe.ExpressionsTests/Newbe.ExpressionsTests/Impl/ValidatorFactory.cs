using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Newbe.ExpressionsTests.Interfaces;
using Newbe.ExpressionsTests.Model;

namespace Newbe.ExpressionsTests.Impl
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IEnumerable<IPropertyValidatorFactory> _propertyValidatorFactories;

        public ValidatorFactory(
            IEnumerable<IPropertyValidatorFactory> propertyValidatorFactories)
        {
            _propertyValidatorFactories = propertyValidatorFactories;
        }

        private Func<object, ValidateResult> CreateValidator(Type type)
        {
            var finalExpression = CreateCore();
            return finalExpression.Compile();

            Expression<Func<object, ValidateResult>> CreateCore()
            {
                // exp for input
                var inputExp = Expression.Parameter(typeof(object), "input");

                // exp for output
                var resultExp = Expression.Variable(typeof(ValidateResult), "result");

                // exp for return statement
                var returnLabel = Expression.Label(typeof(ValidateResult));

                var innerExps = new List<Expression> {CreateDefaultResult()};

                var validateExpressions = type.GetProperties()
                    .SelectMany(p => _propertyValidatorFactories
                        .SelectMany(f =>
                            f.CreateExpression(new CreatePropertyValidatorInput
                            {
                                InputExpression = inputExp,
                                PropertyInfo = p,
                                ResultExpression = resultExp,
                                ReturnLabel = returnLabel,
                                InputType = type,
                            })))
                    .ToArray();
                innerExps.AddRange(validateExpressions);

                innerExps.Add(Expression.Label(returnLabel, resultExp));

                // build whole block
                var body = Expression.Block(
                    new[] {resultExp},
                    innerExps);

                // build lambda from body
                var final = Expression.Lambda<Func<object, ValidateResult>>(
                    body,
                    inputExp);
                return final;

                Expression CreateDefaultResult()
                {
                    var okMethod = typeof(ValidateResult).GetMethod(nameof(ValidateResult.Ok));
                    Debug.Assert(okMethod != null, nameof(okMethod) + " != null");
                    var methodCallExpression = Expression.Call(okMethod);
                    var re = Expression.Assign(resultExp, methodCallExpression);
                    /**
                     * final as:
                     * result = ValidateResult.Ok()
                     */
                    return re;
                }
            }
        }

        private static readonly ConcurrentDictionary<Type, Func<object, ValidateResult>> ValidateFunc =
            new ConcurrentDictionary<Type, Func<object, ValidateResult>>();

        public Func<object, ValidateResult> GetValidator(Type type)
        {
            var re = ValidateFunc.GetOrAdd(type, CreateValidator);
            return re;
        }
    }
}