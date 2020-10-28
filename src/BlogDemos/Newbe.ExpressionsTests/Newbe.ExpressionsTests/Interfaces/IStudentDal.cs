using System.Collections.Generic;

namespace Newbe.ExpressionsTests.Interfaces
{
    public interface IStudentDal
    {
        IEnumerable<Student> GetStudents();
    }
}