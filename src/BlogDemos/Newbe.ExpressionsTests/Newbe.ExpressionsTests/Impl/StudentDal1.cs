using System.Collections.Generic;
using Newbe.ExpressionsTests.Interfaces;

namespace Newbe.ExpressionsTests.Impl
{
    public class StudentDal1 : IStudentDal
    {
        private readonly IList<Student> _studentList = new List<Student>
        {
            new Student
            {
                Id = "12",
                Name = "Traceless",
            }
        };

        public IEnumerable<Student> GetStudents()
        {
            return _studentList;
        }
    }
}