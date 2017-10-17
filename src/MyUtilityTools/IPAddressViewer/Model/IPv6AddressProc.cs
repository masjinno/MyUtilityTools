using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressViewer.Model
{
    /// <summary>
    /// IPv6アドレス処理クラス
    /// ・IPv6アドレス(文字列)のリストを取得する
    /// </summary>
    class IPv6AddressProc : IPAddressProc
    {
        public List<string> V6AddressList
        {
            get
            {
                return GetAddressList();
            }
        }

        public IPv6AddressProc()
        {
            this.addressFamily = System.Net.Sockets.AddressFamily.InterNetworkV6;
        }
    }
}
