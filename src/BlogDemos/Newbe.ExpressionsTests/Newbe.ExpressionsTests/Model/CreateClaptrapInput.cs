using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Newbe.ExpressionsTests.Model
{
    public class CreateClaptrapInput
    {
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        public string Name { get; set; }

        [Required] [MinLength(3)] public string NickName { get; set; }
        [Range(0, int.MaxValue)] public int Age { get; set; }

        public int[] Levels { get; set; } = {1};
        public List<string> List { get; set; } = new List<string> {"123"};
        public IEnumerable<int> Items { get; set; } = new List<int> {123};

        [Required] public int? Size { get; set; } = 1;

        [EnumRange] public ActionType ActionType { get; set; } = ActionType.Add;
    }

    public enum ActionType
    {
        Add = 1,
        Update = 2,
        Delete = 3,
    }

    public class EnumRangeAttribute : Attribute
    {
    }
}