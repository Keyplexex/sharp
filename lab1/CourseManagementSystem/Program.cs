using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseManagementSystem
{
    class Program
    {
        private static CourseManager manager = new CourseManager();

        static void Main(string[] args)
        {
            Console.WriteLine("=== Система управления курсами и преподавателями ===\n");
            
            AddSampleData();
            
            bool exit = false;
            while (!exit)
            {
                ShowMenu();
                var input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        ShowAllCourses();
                        break;
                    case "2":
                        AddNewCourse();
                        break;
                    case "3":
                        RemoveCourse();
                        break;
                    case "4":
                        ShowAllTeachers();
                        break;
                    case "5":
                        AddNewTeacher();
                        break;
                    case "6":
                        AssignTeacherToCourse();
                        break;
                    case "7":
                        ShowAllStudents();
                        break;
                    case "8":
                        AddNewStudent();
                        break;
                    case "9":
                        EnrollStudentInCourse();
                        break;
                    case "10":
                        ShowCoursesByTeacher();
                        break;
                    case "11":
                        ShowStudentsInCourse();
                        break;
                    case "12":
                        ShowCoursesByStudent();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Выход из системы...");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
                
                if (!exit)
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("=== ГЛАВНОЕ МЕНЮ ===");
            Console.WriteLine("1. Показать все курсы");
            Console.WriteLine("2. Добавить новый курс");
            Console.WriteLine("3. Удалить курс");
            Console.WriteLine("4. Показать всех преподавателей");
            Console.WriteLine("5. Добавить нового преподавателя");
            Console.WriteLine("6. Назначить преподавателя на курс");
            Console.WriteLine("7. Показать всех студентов");
            Console.WriteLine("8. Добавить нового студента");
            Console.WriteLine("9. Записать студента на курс");
            Console.WriteLine("10. Показать курсы преподавателя");
            Console.WriteLine("11. Показать студентов на курсе");
            Console.WriteLine("12. Показать курсы студента");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
        }

        static void AddSampleData()
        {
            manager.AddTeacher(new Teacher("T001", "Иван Петров", "ivan@university.ru", "Информатика"));
            manager.AddTeacher(new Teacher("T002", "Мария Сидорова", "maria@university.ru", "Математика"));
            
            manager.AddStudent(new Student("S001", "Алексей Иванов", "alex@student.ru", 2));
            manager.AddStudent(new Student("S002", "Елена Смирнова", "elena@student.ru", 1));
            manager.AddStudent(new Student("S003", "Дмитрий Козлов", "dmitry@student.ru", 3));
            
            manager.AddCourse(new OnlineCourse("C001", "C# Programming", "Основы программирования на C#", 
                                            "Microsoft Teams", "https://teams.com/csharp"));
            manager.AddCourse(new OfflineCourse("C002", "Алгебра", "Высшая математика", 
                                              "Аудитория 101", "Главный корпус", "Пн/Ср/Пт 10:00-11:30"));
            
            manager.AssignTeacherToCourse("T001", "C001");
            manager.AssignTeacherToCourse("T002", "C002");
            
            manager.EnrollStudentInCourse("S001", "C001");
            manager.EnrollStudentInCourse("S002", "C001");
            manager.EnrollStudentInCourse("S003", "C001");
            manager.EnrollStudentInCourse("S001", "C002");
        }

        static void ShowAllCourses()
        {
            Console.WriteLine("\n=== ВСЕ КУРСЫ ===");
            var courses = manager.GetAllCourses();
            if (courses.Count == 0)
            {
                Console.WriteLine("Курсы не найдены.");
                return;
            }
            
            foreach (var course in courses)
            {
                Console.WriteLine($"- {course.GetInfo()}");
            }
        }

        static void AddNewCourse()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОГО КУРСА ===");
            
            Console.Write("Тип курса (1 - Онлайн, 2 - Офлайн): ");
            var typeInput = Console.ReadLine();
            
            Console.Write("ID курса: ");
            var id = Console.ReadLine();
            
            Console.Write("Название курса: ");
            var name = Console.ReadLine();
            
            Console.Write("Описание курса: ");
            var description = Console.ReadLine();
            
            try
            {
                if (typeInput == "1")
                {
                    Console.Write("Платформа: ");
                    var platform = Console.ReadLine();
                    
                    Console.Write("Ссылка на встречу: ");
                    var link = Console.ReadLine();
                    
                    var course = new OnlineCourse(id, name, description, platform, link);
                    manager.AddCourse(course);
                    Console.WriteLine(" Онлайн курс успешно добавлен!");
                }
                else if (typeInput == "2")
                {
                    Console.Write("Аудитория: ");
                    var classroom = Console.ReadLine();
                    
                    Console.Write("Корпус: ");
                    var building = Console.ReadLine();
                    
                    Console.Write("Расписание: ");
                    var schedule = Console.ReadLine();
                    
                    var course = new OfflineCourse(id, name, description, classroom, building, schedule);
                    manager.AddCourse(course);
                    Console.WriteLine(" Офлайн курс успешно добавлен!");
                }
                else
                {
                    Console.WriteLine(" Неверный тип курса.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
            }
        }

        static void RemoveCourse()
        {
            Console.WriteLine("\n=== УДАЛЕНИЕ КУРСА ===");
            ShowAllCourses();
            
            Console.Write("Введите ID курса для удаления: ");
            var courseId = Console.ReadLine();
            
            if (manager.RemoveCourse(courseId))
            {
                Console.WriteLine(" Курс успешно удален!");
            }
            else
            {
                Console.WriteLine(" Курс не найден.");
            }
        }

        static void ShowAllTeachers()
        {
            Console.WriteLine("\n=== ВСЕ ПРЕПОДАВАТЕЛИ ===");
            var teachers = manager.GetAllTeachers();
            if (teachers.Count == 0)
            {
                Console.WriteLine("Преподаватели не найдены.");
                return;
            }
            
            foreach (var teacher in teachers)
            {
                Console.WriteLine($"- {teacher}");
            }
        }

        static void AddNewTeacher()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОГО ПРЕПОДАВАТЕЛЯ ===");
            
            Console.Write("ID преподавателя: ");
            var id = Console.ReadLine();
            
            Console.Write("Имя преподавателя: ");
            var name = Console.ReadLine();
            
            Console.Write("Email: ");
            var email = Console.ReadLine();
            
            Console.Write("Кафедра: ");
            var department = Console.ReadLine();
            
            try
            {
                var teacher = new Teacher(id, name, email, department);
                manager.AddTeacher(teacher);
                Console.WriteLine(" Преподаватель успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Ошибка: {ex.Message}");
            }
        }

        static void AssignTeacherToCourse()
        {
            Console.WriteLine("\n=== НАЗНАЧЕНИЕ ПРЕПОДАВАТЕЛЯ НА КУРС ===");
            
            ShowAllTeachers();
            Console.Write("Введите ID преподавателя: ");
            var teacherId = Console.ReadLine();
            
            ShowAllCourses();
            Console.Write("Введите ID курса: ");
            var courseId = Console.ReadLine();
            
            if (manager.AssignTeacherToCourse(teacherId, courseId))
            {
                Console.WriteLine(" Преподаватель успешно назначен на курс!");
            }
            else
            {
                Console.WriteLine(" Ошибка: преподаватель или курс не найден.");
            }
        }

        static void ShowAllStudents()
        {
            Console.WriteLine("\n=== ВСЕ СТУДЕНТЫ ===");
            var students = manager.GetAllStudents();
            if (students.Count == 0)
            {
                Console.WriteLine("Студенты не найдены.");
                return;
            }
            
            foreach (var student in students)
            {
                Console.WriteLine($"- {student}");
            }
        }

        static void AddNewStudent()
        {
            Console.WriteLine("\n=== ДОБАВЛЕНИЕ НОВОГО СТУДЕНТА ===");
            
            Console.Write("ID студента: ");
            var id = Console.ReadLine();
            
            Console.Write("Имя студента: ");
            var name = Console.ReadLine();
            
            Console.Write("Email: ");
            var email = Console.ReadLine();
            
            Console.Write("Курс (год обучения): ");
            if (int.TryParse(Console.ReadLine(), out int year))
            {
                try
                {
                    var student = new Student(id, name, email, year);
                    manager.AddStudent(student);
                    Console.WriteLine(" Студент успешно добавлен!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Ошибка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine(" Неверный формат года обучения.");
            }
        }

        static void EnrollStudentInCourse()
        {
            Console.WriteLine("\n=== ЗАПИСЬ СТУДЕНТА НА КУРС ===");
            
            ShowAllStudents();
            Console.Write("Введите ID студента: ");
            var studentId = Console.ReadLine();
            
            ShowAllCourses();
            Console.Write("Введите ID курса: ");
            var courseId = Console.ReadLine();
            
            if (manager.EnrollStudentInCourse(studentId, courseId))
            {
                Console.WriteLine(" Студент успешно записан на курс!");
            }
            else
            {
                Console.WriteLine(" Ошибка: студент или курс не найден.");
            }
        }

        static void ShowCoursesByTeacher()
        {
            Console.WriteLine("\n=== КУРСЫ ПРЕПОДАВАТЕЛЯ ===");
            
            ShowAllTeachers();
            Console.Write("Введите ID преподавателя: ");
            var teacherId = Console.ReadLine();
            
            var courses = manager.GetCoursesByTeacher(teacherId);
            if (courses.Count == 0)
            {
                Console.WriteLine("Курсы не найдены или преподаватель не существует.");
                return;
            }
            
            Console.WriteLine($"Курсы преподавателя:");
            foreach (var course in courses)
            {
                Console.WriteLine($"- {course.Name} ({course.GetCourseType()})");
            }
        }

        static void ShowStudentsInCourse()
        {
            Console.WriteLine("\n=== СТУДЕНТЫ НА КУРСЕ ===");
            
            ShowAllCourses();
            Console.Write("Введите ID курса: ");
            var courseId = Console.ReadLine();
            
            var students = manager.GetStudentsInCourse(courseId);
            if (students.Count == 0)
            {
                Console.WriteLine("Студенты не найдены или курс не существует.");
                return;
            }
            
            Console.WriteLine($"Студенты на курсе:");
            foreach (var student in students)
            {
                Console.WriteLine($"- {student.Name}");
            }
        }

        static void ShowCoursesByStudent()
        {
            Console.WriteLine("\n=== КУРСЫ СТУДЕНТА ===");
            
            ShowAllStudents();
            Console.Write("Введите ID студента: ");
            var studentId = Console.ReadLine();
            
            var courses = manager.GetCoursesByStudent(studentId);
            if (courses.Count == 0)
            {
                Console.WriteLine("Курсы не найдены или студент не существует.");
                return;
            }
            
            Console.WriteLine($"Курсы студента:");
            foreach (var course in courses)
            {
                Console.WriteLine($"- {course.Name} ({course.GetCourseType()})");
            }
        }
    }
}