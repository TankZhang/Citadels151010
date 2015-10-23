using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Cards
{
   public  class MilitaryBuilding:Building
    {
        public MilitaryBuilding() { }
        public MilitaryBuilding(int id,int price,string name,string imgPath, string description)
            :base(id,price,name,FunctionType.warlord,imgPath,description)
        { }
    }
}
