using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Newbe.ExpressionsTests.Model
{
    //typeof(CreateClaptrapInput).Property(x=>x.Name).Required().CreateValidator();
    public class CreateClaptrapInput
    {
        [Required]
        [MinLength(3)]
        [MaxLength(10)]
        public string Name { get; set; }

        [Required] [MinLength(3)] public string NickName { get; set; }
        [Range(0, int.MaxValue)] public int Age { get; set; } = 0;

        public int[] Levels { get; set; } = {1};
        public List<string> List { get; set; } = new List<string> {"123"};
        public IEnumerable<int> Items { get; set; } = new List<int> {123};

        [Required] public int? Size { get; set; } = 1;

        [GreatThan(Name = nameof(Age))] public int Height { get; set; } = 1;

        [EnumRange] public ActionType ActionType { get; set; } = ActionType.Add;

        [EqualTo(Name = nameof(OldPwd))]
        public string NewPwd { get; set; }
        public string OldPwd { get; set; }

        public CreateClaptrapInput InnerInput { get; set; }
    }

    public class EqualToAttribute : Attribute
    {
        public string Name { get; set; }
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

    public class GreatThanAttribute : Attribute
    {
        public string Name { get; set; }
    }
}