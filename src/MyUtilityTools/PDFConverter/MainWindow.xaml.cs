using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDFConverter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DEFAULT_PAGE_SIZE = "A4";
        private const float DEFAULT_FONT_SIZE = 10F;
        private const string DEFAULT_FONT_NAME = "ms gothic";//"ms gothic"; //"ms micho";
        private const string DEFAULT_FONT_FILENAME = @"C:\Windows\Fonts\msgothic.ttc,0";
        private const float DEFAULT_MARGIN_LEFT = 30F;
        private const float DEFAULT_MARGIN_RIGHT = 30F;
        private const float DEFAULT_MARGIN_TOP = 50F;
        private const float DEFAULT_MARGIN_BOTTOM = 50F;

        private PDFConverter.DocumentMargin margin;
        private iTextSharp.text.Rectangle pageSize;
        private float fontSize = 10F;

        private readonly Dictionary<string, iTextSharp.text.Rectangle> PageSizeDictionary = new Dictionary<string, iTextSharp.text.Rectangle>()
        {
            { "A0", PageSize.A0 }, { "A1", PageSize.A1 }, { "A2", PageSize.A2 }, { "A3", PageSize.A3 }, { "A4", PageSize.A4 }, { "A5", PageSize.A5 },
            { "A6", PageSize.A6 }, { "A7", PageSize.A7 }, { "A8", PageSize.A8 }, { "A9", PageSize.A9 }, { "A10", PageSize.A10 },
            { "B0", PageSize.B0 }, { "B1", PageSize.B1 }, { "B2", PageSize.B2 }, { "B3", PageSize.B3 }, { "B4", PageSize.B4 }, { "B5", PageSize.B5 },
            { "B6", PageSize.B6 }, { "B7", PageSize.B7 }, { "B8", PageSize.B8 }, { "B9", PageSize.B9 }, { "B10", PageSize.B10 },
        };

        private string outputFilePath
        {
            get
            {
                return this.OutputFilePath_TextBox.Text;
            }
            set
            {
                this.OutputFilePath_TextBox.Text = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            margin = new DocumentMargin();

            this.InitPageSize();
            this.InitMargin();
            this.InitFontFamily();
            this.InitFontSize();

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            this.outputFilePath = System.IO.Path.GetDirectoryName(exePath) + @"\OUTPUT.pdf";
        }

        ~MainWindow()
        {

        }

        private void InitPageSize()
        {
            foreach (string key in PageSizeDictionary.Keys)
            {
                this.PageSize_ComboBox.Items.Add(key);
            }
            PageSize_ComboBox.SelectedItem = DEFAULT_PAGE_SIZE;
        }

        private void InitMargin()
        {
            this.MarginLeft_TextBox.Text = DEFAULT_MARGIN_LEFT.ToString();
            this.MarginRight_TextBox.Text = DEFAULT_MARGIN_RIGHT.ToString();
            this.MarginTop_TextBox.Text = DEFAULT_MARGIN_TOP.ToString();
            this.MarginBottom_TextBox.Text = DEFAULT_MARGIN_BOTTOM.ToString();
        }

        private void InitFontFamily()
        {
            FontFactory.RegisterDirectory("C:\\WINDOWS\\Fonts");
            foreach (string fontname in FontFactory.RegisteredFonts)
            {
                FontFamily_ComboBox.Items.Add(fontname);
            }
            FontFamily_ComboBox.SelectedItem = DEFAULT_FONT_NAME;
        }

        private void InitFontSize()
        {
            FontSize_TextBox.Text = DEFAULT_FONT_SIZE.ToString();
        }


        private Font GetFont(string fontname, float fontsize)
        {
            FontFactory.RegisterDirectories();
            Font retFont = FontFactory.GetFont(fontname, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, fontsize);
            return retFont;
        }

        private BaseFont GetBaseFont()
        {
            FontFactory.RegisterDirectories();
            BaseFont retBaseFont = BaseFont.CreateFont(DEFAULT_FONT_FILENAME, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return retBaseFont;
        }

        private string ReplaceTabToSpace(string text, int spaceNum)
        {
            if (spaceNum <= 0) return text;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < spaceNum; i++)
            {
                sb.Append(" ");
            }

            string spaces = sb.ToString();
            string retText;

            retText = text.Replace("\t", spaces);

            return retText;
        }

        private void Convert_Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> pathList = new List<string>();
            string[] inputPath = Paths_TextBox.Text.Split(new string[2] { "\r\n", "\"" }, StringSplitOptions.None);
            foreach (string s in inputPath)
            {
                if (File.Exists(s))
                {
                    pathList.Add(s);
                }
            }

            if (pathList.Count() <= 0)
            {
                MessageBox.Show("Valid text file path(s) is nothing.\r\nInput text file path.", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Font bodyFont = GetFont(FontFamily_ComboBox.SelectedItem.ToString(), this.fontSize);
            Font headerFont = GetFont(DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE);
            Font footerFont = GetFont(DEFAULT_FONT_NAME, DEFAULT_FONT_SIZE);
            BaseFont baseFont = GetBaseFont();

            List<string> eachPdfPathList = new List<string>();
            foreach (string textPath in pathList)
            {
                string pdfPath = textPath + ".pdf";
                eachPdfPathList.Add(pdfPath);

                string textFileName = System.IO.Path.GetFileName(textPath);

                using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
                {
                    using (Document doc = new Document(this.pageSize, this.margin.left, this.margin.right, this.margin.top, this.margin.bottom))
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(doc, fs);
                        ITextEvents itextEvents = new ITextEvents();
                        itextEvents.Margin = margin;
                        itextEvents.headerFont = headerFont;
                        itextEvents.footerFont = footerFont;
                        itextEvents.baseFont = baseFont;
                        itextEvents.convertedFileName = textFileName;
                        pdfWriter.PageEvent = itextEvents;
                        doc.Open();

                        ///TODO: ファイルエンコードの取得と設定
                        ///932:SJIS
                        Encoding enc = Encoding.GetEncoding(932);
                        StreamReader sr = new StreamReader(textPath, enc);
                        string text = sr.ReadToEnd();
                        string noTabText = this.ReplaceTabToSpace(text, 4);

                        #region how to research usable fonts
                        //int totalfonts = FontFactory.RegisterDirectory("C:\\WINDOWS\\Fonts");
                        //StringBuilder sb = new StringBuilder();
                        //foreach (string fontname in FontFactory.RegisteredFonts)
                        //{
                        //    sb.Append(fontname + "\n");
                        //}
                        //doc.Add(new iTextSharp.text.Paragraph("All Fonts:\n" + sb.ToString()));
                        #endregion

                        sr.Close();
                        doc.Add(new iTextSharp.text.Paragraph(noTabText, bodyFont));

                        doc.Close();
                    }
                }
            }
            
            this.JoinPdf(this.outputFilePath, eachPdfPathList);
            MessageBox.Show("PDF files have created.", "information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OutputFilePathBrowse_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDFファイル(*.pdf)|*.pdf|すべてのファイル(*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                outputFilePath = sfd.FileName;
            }
        }

        /// <summary>
        /// リスト内のPDFを結合しPDFを作成する
        /// http://zero0nine.com/archives/2866
        /// </summary>
        /// <param name="outPdfPath">
        /// 結合後のPDFファイルパス（フルパス）
        /// </param>
        /// <param name="pdfList">
        /// 結合対象のPDFファイルパスを格納したリスト
        /// </param>
        /// <returns>
        /// true:正常
        /// false:異常
        /// </returns>
        private bool JoinPdf(string outPdfPath, List<string> pdfList)
        {
            bool ret = true;

            Document joinedDoc = null;
            PdfCopy pdfCopy = null;

            try
            {
                joinedDoc = new Document();
                pdfCopy = new PdfCopy(joinedDoc, new FileStream(outPdfPath, FileMode.Create));

                joinedDoc.Open();

                /// 結合対象ファイル分ループ
                pdfList.ForEach(list =>
                {
                    Debug.WriteLine(list);
                    /// 結合元のPDFファイル読込
                    PdfReader pdfReader = new PdfReader(list);
                    /// 結合元のPDFファイルを追加(全ページ)
                    pdfCopy.AddDocument(pdfReader);
                    pdfReader.Close();
                });
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
                ret = false;
            }
            finally
            {
                joinedDoc.Close();
                pdfCopy.Close();
            }

            return ret;
        }

        private void ParseTextToFloat(string text, ref float value)
        {
            float tmpValue;

            if (float.TryParse(text, out tmpValue))
            {
                value = tmpValue;
            }
            else
            {
                MessageBox.Show("Input text is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PageSize_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pageSize = PageSizeDictionary[(sender as ComboBox).SelectedItem.ToString()];
        }

        private void MarginTop_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            float tmp = margin.top;
            ParseTextToFloat((sender as TextBox).Text, ref tmp);
            margin.top = tmp;
        }

        private void MarginLeft_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            float tmp = margin.left;
            ParseTextToFloat((sender as TextBox).Text, ref tmp);
            margin.left = tmp;
        }

        private void MarginRight_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            float tmp = margin.right;
            ParseTextToFloat((sender as TextBox).Text, ref tmp);
            margin.right = tmp;
        }

        private void MarginBottom_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            float tmp = margin.bottom;
            ParseTextToFloat((sender as TextBox).Text, ref tmp);
            margin.bottom = tmp;
        }

        private void FontFamily_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FontSize_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            float tmp = fontSize;
            ParseTextToFloat((sender as TextBox).Text, ref tmp);
            fontSize = tmp;
        }
    }
}
