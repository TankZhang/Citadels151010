using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Cards
{
   public  class NobleBuilding:Building
    {
        public NobleBuilding() { }
        public NobleBuilding(int id, int price, string name,string imgPath, string description)
            : base(id, price, name, FunctionType.noble, imgPath,description)
        { }
    }
}
