using System;
using System.Collections.Generic;

namespace Newbe.ExpressionsTests
{
    public class TypeRegistrationItem
    {
        public Type ImplType { get; set; } = null!;
        public MyMedata Medata { get; set; }
        public Type TargetType { get; set; }

        protected bool Equals(TypeRegistrationItem other)
        {
            return ImplType.Equals(other.ImplType) && TargetType.Equals(other.TargetType);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((TypeRegistrationItem) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ImplType, TargetType);
        }
    }

    public class MyContainerBuilder : IMyContainerBuilder
    {
        /// <summary>
        /// targetType:[impls]
        /// </summary> 
        private readonly Dictionary<Type, HashSet<TypeRegistrationItem>> _dictionary =
            new Dictionary<Type, HashSet<TypeRegistrationItem>>();

        public void Register<TImpl, TInterface>(object? key = null)
        {
            var interfaceType = typeof(TInterface);
            if (!_dictionary.TryGetValue(interfaceType, out var set))
            {
                set = new HashSet<TypeRegistrationItem>();
            }

            var implType = typeof(TImpl);
            set.Add(new TypeRegistrationItem
            {
                ImplType = implType,
                TargetType = interfaceType,
                Medata = new MyMedata
                {
                    Key = key,
                    ImplType = implType
                }
            });
            _dictionary[interfaceType] = set;
        }

        public IObjectFactory Build()
        {
            var simpleObjectFactoryHandler = new SimpleObjectFactoryHandler {TypeMapping = _dictionary};
            var objectFactory = new ObjectFactory(new[]
            {
                simpleObjectFactoryHandler
            });
            simpleObjectFactoryHandler.InitFunc();
            return objectFactory;
        }
    }
}