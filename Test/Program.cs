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
            int a = 10;
            switch(a)
            {
                case 1:
                    break;
                case 10:
                    for (int i = 0; i < 4; i++)
                    {
                        break;
                    }
                    Console.WriteLine("dsa");
                    break;
            }
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
