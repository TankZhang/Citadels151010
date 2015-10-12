using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Cards
{
    public class CardRes
    {
        public List<Building> Buildings;
        public List<Hero> Heros;
        //加到建筑List中特定类别的，特定名称的，特定价格的，启示id，的有限数目个building
        public static void Add(List<Building> bl, FunctionType type, string name, int price, ref int Startnum, int num)
        {
            for (int i = Startnum; i < Startnum + num; i++)
            {
                Building b = new Building(i, price, name, type);
                bl.Add(b);
            }
            Startnum += num;
        }
        public List<Hero> RandOrderHList(List<Hero> BL)
        {
            Random r = new Random();
            List<Hero> newBL = new List<Hero>();
            foreach (var item in BL)
            {
                newBL.Insert(r.Next(newBL.Count + 1), item);
            }
            return newBL;
        }

        public List<Building> RandOrderBList(List<Building> BL)
        {
            Random r = new Random();
            List<Building> newBL = new List<Building>();
            foreach (var item in BL)
            {
                newBL.Insert(r.Next(newBL.Count + 1), item);
            }
            return newBL;
        }

        public CardRes()
        {
            List<Building> orderBuildings = new List<Building>();
            List<Hero> orderHeros = new List<Hero>();
            orderHeros.Add(new Hero(1, "刺客", FunctionType.nothing));
            orderHeros.Add(new Hero(2, "小偷", FunctionType.nothing));
            orderHeros.Add(new Hero(3, "魔术师", FunctionType.nothing));
            orderHeros.Add(new Hero(4, "国王", FunctionType.noble));
            orderHeros.Add(new Hero(5, "主教", FunctionType.religious));
            orderHeros.Add(new Hero(6, "商人", FunctionType.commercial));
            orderHeros.Add(new Hero(7, "建筑师", FunctionType.nothing));
            orderHeros.Add(new Hero(8, "军阀", FunctionType.warlord));
            int startNum = 1;
            Add(orderBuildings, FunctionType.warlord, "战场", 3, ref startNum, 3);
            Add(orderBuildings, FunctionType.warlord, "堡垒", 5, ref startNum, 2);
            Add(orderBuildings, FunctionType.warlord, "监狱", 2, ref startNum, 3);
            Add(orderBuildings, FunctionType.warlord, "瞭望塔", 3, ref startNum, 3);
            Add(orderBuildings, FunctionType.noble, "城堡", 4, ref startNum, 4);
            Add(orderBuildings, FunctionType.noble, "皇宫", 5, ref startNum, 3);
            Add(orderBuildings, FunctionType.noble, "庄园", 3, ref startNum, 5);
            Add(orderBuildings, FunctionType.religious, "大教堂", 5, ref startNum, 2);
            Add(orderBuildings, FunctionType.religious, "教堂", 2, ref startNum, 3);
            Add(orderBuildings, FunctionType.religious, "修道院", 3, ref startNum, 3);
            Add(orderBuildings, FunctionType.religious, "神殿", 1, ref startNum, 3);
            Add(orderBuildings, FunctionType.commercial, "海港", 4, ref startNum, 3);
            Add(orderBuildings, FunctionType.commercial, "市场", 2, ref startNum, 4);
            Add(orderBuildings, FunctionType.commercial, "酒馆", 1, ref startNum, 5);
            Add(orderBuildings, FunctionType.commercial, "市政厅", 5, ref startNum, 2);
            Add(orderBuildings, FunctionType.commercial, "贸易站", 2, ref startNum, 3);
            Add(orderBuildings, FunctionType.commercial, "码头", 3, ref startNum, 3);
            Add(orderBuildings, FunctionType.magic, "城墙", 6, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "大学", 6, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "铁匠铺", 5, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "天文台", 5, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "魔法学校", 6, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "图书馆", 6, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "实验室", 5, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "鬼城", 2, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "要塞", 3, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "龙门", 6, ref startNum, 1);
            Add(orderBuildings, FunctionType.magic, "墓地", 5, ref startNum, 1);
            Buildings = RandOrderBList(orderBuildings);
            Heros = RandOrderHList(orderHeros);
        }
    }
}
