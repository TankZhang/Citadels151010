using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModel
{
   public class GameVM:NotificationObject
    {
        Thread ThReceive;

        public delegate void Del(string a);
        Del del;

        public void ReceiveSocket(object obj)
        {
            Socket s = obj as Socket;
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 5];
                try
                {
                    int r = s.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    //DealReceivePre(str);
                    Console.WriteLine("game收到了：" + str);
                    Application.Current.Dispatcher.Invoke(del, str);
                }//try结束
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

        //预处理，防止收到两个连续数据包
        void DealReceivePre(string s)
        {
            string[] ss = s.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in ss)
            {
                DealReceive(item);
            }
        }

        //处理game收到的信息
        private void DealReceive(string item)
        {

        }
        //构造函数
        public GameVM()
        {
            del = new Del(DealReceivePre);
            ThReceive = new Thread(ReceiveSocket);
            ThReceive.IsBackground = true;
            ThReceive.Start(App.NetCtrl.SocketClient);
        }
    }
}
