using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autofac;
using FluentAssertions;
using Newbe.ExpressionsTests.Interfaces;
using Newbe.ExpressionsTests.Model;
using NUnit.Framework;

// ReSharper disable InvalidXmlDocComment

namespace Newbe.ExpressionsTests
{
    /// <summary>
    /// Final
    /// </summary>
    public class X03PropertyValidationTest10
    {
        private const int Count = 10_000;

        private IValidatorFactory _factory = null!;

        [SetUp]
        public void Init()
        {
            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<ValidatorModule>();
                var container = builder.Build();
                _factory = container.Resolve<IValidatorFactory>();
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
                    var input = new CreateClaptrapInput
                    {
                        NickName = "newbe36524"
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("missing Name");
                }

                // test 2
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "1",
                        NickName = "newbe36524"
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("Length of Name should be great than 3");
                }

                // test 3
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo",
                        NickName = "newbe36524"
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeTrue();
                    errorMessage.Should().BeNullOrEmpty();
                }

                // test 4
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo is the only one dalao",
                        NickName = "newbe36524"
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("Length of Name should be less than 10");
                }

                // test 5
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo",
                        NickName = "newbe36524",
                        Age = -100
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be($"Value of Age should be in [0,{int.MaxValue}]");
                }

                // test 6
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo",
                        NickName = "newbe36524",
                        Levels = Array.Empty<int>()
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("Levels must contains more than one element");
                }

                // test 7
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo",
                        NickName = "newbe36524",
                        List = new List<string>()
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("List must contains more than one element");
                }

                // test 8
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo",
                        NickName = "newbe36524",
                        Items = Enumerable.Range(0, 10)
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("Items must be type of Array or List");
                }
            }
        }

        [Test]
        public void Test()
        {
            IEnumerable<int> student = Enumerable.Range(0, 10);
            (student is ICollection).Should().BeFalse();
            (new List<int>() is ICollection).Should().BeTrue();
            (Array.Empty<int>() is ICollection).Should().BeTrue();
        }

        public ValidateResult Validate(CreateClaptrapInput input)
        {
            Debug.Assert(_factory != null, nameof(_factory) + " != null");

            var validator = _factory.GetValidator(typeof(CreateClaptrapInput));
            return validator.Invoke(input);
        }
    }
}