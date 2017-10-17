using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressViewer.Model
{
    /// <summary>
    /// IPv4アドレス処理クラス
    /// ・IPv4アドレス(文字列)のリストを取得する
    /// </summary>
    class IPv4AddressProc : IPAddressProc
    {
        public List<string> V4AddressList
        {
            get
            {
                return GetAddressList();
            }
        }

        public IPv4AddressProc()
        {
            this.addressFamily = System.Net.Sockets.AddressFamily.InterNetwork;
        }
    }
}
