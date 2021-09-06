using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Newbe.ExpressionsTests.FilterFactory.Impl
{
    public class IsDalaoFilterNode : IFilterNode<Dictionary<string, object>>
    {
        public Expression<Func<Dictionary<string, object>, bool>> CreateExpression()
        {
            return dict => dict.ContainsKey("Name") && (string)dict["Name"] == "40W";
        }
    }
}