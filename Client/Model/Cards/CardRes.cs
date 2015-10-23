using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Cards
{
   public class CardRes
    {
        public List<Building> Buildings { get; set; }
        public List<Hero> Heros { get; set; }

        public static void Add(List<Building> bl, FunctionType type, string name, int price,string imgPath, string description, ref int Startnum, int num)
        {
            for (int i = Startnum; i < Startnum + num; i++)
            {
                Building b = new Building(i, price, name, type,imgPath,description);
                bl.Add(b);
            }
            Startnum += num;
        }

        public CardRes()
        {
            Buildings = new List<Building>();
            Heros = new List<Hero>();
            Heros.Add(new Hero(0, "", FunctionType.nothing, @"\Res\Cards\hero_Back.png", ""));
            Heros.Add(new Hero(1, "刺客", FunctionType.nothing, @"\Res\Cards\hero_1_Assassin.png", "说出一位你想暗杀的角色（该角色必须保持沉默），该橘色不可翻开角色牌并直接失去他的回合。"));
            Heros.Add(new Hero(2, "小偷", FunctionType.nothing, @"\Res\Cards\hero_2_Thief.png", "说出以为你想偷取的角色，该角色拥有者进行回合前，取走他所有的金币（不可偷取刺客以及被暗杀的角色）。"));
            Heros.Add(new Hero(3, "魔术师", FunctionType.nothing, @"\Res\Cards\hero_3_Magician.png", "回合内可执行下列两种能力之一：1、将你的手牌与某位玩家交换；2、弃手中任意张数的地区牌至牌堆低并从牌堆上方拿回相同张数的地区牌。"));
            Heros.Add(new Hero(4, "国王", FunctionType.noble, @"\Res\Cards\hero_4_King.png", "每有一个贵族（黄色）地区便可以获得1枚金币。当国王被叫到时候，皇冠转移到该玩家手中。"));
            Heros.Add(new Hero(5, "主教", FunctionType.religious, @"\Res\Cards\hero_5_Bishop.png", "每有一个宗教（蓝色）地区便可以获得1枚金币，你的地区不会被军阀摧毁。"));
            Heros.Add(new Hero(6, "商人", FunctionType.commercial, @"\Res\Cards\hero_6_Merchant.png", "每有一个商业（绿色）地区便可以获得1枚金币，在你执行一次行动之后可以额外获得。"));
            Heros.Add(new Hero(7, "建筑师", FunctionType.nothing, @"\Res\Cards\hero_7_Architect.png", "在执行一次行动之后可以额外抽取2张地区牌加到你的手牌中。并且每次建设最多可建设3张地区牌。"));
            Heros.Add(new Hero(8, "军阀", FunctionType.warlord, @"\Res\Cards\hero_8_ Warlord.png", "每有一个军事（红色）地区便可以获得1枚金币。回合结束时，你可以支付某一地区的建造费用减少1枚金币来摧毁它。"));
            Buildings.Add(new Building(0, 0, "", FunctionType.nothing, @"\Res\Cards\building_Back.png", ""));
            int startNum = 1;
            Add(Buildings, FunctionType.warlord, "战场", 3, @"\Res\Cards\militaryBuilding_Battlefield.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.warlord, "堡垒", 5, @"\Res\Cards\militaryBuilding_Fortress.png", "", ref startNum, 2);
            Add(Buildings, FunctionType.warlord, "监狱", 2, @"\Res\Cards\militaryBuilding_Prison.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.warlord, "瞭望塔", 3, @"\Res\Cards\militaryBuilding_Watchtower.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.noble, "城堡", 4, @"\Res\Cards\nobleBuilding_Castle.png", "", ref startNum, 4);
            Add(Buildings, FunctionType.noble, "皇宫", 5, @"\Res\Cards\nobleBuilding_Palace.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.noble, "庄园", 3, @"\Res\Cards\nobleBuilding_Manor.png", "", ref startNum, 5);
            Add(Buildings, FunctionType.religious, "大教堂", 5, @"\Res\Cards\religiousBuilding_Cathedral.png", "", ref startNum, 2);
            Add(Buildings, FunctionType.religious, "教堂", 2, @"\Res\Cards\religiousBuilding_Church.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.religious, "修道院", 3, @"\Res\Cards\religiousBuilding_Monastery.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.religious, "神殿", 1, @"\Res\Cards\religiousBuilding_Temple.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.commercial, "海港", 4, @"\Res\Cards\tradeBuilding_Harbour.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.commercial, "市场", 2, @"\Res\Cards\tradeBuilding_Market.png", "", ref startNum, 4);
            Add(Buildings, FunctionType.commercial, "酒馆", 1, @"\Res\Cards\tradeBuilding_Pub.png", "", ref startNum, 5);
            Add(Buildings, FunctionType.commercial, "市政厅", 5, @"\Res\Cards\tradeBuilding_Townhouse.png", "", ref startNum, 2);
            Add(Buildings, FunctionType.commercial, "贸易站", 2, @"\Res\Cards\tradeBuilding_TradingPost.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.commercial, "码头", 3, @"\Res\Cards\tradeBuilding_Wharf.png", "", ref startNum, 3);
            Add(Buildings, FunctionType.magic, "城墙", 6, @"\Res\Cards\specialBuilding_Wall.png", "当军阀要摧毁你的其他地区时需要多付出一枚金币。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "大学", 6, @"\Res\Cards\specialBuilding_University.png", "建造此建筑需要6枚金币，但是在计分时值8分。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "铁匠铺", 5, @"\Res\Cards\specialBuilding_Smithy.png", "你可以付2枚金币来抽3张地区牌。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "天文台", 5, @"\Res\Cards\specialBuilding_Observatory.png", "当你选抽地区牌作为一次行动时，可抽三张地区牌并选一张加入到手牌中，剩余的2张放回牌堆最下层。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "魔法学校", 6, @"\Res\Cards\specialBuilding_MagicSchool.png", "当你获得收入时候，魔法学校可以被视为任何一种指定的颜色，例如如果你是国王，你可以把它视为一个贵族（黄色）地区。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "图书馆", 6, @"\Res\Cards\specialBuilding_Library.png", "当你选择抽地区牌作为一次行动时你可以保留2张。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "实验室", 5, @"\Res\Cards\specialBuilding_Laboratory.png", "你可以从手牌中丢弃一张地区牌来获得1枚金币。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "鬼城", 2, @"\Res\Cards\specialBuilding_GhostTown.png", "当计分时，鬼城可被视为任一个你指定的颜色，当鬼城是你最后一轮建造的地区时，不可以使用它的能力（仍然是紫色）。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "要塞", 3, @"\Res\Cards\specialBuilding_Fortress.png", "要塞不会被军阀摧毁。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "龙门", 6, @"\Res\Cards\specialBuilding_DragonGate.png", "建造此建筑需要6枚金币，但是在计分时值8分。", ref startNum, 1);
            Add(Buildings, FunctionType.magic, "墓地", 5, @"\Res\Cards\specialBuilding_Cemetery.png", "每当军阀摧毁一个地区，你可以付出1枚金币给银行，将此地区牌加入首派中，如果你是军阀就不可以使用它的能力。", ref startNum, 1);
        }
    }
}
