using System;
using System.Collections.Generic;

namespace Newbe.ExpressionsTests
{
    public class TreeNode
    {
        public Type InterfaceType { get; set; }
        public Type ImplType { get; set; }
        public TreeNode? Parent { get; set; }
        public List<TreeNode> Children { get; set; }
    }
}