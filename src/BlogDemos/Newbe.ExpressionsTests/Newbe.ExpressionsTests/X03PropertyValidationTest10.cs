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
                    errorMessage.Should().Be("Levels must contain more than one element");
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
                    errorMessage.Should().Be("List must contain more than one element");
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

                // test 9
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo",
                        NickName = "newbe36524",
                        Size = null
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("Size must be not null");
                }

                // test 10
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo",
                        NickName = "newbe36524",
                        ActionType = (ActionType) 666,
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("ActionType must be 1,2,3 but found 666.");
                }

                // test 11
                {
                    var input = new CreateClaptrapInput
                    {
                        Name = "yueluo",
                        NickName = "newbe36524",
                        Height = -1
                    };
                    var (isOk, errorMessage) = Validate(input);
                    isOk.Should().BeFalse();
                    errorMessage.Should().Be("Value of Height must be greater than Age. But found: Height: -1, Age: 0");
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

        [Test]
        public void TestEnumEqual()
        {
            (typeof(Enum) == typeof(ActionType)).Should().BeFalse();
            typeof(ActionType).BaseType.Should().Be(typeof(Enum));
        }

        [Test]
        public void TestEnum()
        {
            var values = Enum.GetValues(typeof(BoolEnum))
                .Cast<BoolEnum>()
                .ToArray();
            var b = BoolEnum.Yes;
            values.Contains(b).Should().BeTrue();
            b.Should().Be(BoolEnum.Yes);
            b = (BoolEnum) 123;
            b.Should().Be(123);
            values.Contains(b).Should().BeFalse();
        }

        public enum BoolEnum
        {
            Yes = 1,
            No = 2
        }

        [Test]
        public void TestInt()
        {
            int? age = null;
            age.Should().BeNull();
        }


        [Test]
        public void TestEnumerable()
        {
            CheckType(typeof(IEnumerable<int>));
            CheckType(typeof(List<int>));
            CheckType(typeof(int[]));

            void CheckType(Type type)
            {
                var allInterfaces = GetAllInterfaceIncludingSelf();
                allInterfaces.Any(x => x.Name == "IEnumerable`1").Should().Be(true);

                IEnumerable<Type> GetAllInterfaceIncludingSelf()
                {
                    foreach (var i in type.GetInterfaces())
                    {
                        yield return i;
                    }

                    yield return type;
                }
            }
        }

        public ValidateResult Validate(CreateClaptrapInput input)
        {
            Debug.Assert(_factory != null, nameof(_factory) + " != null");

            var validator = _factory.GetValidator(typeof(CreateClaptrapInput));
            return validator.Invoke(input);
        }
    }
}