using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Cards
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

        string _name;
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

        FunctionType _type;
        public FunctionType Type
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

        string _description;
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public Hero() { }
        public Hero(int id,string name,FunctionType type,string imgPath, string description)
        {
            Id = id;
            Name = name;
            Type = type;
            ImgPath = imgPath;
            Description = description;
        }
    }
}
