using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryReadWrite
{
    internal static class Program
    {
        private static void Main()
        {
            List<Student> studentsToWrite =
                new()
                {
                    new()
                    {Name = "Жульен", Group = "G1", DateOfBirth = new DateTime(2001, 10, 22), AverageScore = 3.3M},
                    new Student
                    {Name = "Боб", Group = "G1", DateOfBirth = new DateTime(1999, 5, 25), AverageScore = 4.5M},
                    new Student
                    {Name = "Лилия", Group = "F2", DateOfBirth = new DateTime(1999, 1, 11), AverageScore = 5M},
                    new Student
                    {Name = "Роза", Group = "F2", DateOfBirth = new DateTime(1989, 9, 19), AverageScore = 3.7M},
                    new Student
                    {Name = "Михаил", Group = "A2", DateOfBirth = new DateTime(1988, 10, 29), AverageScore = 3.7M},
                    new Student
                    {Name = "Ирина", Group = "G1", DateOfBirth = new DateTime(1987, 2, 14), AverageScore = 3.7M}
                };

            Console.WriteLine($"Записываем данные учеников в файл в бинарном формате в файл:\n{Path.Combine(Directory.GetCurrentDirectory(), "students.dat")}");
            WriteStudentsToBinFile(studentsToWrite, "students.dat");
            PressAnyKey();

            Console.WriteLine("Произвочим чтение записанных данных и вывод результата на экран");
            List<Student> studentsRead = ReadStudentsFromBinFile("students.dat");
            PrintDatabase(studentsRead);
            PressAnyKey();

            Console.WriteLine("Производим сортировку студентов \"по группе\"");
            studentsRead.Sort((x, y) => string.CompareOrdinal(x.Group, y.Group));
            Console.WriteLine("\nПосле сортировки:");
            PrintDatabase(studentsRead);
            PressAnyKey();

            string desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Students");
            Console.WriteLine($"Создаем каталог {desktopPath}");
            Console.WriteLine("И сохраняем студентов в текстовые файлы. Для каждой группы отдельный файл.");
            SaveToDifferetFiles(studentsRead, desktopPath);
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            PressAnyKey();
        }

        /// Выводит сообщение "Нажмите любую кнопку..." в нижней части консоли,
        /// ожидает нажатия клавиши и очищает консоль.
        private static void PressAnyKey()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            Console.Write("Нажмите любую кнопку...");
            Console.ReadKey();
            Console.Clear();
        }

        /// Сохраняет данные студентов в отдельные текстовые файлы для каждой группы.
        private static void SaveToDifferetFiles(List<Student> students, string path)
        {
            if (CreateDirectory(path))
            {
                foreach (Student student in students)
                {
                    string filePath = Path.Combine(path, $"Группа {student.Group}.txt");

                    try
                    {
                        using (StreamWriter sw = File.AppendText(filePath))
                        {
                            sw.WriteLine($"{student.Name} \t{student.Group} \t{student.DateOfBirth:dd.MM.yyyy} \t{student.AverageScore}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при записи в файл {filePath}: {ex.Message}");
                    }
                }
            }
        }

        /// Создает директорию по указанному пути, если она не существует.
        private static bool CreateDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    return true;
                }
                else
                {
                    Console.WriteLine($"Директория уже существует: {path}");
                    PressAnyKey();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при создании директории: {e.Message}");
                PressAnyKey();
                return false;
            }
        }

        /// Записывает список студентов в бинарный файл.
        private static void WriteStudentsToBinFile(List<Student> students, string fileName)
        {
            using FileStream fs = new(fileName, FileMode.Create);
            using BinaryWriter bw = new(fs);
            foreach (Student student in students)
            {
                bw.Write(student.Name);
                bw.Write(student.Group);
                bw.Write(student.DateOfBirth.ToBinary());
                bw.Write(student.AverageScore);
            }
            bw.Flush();
            bw.Close();
            fs.Close();
        }

        /// Выводит данные студентов на консоль.
        private static void PrintDatabase(List<Student> studentData)
        {
            foreach (Student student in studentData)
            {
                Console.WriteLine($"{student.Name} \t{student.Group} \t{student.DateOfBirth:dd.MM.yyyy} \t{student.AverageScore}");
            }
        }

        /// Читает данные студентов из бинарного файла.
        private static List<Student> ReadStudentsFromBinFile(string fileName)
        {
            List<Student> result = new();
            using FileStream fs = new(fileName, FileMode.Open);
            using BinaryReader br = new(fs);

            int recordNumber = 0;
            while (fs.Position < fs.Length)
            {
                recordNumber++;
                Student student = ReadSingleStudent(br);
                if (student != null)
                {
                    result.Add(student);
                }
                else
                {
                    Console.WriteLine($"Обнаружены невалидные данные в записи {recordNumber}");
                }
            }

            Console.WriteLine($"Прочитано {result.Count} корректных записей из {recordNumber}\n");
            return result;
        }

        /// Читает данные одного студента из бинарного потока.
        private static Student ReadSingleStudent(BinaryReader br)
        {
            try
            {
                string name = br.ReadString();
                string group = br.ReadString();
                long dateTicks = br.ReadInt64();
                decimal averageScore = br.ReadDecimal();

                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Ошибка: пустое имя");
                    return null;
                }
                if (string.IsNullOrEmpty(group))
                {
                    Console.WriteLine("Ошибка: пустая группа");
                    return null;
                }
                if (dateTicks < DateTime.MinValue.Ticks || dateTicks > DateTime.MaxValue.Ticks)
                {
                    Console.WriteLine("Ошибка: некорректная дата рождения");
                    return null;
                }
                if (averageScore < 0 || averageScore > 5)
                {
                    Console.WriteLine("Ошибка: некорректный средний балл");
                    return null;
                }

                return new Student
                {
                    Name = name,
                    Group = group,
                    DateOfBirth = DateTime.FromBinary(dateTicks),
                    AverageScore = averageScore
                };
            }
            catch (EndOfStreamException)
            {
                Console.WriteLine("Достигнут конец файла");
                return null;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Ошибка ввода-вывода: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неожиданная ошибка: {ex.Message}");
                return null;
            }
        }
    }
}