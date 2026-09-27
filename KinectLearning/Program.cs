using System;
using System.Globalization;

namespace KinectLearning
{
    class Person
    {
        private string Name;
        int Age;

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public void SayHello()
        {
            Console.WriteLine("Hello!");
        }

        public bool IsAdult()
        {
            return (Age >= 18);
        }
    }

    internal class Program //C# programs are obrigatorily classes, which then have their methods (functions) inside them. The Main method is the entry point of the program, where execution starts.
    {
        static void Main(string[] args)
        {
            Console.Write("Create a Person \n");
            string name = Console.ReadLine();
            Console.Write("Enter age: ");
            int age = int.Parse(Console.ReadLine());

            Person person = new Person(name, age);
            person.SayHello();

            if (person.IsAdult())
            {
                Console.WriteLine("This person is an adult.");
            }

        }
    }
}
