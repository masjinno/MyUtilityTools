using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace IPAddressViewer.Model
{
    /// <summary>
    /// IPアドレス処理クラス
    /// </summary>
    public class IPAddressProc
    {
        public System.Net.Sockets.AddressFamily addressFamily { get; set; }

        protected List<string> GetAddressList()
        {
            string hostname = Dns.GetHostName();

            // ホスト名からIPアドレスを取得する
            IPAddress[] adrList = Dns.GetHostAddresses(hostname);

            List<string> addressList = new List<string>();
            foreach (IPAddress add in adrList)
            {
                if (add.AddressFamily == this.addressFamily)
                {
                    addressList.Add(add.ToString());
                }
            }
            return addressList;
        }
    }
}
