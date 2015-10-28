using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Building:NotificationObject
    {

        private int _price;
        public int Price
        {
            get
            {
                return _price;
            }

            set
            {
                _price = value;
            }
        }

        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        private FunctionType _type;
        internal FunctionType Type
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }

        private int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }

            set
            {
                _ID = value;
            }
        }

        string _imgPath;
        public string ImgPath
        {
            get
            {
                return _imgPath;
            }

            set
            {
                _imgPath = value;
            }
        }


        public Building() { }
        public Building(int id, int price, string name, FunctionType type,string s)
        {
            ID = id;
            Price = price;
            Name = name;
            Type = type;
            ImgPath = s;
        }
    }
}
