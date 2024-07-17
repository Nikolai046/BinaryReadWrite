using System;
using System.Collections.Generic;
using System.IO;

namespace BinaryReadWrite
{
    static class Program
    {
        private static void Main()
        {
            List<Student> studentsToWrite =
                new()
                {
                    new()
                    {
                        Name = "Жульен",
                        Group = "G1",
                        DateOfBirth = new DateTime(2001, 10, 22),
                        AverageScore = 3.3M
                    },
                    new Student
                    {
                        Name = "Боб",
                        Group = "G1",
                        DateOfBirth = new DateTime(1999, 5, 25),
                        AverageScore = 4.5M
                    },
                    new Student
                    {
                        Name = "Лилия",
                        Group = "F2",
                        DateOfBirth = new DateTime(1999, 1, 11),
                        AverageScore = 5M
                    },
                    new Student
                    {
                        Name = "Роза",
                        Group = "F2",
                        DateOfBirth = new DateTime(1989, 9, 19),
                        AverageScore = 3.7M
                    }
                };

            WriteStudentsToBinFile(studentsToWrite, "students.dat");

            foreach (Student studentProp in ReadStudentsFromBinFile("students.dat"))
            {
                Console.WriteLine(
                    studentProp.Name
                        + " "
                        + studentProp.Group
                        + " "
                        + studentProp.DateOfBirth
                        + " "
                        + studentProp.AverageScore
                );
            }
        }

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

        //private static List<Student> ReadStudentsFromBinFile(string fileName)
        //{
        //    List<Student> result = new();
        //    using FileStream fs = new(fileName, FileMode.Open);
        //    BinaryReader br = new(fs);

        //    while (fs.Position < fs.Length)
        //    {
        //        Student student = new()
        //        {
        //            Name = br.ReadString(),
        //            Group = br.ReadString(),
        //            DateOfBirth = DateTime.FromBinary(br.ReadInt64()),
        //            AverageScore = br.ReadDecimal()
        //        };

        //        result.Add(student);
        //    }

        //    fs.Close();
        //    return result;
        //}
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
