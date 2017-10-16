using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace IPAddressShower
{
    class MyIPAddress
    {
        private string address;

        public MyIPAddress()
        {
            SetAddress();
        }

        private void SetAddress()
        {
            string hostname = Dns.GetHostName();

            // ホスト名からIPアドレスを取得する
            IPAddress[] adrList = Dns.GetHostAddresses(hostname);

            address = "";
            foreach (IPAddress add in adrList)
            {
                if (add.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    address = add.ToString();
                    //System.Windows.Forms.MessageBox.Show(address + "\n" + add.ToString(), "address");
                }
            }
        }

        public string GetAddress()
        {
            SetAddress();
            return address;
        }
    }
}
