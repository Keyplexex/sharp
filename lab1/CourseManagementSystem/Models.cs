using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseManagementSystem
{
    public abstract class Course
    {
        public string CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Teacher? Teacher { get; set; }
        public List<Student> Students { get; set; }

        public Course(string courseId, string name, string description)
        {
            CourseId = courseId;
            Name = name;
            Description = description;
            Students = new List<Student>();
        }

        public abstract string GetCourseType();
        
        public virtual void AddStudent(Student student)
        {
            if (!Students.Contains(student))
            {
                Students.Add(student);
            }
        }

        public virtual void RemoveStudent(Student student)
        {
            Students.Remove(student);
        }

        public virtual string GetInfo()
        {
            return $"Course: {Name} (ID: {CourseId}), Type: {GetCourseType()}, Teacher: {Teacher?.Name ?? "Not assigned"}, Students: {Students.Count}";
        }
    }

    public class OnlineCourse : Course
    {
        public string Platform { get; set; }
        public string MeetingLink { get; set; }
        public bool IsRecorded { get; set; }

        public OnlineCourse(string courseId, string name, string description, string platform, string meetingLink)
            : base(courseId, name, description)
        {
            Platform = platform;
            MeetingLink = meetingLink;
            IsRecorded = false;
        }

        public override string GetCourseType() => "Online";

        public override string GetInfo()
        {
            return base.GetInfo() + $", Platform: {Platform}, Link: {MeetingLink}, Recorded: {IsRecorded}";
        }

        public void EnableRecording() => IsRecorded = true;
        public void DisableRecording() => IsRecorded = false;
    }

    public class OfflineCourse : Course
    {
        public string Classroom { get; set; }
        public string Building { get; set; }
        public string Schedule { get; set; }

        public OfflineCourse(string courseId, string name, string description, string classroom, string building, string schedule)
            : base(courseId, name, description)
        {
            Classroom = classroom;
            Building = building;
            Schedule = schedule;
        }

        public override string GetCourseType() => "Offline";

        public override string GetInfo()
        {
            return base.GetInfo() + $", Classroom: {Classroom}, Building: {Building}, Schedule: {Schedule}";
        }
    }

    public class Teacher
    {
        public string TeacherId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }

        public Teacher(string teacherId, string name, string email, string department)
        {
            TeacherId = teacherId;
            Name = name;
            Email = email;
            Department = department;
        }

        public override string ToString() => $"{Name} (ID: {TeacherId}, Department: {Department})";
    }

    public class Student
    {
        public string StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Year { get; set; }

        public Student(string studentId, string name, string email, int year)
        {
            StudentId = studentId;
            Name = name;
            Email = email;
            Year = year;
        }

        public override string ToString() => $"{Name} (ID: {StudentId}, Year: {Year})";

        public override bool Equals(object? obj)
        {
            if (obj is Student other)
            {
                return StudentId == other.StudentId;
            }
            return false;
        }

        public override int GetHashCode() => StudentId.GetHashCode();
    }
}