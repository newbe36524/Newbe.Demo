using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

// ReSharper disable InvalidXmlDocComment

namespace Newbe.ExpressionsTests.Old
{
    /// <summary>
    /// Property Expression
    /// </summary>
    public class X03PropertyValidationTest03
    {
        private const int Count = 10_000;

        private static Func<CreateClaptrapInput, int, ValidateResult> _func;

        [SetUp]
        public void Init()
        {
            try
            {
                var finalExpression = CreateCore();
                _func = finalExpression.Compile();

                Expression<Func<CreateClaptrapInput, int, ValidateResult>> CreateCore()
                {
                    // exp for input
                    var inputExp = Expression.Parameter(typeof(CreateClaptrapInput), "input");
                    var nameProp = typeof(CreateClaptrapInput).GetProperty(nameof(CreateClaptrapInput.Name));
                    Debug.Assert(nameProp != null, nameof(nameProp) + " != null");
                    var namePropExp = Expression.Property(inputExp, nameProp);
                    var nameNameExp = Expression.Constant(nameProp.Name);
                    var minLengthPExp = Expression.Parameter(typeof(int), "minLength");

                    // exp for output
                    var resultExp = Expression.Variable(typeof(ValidateResult), "result");

                    // exp for return statement
                    var returnLabel = Expression.Label(typeof(ValidateResult));

                    // build whole block
                    var body = Expression.Block(
                        new[] {resultExp},
                        CreateDefaultResult(),
                        CreateValidateNameRequiredExpression(),
                        CreateValidateNameMinLengthExpression(),
                        Expression.Label(returnLabel, resultExp));

                    // build lambda from body
                    var final = Expression.Lambda<Func<CreateClaptrapInput, int, ValidateResult>>(
                        body,
                        inputExp,
                        minLengthPExp);
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

                    Expression CreateValidateNameRequiredExpression()
                    {
                        var requireMethod = typeof(X03PropertyValidationTest03).GetMethod(nameof(ValidateStringRequired));
                        var isOkProperty = typeof(ValidateResult).GetProperty(nameof(ValidateResult.IsOk));
                        Debug.Assert(requireMethod != null, nameof(requireMethod) + " != null");
                        Debug.Assert(isOkProperty != null, nameof(isOkProperty) + " != null");

                        var requiredMethodExp = Expression.Call(requireMethod, nameNameExp, namePropExp);
                        var assignExp = Expression.Assign(resultExp, requiredMethodExp);
                        var resultIsOkPropertyExp = Expression.Property(resultExp, isOkProperty);
                        var conditionExp = Expression.IsFalse(resultIsOkPropertyExp);
                        var ifThenExp =
                            Expression.IfThen(conditionExp,
                                Expression.Return(returnLabel, resultExp));
                        var re = Expression.Block(
                            new[] {resultExp},
                            assignExp,
                            ifThenExp);
                        /**
                         * final as:
                         * result = ValidateNameRequired("Name", input.Name);
                         * if (!result.IsOk)
                         * {
                         *     return result;
                         * }
                         */
                        return re;
                    }

                    Expression CreateValidateNameMinLengthExpression()
                    {
                        var minLengthMethod =
                            typeof(X03PropertyValidationTest03).GetMethod(nameof(ValidateStringMinLength));
                        var isOkProperty = typeof(ValidateResult).GetProperty(nameof(ValidateResult.IsOk));
                        Debug.Assert(minLengthMethod != null, nameof(minLengthMethod) + " != null");
                        Debug.Assert(isOkProperty != null, nameof(isOkProperty) + " != null");

                        var requiredMethodExp = Expression.Call(minLengthMethod,
                            nameNameExp,
                            namePropExp,
                            minLengthPExp);
                        var assignExp = Expression.Assign(resultExp, requiredMethodExp);
                        var resultIsOkPropertyExp = Expression.Property(resultExp, isOkProperty);
                        var conditionExp = Expression.IsFalse(resultIsOkPropertyExp);
                        var ifThenExp =
                            Expression.IfThen(conditionExp,
                                Expression.Return(returnLabel, resultExp));
                        var re = Expression.Block(
                            new[] {resultExp},
                            assignExp,
                            ifThenExp);
                        /**
                        * final as:
                        * result = ValidateNameMinLength("Name", input.Name, minLength);
                        * if (!result.IsOk)
                        * {
                        *     return result;
                        * }
                        */
                        return re;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [Test]
        public void Run()
        {
            for (int i = 0; i < Count; i++)
            {
                // test 1
                {
                    var input = new CreateClaptrapInput();
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("missing Name");
                }

                // test 2
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "1"
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("Length of Name should be great than 3");
                }

                // test 3
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo is the only one dalao"
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeTrue();
                    errorMessage.Should().BeNullOrEmpty();
                }
            }
        }

        public static ValidateResult Validate(CreateClaptrapInput input)
        {
            return _func.Invoke(input, 3);
        }

        public static ValidateResult ValidateStringRequired(string name, string value)
        {
            return string.IsNullOrEmpty(value)
                ? ValidateResult.Error($"missing {name}")
                : ValidateResult.Ok();
        }

        public static ValidateResult ValidateStringMinLength(string name, string value, int minLength)
        {
            return value.Length < minLength
                ? ValidateResult.Error($"Length of {name} should be great than {minLength}")
                : ValidateResult.Ok();
        }


        public class CreateClaptrapInput
        {
            [Required] [MinLength(3)] public string Name { get; set; }
        }

        public struct ValidateResult
        {
            public bool IsOk { get; set; }
            public string ErrorMessage { get; set; }

            public void Deconstruct(out bool isOk, out string errorMessage)
            {
                isOk = IsOk;
                errorMessage = ErrorMessage;
            }

            public static ValidateResult Ok()
            {
                return new ValidateResult
                {
                    IsOk = true
                };
            }

            public static ValidateResult Error(string errorMessage)
            {
                return new ValidateResult
                {
                    IsOk = false,
                    ErrorMessage = errorMessage
                };
            }
        }
    }
}