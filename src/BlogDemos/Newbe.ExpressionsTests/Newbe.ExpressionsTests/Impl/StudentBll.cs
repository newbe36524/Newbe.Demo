using System.Collections.Generic;
using Newbe.ExpressionsTests.Interfaces;

namespace Newbe.ExpressionsTests.Impl
{
    public class StudentBll : IStudentBll
    {
        private readonly IStudentDal _studentDal;

        /**
             * 通过构造函数传入一个 IStudentDal 这种方式称为“构造函数注入”
             * 使用构造函数注入的方式，使得不依赖于特定的 IStudentDal 实现。
             * 只要 IStudentDal 接口的定义不修改，该类就不需要修改，实现了DAL与BLL的解耦
             */
        public StudentBll(
            IStudentDal studentDal)
        {
            _studentDal = studentDal;
        }

        public IEnumerable<Student> GetStudents()
        {
            var re = _studentDal.GetStudents();
            return re;
        }
    }
}