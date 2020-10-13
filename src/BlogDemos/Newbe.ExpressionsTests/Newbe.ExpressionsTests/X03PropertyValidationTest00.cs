using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using NUnit.Framework;

namespace Newbe.ExpressionsTests
{
    /// <summary>
    /// Validate data by static method
    /// </summary>
    public class X03PropertyValidationTest00
    {
        private const int Count = 10_000;

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
            return ValidateCore(input, 3);
        }

        public static ValidateResult ValidateCore(CreateClaptrapInput input, int minLength)
        {
            if (string.IsNullOrEmpty(input.Name))
            {
                return ValidateResult.Error("missing Name");
            }

            if (input.Name.Length < minLength)
            {
                return ValidateResult.Error($"Length of Name should be great than {minLength}");
            }

            return ValidateResult.Ok();
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