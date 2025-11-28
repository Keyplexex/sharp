using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseManagementSystem
{
    public class CourseManager
    {
        private List<Course> courses;
        private List<Teacher> teachers;
        private List<Student> students;

        public CourseManager()
        {
            courses = new List<Course>();
            teachers = new List<Teacher>();
            students = new List<Student>();
        }

        public void AddCourse(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));
            
            if (courses.Any(c => c.CourseId == course.CourseId))
                throw new InvalidOperationException($"Course with ID {course.CourseId} already exists");
            
            courses.Add(course);
        }

        public bool RemoveCourse(string courseId)
        {
            var course = courses.FirstOrDefault(c => c.CourseId == courseId);
            return course != null && courses.Remove(course);
        }

        public Course? GetCourse(string courseId) => courses.FirstOrDefault(c => c.CourseId == courseId);
        public List<Course> GetAllCourses() => new List<Course>(courses);

        public void AddTeacher(Teacher teacher)
        {
            if (teacher == null)
                throw new ArgumentNullException(nameof(teacher));
            
            if (teachers.Any(t => t.TeacherId == teacher.TeacherId))
                throw new InvalidOperationException($"Teacher with ID {teacher.TeacherId} already exists");
            
            teachers.Add(teacher);
        }

        public Teacher? GetTeacher(string teacherId) => teachers.FirstOrDefault(t => t.TeacherId == teacherId);
        public List<Teacher> GetAllTeachers() => new List<Teacher>(teachers);

        public bool AssignTeacherToCourse(string teacherId, string courseId)
        {
            var teacher = GetTeacher(teacherId);
            var course = GetCourse(courseId);

            if (teacher == null || course == null)
                return false;

            course.Teacher = teacher;
            return true;
        }

        public List<Course> GetCoursesByTeacher(string teacherId) => 
            courses.Where(c => c.Teacher?.TeacherId == teacherId).ToList();


        public void AddStudent(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));
            
            if (students.Any(s => s.StudentId == student.StudentId))
                throw new InvalidOperationException($"Student with ID {student.StudentId} already exists");
            
            students.Add(student);
        }

        public Student? GetStudent(string studentId) => students.FirstOrDefault(s => s.StudentId == studentId);
        public List<Student> GetAllStudents() => new List<Student>(students);

        public bool EnrollStudentInCourse(string studentId, string courseId)
        {
            var student = GetStudent(studentId);
            var course = GetCourse(courseId);

            if (student == null || course == null)
                return false;

            course.AddStudent(student);
            return true;
        }

        public List<Student> GetStudentsInCourse(string courseId)
        {
            var course = GetCourse(courseId);
            return course?.Students ?? new List<Student>();
        }

        public List<Course> GetCoursesByStudent(string studentId) =>
            courses.Where(c => c.Students.Any(s => s.StudentId == studentId)).ToList();
    }
}