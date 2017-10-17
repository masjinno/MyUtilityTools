using IPAddressViewer.Model;
using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private string ipv4
        {
            get
            {
                if (this.IPv4Index >= 0) return this.ipAddressProc.V4AddressList[IPv4Index];
                else return "IPv4 is invalid";
            }
        }

        /// <summary>
        /// IPv6アドレスの文字列
        /// </summary>
        public string ipv6
        {
            get
            {
                if (this.IPv6Index >= 0) return this.ipAddressProc.V6AddressList[IPv6Index];
                else return "IPv6 is invalid";
            }
        }

        #region Binding用プロパティ

        /// <summary>
        /// 表示するIPアドレス文字列
        /// </summary>
        public string IPAddressString
        {
            get
            {
                if (this.IsIPv4Checked) return this.ipv4;
                else if (this.IsIPv6Checked) return this.ipv6;
                else return "Checked IPAddress is invalid";
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
        /// IPv6アドレスのリスト
        /// </summary>
        private int _ipv6Index;
        public int IPv6Index
        {
            get
            {
                int prevIndex = this._ipv6Index;
                if (this.ipAddressProc.V6AddressList.Count() >= prevIndex) return 0;
                if (this.ipAddressProc.V6AddressList.Count() <= 0) return -1;
                else return this._ipv6Index;
            }
            set { SetProperty(ref _ipv6Index, value); }
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

        /// <summary>
        /// IPv4を選択しているか
        /// </summary>
        private bool _isIPv4Checked;
        public bool IsIPv4Checked
        {
            get { return this._isIPv4Checked; }
            set
            {
                SetProperty(ref this._isIPv4Checked, value);
                RaisePropertyChanged(nameof(IPAddressString));
            }
        }

        /// <summary>
        /// IPv6を選択しているか
        /// </summary>
        private bool _isIPv6Checked;
        public bool IsIPv6Checked
        {
            get { return this._isIPv6Checked; }
            set
            {
                SetProperty(ref this._isIPv6Checked, value);
                RaisePropertyChanged(nameof(IPAddressString));
            }
        }

        #endregion

        #region Binding用コマンド

        /// <summary>
        /// IPv4選択時に処理するコマンド
        /// </summary>
        public ICommand IPv4ChooseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.IsIPv6Checked = !this.IsIPv4Checked;
                });
            }
        }

        /// <summary>
        /// IPv6選択時に処理するコマンド
        /// </summary>
        public ICommand IPv6ChooseCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    this.IsIPv4Checked = !this.IsIPv6Checked;
                });
            }
        }

        #endregion


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
            this.IsIPv4Checked = true;
            this.IsIPv6Checked = false;
        }

        private double CalcFontSize(double rectWidth, double rectHeight)
        {
            double retFontSize = 11;



            return retFontSize;
        }
    }
}
