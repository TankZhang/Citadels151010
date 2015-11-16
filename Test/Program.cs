using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] ss = { "das", "da" };
            List<Person> ListP = new List<Person>();
            ListP.Add(new Person(1));
            ListP.Add(new Person(2));
            ListP.Add(new Person(3));
            ListP.Add(new Person(4));
            ListP.Add(new Person(3));
            Console.WriteLine(ListP.FindIndex(p => p.Id == 3));
            Console.WriteLine(ListP.FindAll(p => p.Id == 3));
            Console.WriteLine(ss[1]);
            Console.ReadKey();
        }
    }
    public class Person
    {
        int _id;
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public Person(int id)
        {
            Id = id;
        }
    }
}
