using System.ComponentModel.DataAnnotations;

namespace Newbe.ExpressionsTests.Model
{
    public class CreateClaptrapInput
    {
        [Required] [MinLength(3)] public string Name { get; set; }
        [Required] [MinLength(3)] public string NickName { get; set; }
    }
}