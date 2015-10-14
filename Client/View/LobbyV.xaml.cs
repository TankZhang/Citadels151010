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
    /// LobbyV.xaml 的交互逻辑
    /// </summary>
    public partial class LobbyV : Window
    {
        LobbyVM lobbyVM = new LobbyVM();
        public LobbyV()
        {
            DataContext = lobbyVM;
            InitializeComponent();
        }

        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsEnabled)
            {
                GameV gameV = new GameV();
                gameV.Show();
            }
        }
    }
}
