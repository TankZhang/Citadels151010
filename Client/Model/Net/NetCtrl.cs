using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Net
{
    public class NetCtrl
    {
        private Socket _socketClient;
        //生成与服务器通信的socket
        public Socket SocketClient
        {
            get
            {
                return _socketClient;
            }

            set
            {
                _socketClient = value;
            }
        }
        //构造函数
        public NetCtrl()
        {
        }
        public NetCtrl(string ipAddr, string port)
        {
            SocketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(ipAddr);
            IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(port));
            //获得要连接的远程服务器应用程序的IP地址和端口号
            try
            {
                SocketClient.Connect(point);
                //Thread th = new Thread(Receive);
                //th.IsBackground = true;
                //th.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
        //发送消息
        public void Send(string str)
        {
            str += "*";
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                SocketClient.Send(buffer);
                Console.WriteLine("客户端发送了" + str);
            }
            catch (Exception ex)
            {
                //抛出异常信息
                Console.WriteLine(ex.Message.ToString());
            }
        }
        //接收消息
        public void Receive()
        {
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 5];
                try
                {
                    int r = SocketClient.Receive(buffer);
                    if (r == 0)
                    {
                        break;
                    }
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    Console.WriteLine("Socket接收到了" + str);
                }//try结束
                catch (Exception ex)
                {
                    //抛出异常信息
                    Console.WriteLine(ex.Message.ToString());
                }
            }
        }

    }
}
