﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace IPAddressViewer.Model
{
    /// <summary>
    /// IPアドレス処理クラス
    /// ・IPv4アドレス(文字列)のリストを取得する
    /// </summary>
    class IPAddressProc
    {
        public List<string> V4AddressList
        {
            get
            {
                string hostname = Dns.GetHostName();

                // ホスト名からIPアドレスを取得する
                IPAddress[] adrList = Dns.GetHostAddresses(hostname);

                List<string> v4Address = new List<string>();
                foreach (IPAddress add in adrList)
                {
                    if (add.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        v4Address.Add(add.ToString());
                        //System.Windows.Forms.MessageBox.Show(address + "\n" + add.ToString(), "address");
                    }
                }
                return v4Address;
            }
        }
    }
}