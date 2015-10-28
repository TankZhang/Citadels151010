using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public enum FunctionType
    {
        commercial,
        religious,
        noble,
        warlord,
        magic,
        nothing
    }
    public class Hero:NotificationObject
    {

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

        string s;
        public string S
        {
            get
            {
                return s;
            }

            set
            {
                s = value;
                RaisePropertyChanged("S");
            }
        }

        public Hero() { }
        public Hero(int iD, string name, FunctionType type,string s)
        {
            ID = iD;
            Name = name;
            Type = type;
            S = s;
        }
    }
}
