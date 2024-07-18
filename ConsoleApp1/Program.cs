class Student
{
    public string Name { get; set; }
    public string Group { get; set; }
    public int Age { get; set; }

    public override string ToString()
    {
        return $"{Name} (Group: {Group}, Age: {Age})";
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<Student> students = new List<Student>
        {
            new Student
            {
                Name = "Alice",
                Group = "B2",
                Age = 20
            },
            new Student
            {
                Name = "Bob",
                Group = "A1",
                Age = 22
            },
            new Student
            {
                Name = "Charlie",
                Group = "C3",
                Age = 21
            },
            new Student
            {
                Name = "David",
                Group = "A1",
                Age = 23
            },
            new Student
            {
                Name = "Eve",
                Group = "B2",
                Age = 19
            }
        };

        List<Student> groupedStudents = GroupStudents(students);

        for (int i = 0; i < groupedStudents.Count; i++)
        {
            Console.WriteLine($"student{i + 1}: Group {groupedStudents[i].Group}");
            List<Student> studentsInGroup = GetStudentsInGroup(students, groupedStudents[i].Group);
            studentsInGroup.Sort(CompareStudentsByName);
            foreach (var student in studentsInGroup)
            {
                Console.WriteLine($"  {student}");
            }
            Console.WriteLine();
        }
    }

    static List<Student> GroupStudents(List<Student> students)
    {
        List<Student> groupedStudents = new List<Student>();
        foreach (var student in students)
        {
            bool groupExists = false;
            foreach (var groupedStudent in groupedStudents)
            {
                if (groupedStudent.Group == student.Group)
                {
                    groupExists = true;
                    break;
                }
            }
            if (!groupExists)
            {
                groupedStudents.Add(student);
            }
        }
        groupedStudents.Sort(CompareStudentsByGroup);
        return groupedStudents;
    }

    static List<Student> GetStudentsInGroup(List<Student> students, string group)
    {
        List<Student> studentsInGroup = new List<Student>();
        foreach (var student in students)
        {
            if (student.Group == group)
            {
                studentsInGroup.Add(student);
            }
        }
        return studentsInGroup;
    }

    static int CompareStudentsByGroup(Student x, Student y)
    {
        return string.Compare(x.Group, y.Group, StringComparison.Ordinal);
    }

    static int CompareStudentsByName(Student x, Student y)
    {
        return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
    }
}
