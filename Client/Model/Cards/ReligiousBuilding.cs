using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Cards
{
   public  class ReligiousBuilding:Building
    {
        public ReligiousBuilding() { }
        public ReligiousBuilding(int id, int price, string name, string imgPath,string description)
            : base(id, price, name, FunctionType.religious,imgPath, description)
        { }
    }
}
