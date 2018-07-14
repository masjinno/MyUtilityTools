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
        /// IPv4アドレス処理クラス
        /// </summary>
        private IPv4AddressProc ipv4AddressProc;

        /// <summary>
        /// IPv6アドレス処理クラス
        /// </summary>
        private IPv6AddressProc ipv6AddressProc;

        /// <summary>
        /// IPv4アドレスの文字列
        /// </summary>
        private string ipv4
        {
            get
            {
                if (this.IPv4Index >= 0) return this.ipv4AddressProc.V4AddressList[IPv4Index];
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
                if (this.IPv6Index >= 0) return this.ipv6AddressProc.V6AddressList[IPv6Index];
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
                if (this.IsIPv6Checked) return this.ipv6;
                return "Checked IPAddress is invalid";
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
                if (this.ipv4AddressProc.V4AddressList.Count() >= prevIndex) return 0;
                if (this.ipv4AddressProc.V4AddressList.Count() <= 0) return -1;
                return this._ipv4Index;
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
                if (this.ipv6AddressProc.V6AddressList.Count() >= prevIndex) return 0;
                if (this.ipv6AddressProc.V6AddressList.Count() <= 0) return -1;
                return this._ipv6Index;
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
                RaisePropertyChanged(nameof(this.IPAddressString));
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
                RaisePropertyChanged(nameof(this.IPAddressString));
            }
        }

        /// <summary>
        /// クライアント領域の幅
        /// </summary>
        private double _clientWidth;
        public double ClientWidth
        {
            get { return this._clientWidth; }
            set { SetProperty(ref this._clientWidth, value); }
        }

        /// <summary>
        /// クライアント領域の高さ
        /// </summary>
        private double _clientHeight;
        public double ClientHeight
        {
            get { return this._clientHeight; }
            set { SetProperty(ref this._clientHeight, value); }
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
                    this.IsIPv4Checked = true;
                    this.IsIPv6Checked = false;
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
                    this.IsIPv6Checked = true;
                    this.IsIPv4Checked = false;
                });
            }
        }

        /// <summary>
        /// IPアドレス要求コマンド
        /// </summary>
        public ICommand RequireIPAddressCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    RaisePropertyChanged(nameof(this.IPAddressString));
                });
            }
        }

        /// <summary>
        /// クライアント領域のサイズ変更イベント
        /// IPアドレス文字列の文字サイズをフィットするサイズに変更する(未実装)
        /// </summary>
        public ICommand ClientAreaSizeChangedCommand
        {
            get
            {
                return new DelegateCommand<System.Windows.Controls.Grid>((sender) =>
                {
                    double width = sender.ActualWidth;
                    double height = sender.ActualHeight;
                    this.IPAddressFontSize = this.CalcFontSize(width, height, this.IPAddressString);
                    RaisePropertyChanged(nameof(this.IPAddressFontSize));
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
            this.ipv4AddressProc = new IPv4AddressProc();
            this.ipv6AddressProc = new IPv6AddressProc();
            this.IPAddressFontSize = 48;    // 暫定値
            this.IsIPv4Checked = true;
            this.IsIPv6Checked = false;
        }

        /// <summary>
        /// サイズ(<paramref name="rectWidth"/>, <paramref name="rectHeight"/>)の矩形領域の内側にフィットする、
        /// <paramref name="text"/>のテキストを持つTextBlockのフォントサイズを求める。
        /// </summary>
        /// <param name="rectWidth">矩形領域の幅(pixel)</param>
        /// <param name="rectHeight">矩形領域の高さ(pixel)</param>
        /// <param name="text">TextBlockのtext</param>
        /// <returns>フォントサイズ</returns>
        private double CalcFontSize(double rectWidth, double rectHeight, string text)
        {
            if (text == null) throw new ArgumentNullException();
            if (rectWidth < 0 || rectHeight < 0) throw new ArgumentOutOfRangeException();
            if (text == string.Empty || text.Length <= 0) throw new ArgumentException();

            const double MIN_SEARCH_SIZE = 0.5;
            const double START_FONT_SIZE = MIN_SEARCH_SIZE * 129;
            double retFontSize = START_FONT_SIZE;
            double searchJumpSize = START_FONT_SIZE / 2.0;
            bool isFirstSmaller = true;

            System.Windows.Controls.TextBlock dummyTextBlock = new System.Windows.Controls.TextBlock();
            dummyTextBlock.Text = text;

            while (searchJumpSize > MIN_SEARCH_SIZE)
            {
                dummyTextBlock.FontSize = retFontSize;

                if (this.IsBiggerRect(dummyTextBlock.ActualWidth, dummyTextBlock.ActualHeight, rectWidth, rectHeight))
                {
                    searchJumpSize *= 0.5;
                    retFontSize -= searchJumpSize;
                    isFirstSmaller = false;
                }
                else
                {
                    if (!isFirstSmaller)
                    {
                        searchJumpSize *= 0.5;
                    }
                    retFontSize += searchJumpSize;
                }
            }

            return retFontSize;
        }

        /// <summary>
        /// 判定対象の矩形が比較対象の矩形からはみ出るか判定する
        /// </summary>
        /// <param name="targetRectWidth">判定対象矩形の幅</param>
        /// <param name="targetRectHeight">判定対象矩形の高さ</param>
        /// <param name="compRectWidth">比較対象矩形の幅</param>
        /// <param name="compRectHeight">比較対象矩形の高さ</param>
        /// <returns>判定対象の矩形が比較対象の矩形からはみ出るか</returns>
        private bool IsBiggerRect(double targetRectWidth, double targetRectHeight, double compRectWidth, double compRectHeight)
        {
            return (targetRectWidth > compRectWidth || targetRectHeight > compRectHeight);
        }
    }
}
