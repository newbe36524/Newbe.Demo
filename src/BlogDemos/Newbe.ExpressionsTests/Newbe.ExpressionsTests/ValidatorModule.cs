﻿using Autofac;
using Newbe.ExpressionsTests.Impl;
using Newbe.ExpressionsTests.Interfaces;

namespace Newbe.ExpressionsTests
{
    public class ValidatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ValidatorFactory>()
                .As<IValidatorFactory>()
                .SingleInstance();

            builder.RegisterType<IntRangePropertyValidatorFactory>()
                .As<IPropertyValidatorFactory>()
                .SingleInstance();
            builder.RegisterType<StringRequiredPropertyValidatorFactory>()
                .As<IPropertyValidatorFactory>()
                .SingleInstance();
            builder.RegisterType<StringLengthPropertyValidatorFactory>()
                .As<IPropertyValidatorFactory>()
                .SingleInstance();
        }
    }
}