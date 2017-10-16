using IPAddressViewer.Model;
using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressViewer.ViewModel
{
    public class MainViewModel : BindableBase
    {
        /// <summary>
        /// IPアドレス処理クラス
        /// </summary>
        private IPAddressProc ipAddressProc;
        
        /// <summary>
        /// IPv4アドレスの文字列
        /// </summary>
        public string IPv4
        {
            get
            {
                if (this.IPv4Index >= 0) return this.ipAddressProc.V4AddressList[IPv4Index];
                else return "IPv4 is invalid";
            }
        }

        /// <summary>
        /// IPv4アドレスのリスト
        /// </summary>
        private int _ipv4Index;
        public int IPv4Index
        {
            get
            {
                int prevIndex = this._ipv4Index;
                if (this.ipAddressProc.V4AddressList.Count() >= prevIndex) return 0;
                if (this.ipAddressProc.V4AddressList.Count() <= 0) return -1;
                else return this._ipv4Index;
            }
            set { SetProperty(ref _ipv4Index, value); }
        }

        /// <summary>
        /// IPアドレスのフォントサイズ
        /// </summary>
        private double _ipAddressFontSize;
        public double IPAddressFontSize
        {
            get { return this._ipAddressFontSize; }
            set { SetProperty(ref this._ipAddressFontSize, value); }
        }


        private void ClientAreaSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            System.Windows.MessageBox.Show("message");
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            this.Init();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Init()
        {
            this.ipAddressProc = new IPAddressProc();
            this.IPAddressFontSize = 48;    // 暫定値
        }

        private double CalcFontSize(double rectWidth, double rectHeight)
        {
            double retFontSize = 11;



            return retFontSize;
        }
    }
}
