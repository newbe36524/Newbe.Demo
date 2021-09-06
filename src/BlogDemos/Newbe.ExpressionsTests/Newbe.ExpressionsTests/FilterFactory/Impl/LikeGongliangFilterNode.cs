using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Newbe.ExpressionsTests.FilterFactory.Impl
{
    public class LikeGongliangFilterNode : IFilterNode<Dictionary<string, object>>
    {
        public Expression<Func<Dictionary<string, object>, bool>> CreateExpression()
        {
            return dict => dict.ContainsKey("GongliangFans");
        }
    }
}