using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //怎么排序
            //List<Person> l = new List<Person>();
            //l.Add(new Person(2));
            //l.Add(new Person(4));
            //l.Add(new Person(3));
            //l.Add(new Person(8));
            //ObservableCollection<Person> o = new ObservableCollection<Person>();
            //l.ForEach(s => o.Add(s));
            //o = new ObservableCollection<Person>(o.OrderBy(s => s.Id));
            //o=new ObservableCollection<Person>(o.Reverse());
            //foreach (var item in o)
            //{
            //    Console.WriteLine(item.Id);
            //}

            //排序
            //l.Sort(delegate (Person a, Person b) { return a.Id.CompareTo(b.Id); });
            //l.Reverse();
            //l.ForEach(s => Console.WriteLine(s.Id));
            //IEnumerable<Person> query = null;
            //query = from item in l orderby item.Id select item;
            //List<Person> l2 = new List<Person>();
            //foreach (var item in query)
            //{
            //    l2.Add(item);
            //}
            //l2.ForEach(s => Console.WriteLine(s.Id));

            //Startwith的应用
            string s = "woshiyige ";
            Console.WriteLine(s.StartsWith("shi"));
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
