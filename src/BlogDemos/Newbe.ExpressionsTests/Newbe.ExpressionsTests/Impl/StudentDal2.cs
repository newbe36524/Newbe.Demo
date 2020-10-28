using System.Collections.Generic;
using Newbe.ExpressionsTests.Interfaces;

namespace Newbe.ExpressionsTests.Impl
{
    public class StudentDal2 : IStudentDal
    {
        private readonly IList<Student> _studentList = new List<Student>
        {
            new Student
            {
                Id = "11",
                Name = "月落"
            }
        };

        public IEnumerable<Student> GetStudents()
        {
            return _studentList;
        }
    }
}