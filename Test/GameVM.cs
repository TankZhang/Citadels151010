using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Test
{
    class GameVM:NotificationObject 
    {
        ObservableCollection<PlayerData> _playerDataList;
        public ObservableCollection<PlayerData> PlayerDataList
        {
            get
            {
                return _playerDataList;
            }

            set
            {
                _playerDataList = value;
                RaisePropertyChanged("PlayerDataList");
            }
        }

        ObservableCollection<Hero> _centerHeroList;
        public ObservableCollection<Hero> CenterHeroList
        {
            get
            {
                return _centerHeroList;
            }

            set
            {
                _centerHeroList = value;
                RaisePropertyChanged("CenterHeroList");
            }
        }

        ObservableCollection<Building> _centerArchBList;
        public ObservableCollection<Building> CenterArchBList
        {
            get
            {
                return _centerArchBList;
            }

            set
            {
                _centerArchBList = value;
                RaisePropertyChanged("CenterArchBList");
            }
        }

        ICommand _Send;
        public ICommand Send
        {
            get
            {
                return _Send;
            }

            set
            {
                _Send = value;
            }
        }

        public void Build(object o)
        {
            IEnumerable a=(IEnumerable)o;
            foreach (var item in a)
            {
                Building b = item as Building;
                Console.Write(b.ID);
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
        public void Build()
        {
            Console.WriteLine(PlayerDataList[0].Nick);
        }

        public GameVM()
        {
            Send = new RelayCommand(new Action<object>(Build));
            PlayerDataList = new ObservableCollection<PlayerData>();
            CenterArchBList = new ObservableCollection<Building>();
            CenterHeroList = new ObservableCollection<Hero>();
            PlayerData p1 = new PlayerData();
            p1.Nick = "p1";
            Building b1 = new Building(1, 1, "b1", FunctionType.commercial, @"\res\militaryBuilding_Battlefield.png");
            Building b2 = new Building(2, 2, "b2", FunctionType.magic, @"\res\nobleBuilding_Castle.png");
            Building b3=new Building(3,3,"b3",FunctionType.noble, @"\res\nobleBuilding_Palace.png");
            Hero h1 = new Hero(1, "h1", FunctionType.nothing, @"\res\hero_1_Assassin.png");
            Hero h2 = new Hero(2, "h2", FunctionType.nothing, @"\res\hero_2_Thief.png");
            Hero h3 = new Hero(3, "h3", FunctionType.nothing, @"\res\hero_3_Magician.png");
            CenterArchBList.Add(b1);
            CenterArchBList.Add(b3);
            CenterHeroList.Add(h2);
            CenterHeroList.Add(h3);
            p1.Buildings.Add(b1);
            p1.Buildings.Add(b2);
            p1.Roles.Add(h1);
            p1.Roles.Add(h2);
            PlayerDataList.Add(p1);
            PlayerData p2 = new PlayerData();
            p2.Buildings.Add(b2);
            p2.Buildings.Add(b3);
            p2.Roles.Add(h2);
            p2.Roles.Add(h3);
            PlayerDataList.Add(p2);
        }
    }
}
