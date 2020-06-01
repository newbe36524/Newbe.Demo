using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.RxWorld
{
    public class EnumerableTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EnumerableTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void EnumerableNormalPath()
        {
            var students = GetStudents();
            var studentFullInfos = students
                .Select(student => new
                {
                    student, @class = GetClassByStudentId(student.Id),
                })
                .ToArray();
            foreach (var info in studentFullInfos)
            {
                _testOutputHelper.WriteLine($"student {info.student.Name} is in {info.@class.ClassName}");
            }
        }

        [Fact]
        public void WorkAsObservable()
        {
            GetStudents()
                .ToObservable()
                .Select(student => new
                {
                    student, @class = GetClassByStudentId(student.Id),
                })
                .Subscribe(info =>
                {
                    _testOutputHelper.WriteLine($"student {info.student.Name} is in {info.@class.ClassName}");
                });
        }

        [Fact]
        public void EnumerableWithFunctions()
        {
            var students = GetStudents();
            var studentFullInfos = students
                .Select(GetStudentFullInfo)
                .ToArray();
            foreach (var info in studentFullInfos)
            {
                ShowStudentInfo(info);
            }
        }

        [Fact]
        public void ObservableWithFunctions()
        {
            GetStudents()
                .ToObservable()
                .Select(GetStudentFullInfo)
                .Subscribe(ShowStudentInfo);
        }

        [Fact]
        public void EnumerableWithFunctions2()
        {
            GetStudents()
                .Select(GetStudentFullInfo)
                .ToList()
                .ForEach(ShowStudentInfo);
        }

        [Fact]
        public void ObservableWithLocalFunctions()
        {
            GetStudents()
                .ToObservable()
                .Select(GetStudentFullInfo)
                .Subscribe(ShowStudentInfo);

            static (Student student, Class @class) GetStudentFullInfo(Student student)
                => (student, GetClassByStudentId(student.Id));

            void ShowStudentInfo((Student student, Class @class) info)
                => _testOutputHelper.WriteLine($"student {info.student.Name} is in {info.@class.ClassName}");
        }

        private static (Student student, Class @class) GetStudentFullInfo(Student student)
            => (student, GetClassByStudentId(student.Id));

        private void ShowStudentInfo((Student student, Class @class) info)
            => _testOutputHelper.WriteLine($"student {info.student.Name} is in {info.@class.ClassName}");

        private static IEnumerable<Student> GetStudents()
        {
            yield return new Student
            {
                Id = "s1",
                Name = "student1",
                ClassId = "c1"
            };
            yield return new Student
            {
                Id = "s2",
                Name = "student2",
                ClassId = "c2",
            };
        }

        private static Class GetClassByStudentId(string studentId)
        {
            return studentId switch
            {
                "s1" => new Class {ClassId = "s1", ClassName = "class1"},
                "s2" => new Class {ClassId = "s2", ClassName = "class2"},
                _ => null
            };
        }

        public class Student
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string ClassId { get; set; }
        }

        public class Class
        {
            public string ClassId { get; set; }
            public string ClassName { get; set; }
        }
    }
}