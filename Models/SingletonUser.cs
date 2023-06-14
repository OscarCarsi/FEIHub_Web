using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeiHub.Models
{
    public class SingletonUser
    {
        private static SingletonUser instance;
        private static readonly object lockObject = new object();


        public string Username { get; set; }
        public string Rol { get; set; }
        public string Token { get; set; }
        public static SingletonUser Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new SingletonUser();
                        }
                    }
                }
                return instance;
            }
        }
        public void BorrarSinglenton()
        {
            SingletonUser.Instance.Username = "";
            SingletonUser.Instance.Rol = "";
            SingletonUser.Instance.Token = "";
        }
    }
}
