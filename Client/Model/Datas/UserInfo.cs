using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model.Datas
{
   public class UserInfo
    {
        string _mail;
        public string Mail
        {
            get
            {
                return _mail;
            }

            set
            {
                _mail = value;
            }
        }

        string _pwd;
        public string Pwd
        {
            get
            {
                return _pwd;
            }

            set
            {
                MD5 md5 = MD5.Create();
                byte[] buffer = Encoding.Default.GetBytes(value);
                byte[] MD5Buffer = md5.ComputeHash(buffer);
                string st = "";
                for (int i = 0; i < MD5Buffer.Length; i++)
                {
                    st += MD5Buffer[i].ToString("x2");
                }
                _pwd = st;
            }
        }

        string _nick;
        public string Nick
        {
            get
            {
                return _nick;
            }

            set
            {
                _nick = value;
            }
        }

        string _real;
        public string Real
        {
            get
            {
                return _real;
            }

            set
            {
                _real = value;
            }
        }

        int _exp;
        public int Exp
        {
            get
            {
                return _exp;
            }

            set
            {
                _exp = value;
            }
        }
        
        public UserInfo() { }
        public UserInfo(string mail,string pwd,string nick,string real,int exp)
        {
            Mail = mail;
            Nick = nick;
            Real = real;
            Exp = exp;
        }
    }
}
