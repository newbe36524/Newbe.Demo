using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Newbe.ExpressionsTests.FilterFactory.Impl
{
    public class ValueEqFilterNode<TValue> : IFilterNode<Dictionary<string, object>>
    {
        private readonly string _name;
        private readonly TValue _value;

        public ValueEqFilterNode(string name, TValue value)
        {
            _name = name;
            _value = value;
        }

        public Expression<Func<Dictionary<string, object>, bool>> CreateExpression()
        {
            return dict => _value.Equals(dict[_name]);
        }
    }
}