using Client.Model.Cards;
using Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.View
{
    /// <summary>
    /// GameV.xaml 的交互逻辑
    /// </summary>
    public partial class GameV : Window
    {
        GameVM gameVM;
        public GameV(int num, int seatNum)
        {
            gameVM = new GameVM(num);
            gameVM.GamePlayerList[seatNum - 1].Exp = 1;
            DataContext = gameVM;
            InitializeComponent();
        }
        #region 测试
        public GameV()
        {
            CardRes cardRes = new CardRes();
            gameVM = new GameVM(3);
            InitializeComponent();
            DataContext = gameVM;
            myCitadel.DataContext = gameVM.GamePlayerList[2];
            leftCitadel.DataContext = gameVM.GamePlayerList[1];
            rightCitadel.DataContext = gameVM.GamePlayerList[0];
            gameVM.GamePlayerList[0].Nick = "1号玩家";
            gameVM.GamePlayerList[1].Nick = "2号玩家";
            gameVM.GamePlayerList[2].Nick = "3号玩家";
            gameVM.CenterPlayer = gameVM.GamePlayerList;
            gameVM.GamePlayerList[2].Buildings.Add(cardRes.Buildings[0]);
            gameVM.GamePlayerList[2].Buildings.Add(cardRes.Buildings[10]);
            gameVM.GamePlayerList[2].Buildings.Add(cardRes.Buildings[20]);
            gameVM.GamePlayerList[2].Roles.Add(cardRes.Heros[0]);
            gameVM.GamePlayerList[2].Roles.Add(cardRes.Heros[1]);
            gameVM.GamePlayerList[1].Buildings.Add(cardRes.Buildings[5]);
            gameVM.GamePlayerList[1].Buildings.Add(cardRes.Buildings[15]);
            gameVM.GamePlayerList[1].Buildings.Add(cardRes.Buildings[25]);
            gameVM.GamePlayerList[1].Roles.Add(cardRes.Heros[2]);
            gameVM.GamePlayerList[1].Roles.Add(cardRes.Heros[3]);
            gameVM.GamePlayerList[0].Buildings.Add(cardRes.Buildings[8]);
            gameVM.GamePlayerList[0].Buildings.Add(cardRes.Buildings[18]);
            gameVM.GamePlayerList[0].Buildings.Add(cardRes.Buildings[28]);
            gameVM.GamePlayerList[0].Roles.Add(cardRes.Heros[4]);
            gameVM.GamePlayerList[0].Roles.Add(cardRes.Heros[5]);
            gameVM.CenterHeros.Add(cardRes.Heros[6]);
            gameVM.CenterHeros.Add(cardRes.Heros[7]);
            gameVM.CenterBuildings.Add(cardRes.Buildings[2]);
            gameVM.CenterBuildings.Add(cardRes.Buildings[15]);
            gameVM.PocketBuildings.Add(cardRes.Buildings[2]);
            gameVM.PocketBuildings.Add(cardRes.Buildings[60]);
        }
        #endregion
    }
}
