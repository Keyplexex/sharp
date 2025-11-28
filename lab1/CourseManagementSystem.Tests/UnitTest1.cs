using System;
using System.Linq;
using Xunit;

namespace CourseManagementSystem.Tests
{
    public class CourseManagerTests
    {
        [Fact]
        public void AddCourse_ShouldAddCourseSuccessfully()
        {
            var manager = new CourseManager();
            var course = new OnlineCourse("C001", "Test Course", "Description", "Platform", "Link");

            manager.AddCourse(course);

            var retrievedCourse = manager.GetCourse("C001");
            Assert.NotNull(retrievedCourse);
            Assert.Equal("Test Course", retrievedCourse.Name);
        }

        [Fact]
        public void AddCourse_WithDuplicateId_ShouldThrowException()
        {
            var manager = new CourseManager();
            var course1 = new OnlineCourse("C001", "Course 1", "Description", "Platform", "Link");
            var course2 = new OnlineCourse("C001", "Course 2", "Description", "Platform", "Link");
            
            manager.AddCourse(course1);

            Assert.Throws<InvalidOperationException>(() => manager.AddCourse(course2));
        }

        [Fact]
        public void AssignTeacherToCourse_ShouldAssignTeacher()
        {
            var manager = new CourseManager();
            var teacher = new Teacher("T001", "John Doe", "john@test.com", "CS");
            var course = new OnlineCourse("C001", "Test Course", "Description", "Platform", "Link");
            
            manager.AddTeacher(teacher);
            manager.AddCourse(course);

            var result = manager.AssignTeacherToCourse("T001", "C001");

            Assert.True(result);
            var assignedCourse = manager.GetCourse("C001");
            Assert.NotNull(assignedCourse?.Teacher);
            Assert.Equal("John Doe", assignedCourse.Teacher.Name);
        }

        [Fact]
        public void AssignTeacherToCourse_WithInvalidIds_ShouldReturnFalse()
        {
            var manager = new CourseManager();

            var result = manager.AssignTeacherToCourse("INVALID_TEACHER", "INVALID_COURSE");

            Assert.False(result);
        }

        [Fact]
        public void EnrollStudentInCourse_ShouldEnrollStudent()
        {
            var manager = new CourseManager();
            var student = new Student("S001", "Alice", "alice@test.com", 2);
            var course = new OnlineCourse("C001", "Test Course", "Description", "Platform", "Link");
            
            manager.AddStudent(student);
            manager.AddCourse(course);

            var result = manager.EnrollStudentInCourse("S001", "C001");

            Assert.True(result);
            Assert.Single(manager.GetStudentsInCourse("C001"));
            Assert.Equal("Alice", manager.GetStudentsInCourse("C001")[0].Name);
        }

        [Fact]
        public void GetCoursesByTeacher_ShouldReturnCorrectCourses()
        {
            var manager = new CourseManager();
            var teacher = new Teacher("T001", "John Doe", "john@test.com", "CS");
            var course1 = new OnlineCourse("C001", "Course 1", "Desc", "Platform", "Link");
            var course2 = new OnlineCourse("C002", "Course 2", "Desc", "Platform", "Link");
            
            manager.AddTeacher(teacher);
            manager.AddCourse(course1);
            manager.AddCourse(course2);
            manager.AssignTeacherToCourse("T001", "C001");

            var courses = manager.GetCoursesByTeacher("T001");

            Assert.Single(courses);
            Assert.Equal("Course 1", courses[0].Name);
        }

        [Fact]
        public void GetCoursesByStudent_ShouldReturnCorrectCourses()
        {
            var manager = new CourseManager();
            var student = new Student("S001", "Alice", "alice@test.com", 2);
            var course1 = new OnlineCourse("C001", "Course 1", "Desc", "Platform", "Link");
            var course2 = new OnlineCourse("C002", "Course 2", "Desc", "Platform", "Link");
            
            manager.AddStudent(student);
            manager.AddCourse(course1);
            manager.AddCourse(course2);
            manager.EnrollStudentInCourse("S001", "C001");
            manager.EnrollStudentInCourse("S001", "C002");

            var courses = manager.GetCoursesByStudent("S001");

            Assert.Equal(2, courses.Count);
            Assert.Contains(courses, c => c.Name == "Course 1");
            Assert.Contains(courses, c => c.Name == "Course 2");
        }

        [Fact]
        public void RemoveCourse_ShouldRemoveCourseSuccessfully()
        {
            var manager = new CourseManager();
            var course = new OnlineCourse("C001", "Test Course", "Description", "Platform", "Link");
            manager.AddCourse(course);

            var result = manager.RemoveCourse("C001");

            Assert.True(result);
            Assert.Null(manager.GetCourse("C001"));
        }

        [Fact]
        public void RemoveCourse_WithInvalidId_ShouldReturnFalse()
        {
            var manager = new CourseManager();

            var result = manager.RemoveCourse("INVALID_ID");

            Assert.False(result);
        }

        [Fact]
        public void OnlineCourse_GetInfo_ShouldReturnCorrectInformation()
        {
            var course = new OnlineCourse("C001", "C# Course", "Learn C#", "Zoom", "https://zoom.com/meeting");

            var info = course.GetInfo();

            Assert.Contains("C# Course", info);
            Assert.Contains("Online", info);
            Assert.Contains("Zoom", info);
        }

        [Fact]
        public void OfflineCourse_GetInfo_ShouldReturnCorrectInformation()
        {
            var course = new OfflineCourse("C002", "Math", "Mathematics", "Room 101", "Building A", "Mon/Wed 10:00");

            var info = course.GetInfo();

            Assert.Contains("Math", info);
            Assert.Contains("Offline", info);
            Assert.Contains("Room 101", info);
            Assert.Contains("Building A", info);
        }
    }
}