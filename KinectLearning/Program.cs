using System;
using System.Collections.Generic;
using System.Globalization;

namespace KinectLearning
{
    internal class Person
    {
        private string name;
        public string Name {
            get {
                return name;
            }
            set {
                if (string.IsNullOrEmpty(value))
                {
                    name = "Unknown";
                }
                else
                {
                    name = value;
                }
            } 
        }

        private int age;
        public int Age //encapsulation, Age sanitization, and validation. The Age property is used to control access to the private age field, ensuring that it cannot be set to a negative value.
        {
            get
            {
                return age;
            }
            set
            {
                if (value < 0)
                { age = 0; }
                else { age = value; }
                ;
            }
        }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public void SayHello()
        {
            Console.WriteLine($"Hello! My name is {Name} and I am {Age} years old.");
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
            //Console.Write("Create a Person \n");
            //string name = Console.ReadLine();
            //Console.Write("Enter age: ");
            //int age = int.Parse(Console.ReadLine());

            //Person person = new Person(name, age);
            //person.SayHello();

            //if (person.IsAdult())
            //{
            //    Console.WriteLine("This person is an adult.");
            //}

            //Console.WriteLine(person.Name);
            //List<Person> people = new List<Person>();

            //for (int i = 0; i < 4; i++)
            //{
            //    string name = Console.ReadLine();
            //    int age = int.Parse(Console.ReadLine());
            //    Person person = new Person($"Person {i} - {name}", age);
            //    people.Add(person);
            //}

            //foreach (Person person in people)
            //{
            //    if (person.IsAdult())
            //    {
            //        person.SayHello();
            //        Console.WriteLine($"{person.Name} : This person is an adult.");
            //    }
            //}


            KinectTest.Run();
            Console.ReadLine();



        }

        }
    }
