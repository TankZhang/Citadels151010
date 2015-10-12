﻿using System;
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
    /// ConnectV.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectV : Window
    {
        public ConnectV()
        {
            InitializeComponent();
        }

        private void Window_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue==false)
            {
                Close();
            }
        }
    }
}
