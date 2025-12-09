using System.Reflection;

namespace KR04
{
    class Program
    {
        public static void Main()
        {
            Task01();
            Task02();
            Task03();
        }
        public static void Task01()
        {
            Console.WriteLine("\n=== Задача 1 ===\n");

            Student student = new(12, 1);
            Type studentType = student.GetType();
            // А дальше ваще дохляк
            var fields = studentType.GetRuntimeFields();
            foreach (var field in fields)
            {
                Console.WriteLine($"{field.Name} = {field.GetValue(student)}");
                field.SetValue(student, 52);
                Console.WriteLine($"{field.Name} = {field.GetValue(student)} (изменено)");
            }
        }

        public static void Task02()
        {
            Console.WriteLine("\n=== Задача 2 ===\n");
            var smallObject = new Student(52, 3); // ну а чем не маленький объект
            byte[] bigObject = new byte[1000000];

            Console.WriteLine("До очистки мусора");
            Console.WriteLine($"Маленький объект - {GC.GetGeneration(smallObject)} поколение");
            Console.WriteLine($"Большой объект - {GC.GetGeneration(bigObject)} поколение");

            GC.Collect();

            Console.WriteLine("После очистки мусора");
            Console.WriteLine($"Маленький объект - {GC.GetGeneration(smallObject)} поколение");
            Console.WriteLine($"Большой объект - {GC.GetGeneration(bigObject)} поколение");
        }

        public static void Task03()
        {
            void writeLetters()
            {
                for (char i = 'A'; i <= 'Z'; i++)
                {
                    Console.Write(i);
                }
            }

            void writeNumbers()
            {
                for (int i = 1; i <= 100; i++)
                {
                    Console.Write(i);
                }
            }

            Console.WriteLine("\n=== Задача 3 ===\n");


            Thread letterThread = new(writeLetters);
            Thread numberThread = new(writeNumbers);

            letterThread.Start();
            numberThread.Start();
        }
    }

    class Student
    {
        private int Age;
        private int Course;

        public Student(int age, int course)
        {
            Age = age;
            Course = course;
        }
    }

}