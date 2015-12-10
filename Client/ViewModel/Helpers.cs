using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Client.View
{
    //用于绑定中间选择grid的显示与隐藏
    public class VisibilityBindingHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //用于绑定显示手牌的显示文字
    class HandCardsbtnContentBindingHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "收回手牌";
            else
                return "查看手牌";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //用于绑定我要建设的显示和隐藏
    class CollapsedBindingHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<bool> bools = (ObservableCollection<bool>)value;
            if (bools[8] & bools[12])
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //用于绑定我要刺杀的显示和隐藏
    class BtnVisibleHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)value;
            if (flag)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //用于绑定显示和隐藏
    class VisibleHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)value;
            if (flag)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //用于绑定成功失败图片
    class IsWinHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = (bool)value;
            if (flag)
                return @"\Res\Pic\WinBackground.jpg";
            else
                return @"\Res\Pic\LossBackground.jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    //用于绑定中间提示文字
    class Index2TextHelper : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 1: return "请选择你想要的角色：";
                case 2: return "请选择你要盖下的角色：";
                case 3: return "请选择你要刺杀的角色：";
                case 4: return "请选择你要偷的角色：";
                case 5: return "请选择你要换牌的玩家：";
                case 6: return "请选择你要摧毁的玩家：";
                case 7: return "请选择你要选择的手牌：";
                case 8: return "请选择你要建设的手牌：";
                case 9: return "请选择你要摧毁的建筑：";
                case 10: return "请选择你要选择的手牌：";
                case 11: return "请选择你要丢弃的手牌：";
                case 12: return "请选择你要建筑的手牌（最多三张）：";
                case 13: return "请选择你要的手牌（最多两张）：";
                case 14: return "请选择你要与牌堆交换的牌：";
                case 15: return "请选择你要选择的手牌：";
                case 16: return "请选择你要丢弃的手牌：";
                default: return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
