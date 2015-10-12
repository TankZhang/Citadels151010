using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Cards
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
    public class Hero
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

        public Hero() { }
        public Hero(int iD, string name, FunctionType type)
        {
            ID = iD;
            Name = name;
            Type = type;
        }
    }
}
