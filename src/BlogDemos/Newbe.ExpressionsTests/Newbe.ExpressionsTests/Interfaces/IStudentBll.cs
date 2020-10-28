using System.Collections.Generic;

namespace Newbe.ExpressionsTests.Interfaces
{
    public interface IStudentBll
    {
        IEnumerable<Student> GetStudents();
    }
}