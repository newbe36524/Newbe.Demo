using Autofac;
using Newbe.ExpressionsTests.Old.X10.Impl;
using Newbe.ExpressionsTests.Old.X10.Interfaces;

namespace Newbe.ExpressionsTests.Old.X10
{
    public class ValidatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ValidatorFactory>()
                .As<IValidatorFactory>()
                .SingleInstance();
            
            builder.RegisterType<EqualToPropertyValidatorFactory>()
                .As<IPropertyValidatorFactory>()
                .SingleInstance();
            builder.RegisterType<GreatThanPropertyValidatorFactory>()
                .As<IPropertyValidatorFactory>()
                .SingleInstance();
            builder.RegisterType<EnumRangePropertyValidatorFactory>()
                .As<IPropertyValidatorFactory>()
                .SingleInstance();
            builder.RegisterType<NullableIntPropertyValidatorFactory>()
                .As<IPropertyValidatorFactory>()
                .SingleInstance();
            builder.RegisterType<EnumerablePropertyValidatorFactory>()
                .As<IPropertyValidatorFactory>()
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