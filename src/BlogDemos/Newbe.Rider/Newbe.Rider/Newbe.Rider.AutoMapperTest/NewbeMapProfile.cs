using System;
using AutoMapper;

namespace Newbe.Rider.AutoMapperTest
{
    public class NewbeMapProfile : Profile
    {
        public NewbeMapProfile()
        {
            CreateMap<StudentEntity, StudentVm>()
                .ForMember(a => a.Id, b => b.MapFrom(c => c.Id))
                .ForMember(a => a.Name, b => b.MapFrom(c => c.Name))
                .ForMember(a => a.Age, b => b.MapFrom(c => c.Age))
                .ForMember(a => a.Birthday, b => b.MapFrom(c => c.Birthday))
                ;
        }
    }

    public class StudentEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
    }

    public class StudentVm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
    }
}