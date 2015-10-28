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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        GameVM gameVM;
        public MainWindow()
        {
            gameVM = new GameVM();
            InitializeComponent();
            DataContext = gameVM;
            //myZone.DataContext = gameVM.PlayerDataList[0];
            //myZone.rolesUC.DataContext = gameVM.PlayerDataList[0];
            //myZone.buildingsUC.DataContext = gameVM.PlayerDataList[0];
            //rightZone.DataContext= gameVM.PlayerDataList[1];
            //rightZone.rolesUC.DataContext = gameVM.PlayerDataList[1];
            //rightZone.buildingsUC.DataContext = gameVM.PlayerDataList[1];
            //upZone.DataContext = gameVM.PlayerDataList[1];
            //upZone.rolesUC.DataContext = gameVM.PlayerDataList[1];
            //upZone.buildingsUC.DataContext = gameVM.PlayerDataList[1];
            centerHero.DataContext = gameVM;
            cABUC.DataContext = gameVM;
            Console.WriteLine(4 % 4);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
                Height = Width * 9 / 16;
            if (e.HeightChanged)
                Width = Height * 16 / 9;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            gameVM.PlayerDataList[0].Roles.RemoveAt(0);
            gameVM.PlayerDataList[1].Roles.RemoveAt(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            gameVM.PlayerDataList[0].Roles.Add(new Hero(12,"h12",FunctionType.noble, @"\res\hero_4_King.png"));
            gameVM.PlayerDataList[1].Roles.Add(new Hero(12, "h12", FunctionType.noble, @"\res\hero_4_King.png"));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            gameVM.PlayerDataList[0].Buildings.RemoveAt(0);
            gameVM.PlayerDataList[1].Buildings.RemoveAt(0);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            gameVM.PlayerDataList[0].Buildings.Add(new Building(4, 4, "b4", FunctionType.noble, @"\res\nobleBuilding_Palace.png"));
            gameVM.PlayerDataList[1].Buildings.Add(new Building(4, 4, "b4", FunctionType.noble, @"\res\nobleBuilding_Palace.png"));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            gameVM.CenterHeroList.Clear();
            gameVM.CenterHeroList.Add(new Hero(12, "h12", FunctionType.noble, @"\res\hero_4_King.png"));
            gameVM.CenterHeroList.Add(new Hero(12, "h12", FunctionType.noble, @"\res\hero_4_King.png"));
        }
        
    }
}
