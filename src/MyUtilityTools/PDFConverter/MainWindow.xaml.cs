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
        const float FONT_SIZE = 10.0f;
        const string FONT_NAME = "ms gothic";//"ms gothic"; //"ms micho";
        const string FONT_FILENAME = @"C:\Windows\Fonts\msgothic.ttc,0";

        private PDFConverter.DocumentMargin margin;

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

            margin = new DocumentMargin(20f, 20f, 50f, 20f);

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            this.outputFilePath = System.IO.Path.GetDirectoryName(exePath) + @"\OUTPUT.pdf";
        }

        ~MainWindow()
        {

        }

        private Font GetFont()
        {
            FontFactory.RegisterDirectories();
            Font retFont = FontFactory.GetFont(FONT_NAME, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, FONT_SIZE);
            return retFont;
        }

        private BaseFont GetBaseFont()
        {
            FontFactory.RegisterDirectories();
            BaseFont retBaseFont = BaseFont.CreateFont(FONT_FILENAME, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            return retBaseFont;
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

            Font font = GetFont();
            BaseFont baseFont = GetBaseFont();

            List<string> eachPdfPathList = new List<string>();
            foreach (string textPath in pathList)
            {
                string pdfPath = textPath + ".pdf";
                eachPdfPathList.Add(pdfPath);

                using (FileStream fs = new FileStream(pdfPath, FileMode.Create))
                {
                    using (Document doc = new Document(PageSize.A4, margin.left, margin.right, margin.top, margin.bottom))
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(doc, fs);
                        ITextEvents itextEvents = new ITextEvents();
                        itextEvents.Margin = margin;
                        itextEvents.HeaderFont = font;
                        itextEvents.HeaderBaseFont = baseFont;
                        pdfWriter.PageEvent = itextEvents;
                        doc.Open();

                        ///TODO: ファイルエンコードの取得と設定
                        ///932:SJIS
                        Encoding enc = Encoding.GetEncoding(932);
                        StreamReader sr = new StreamReader(textPath, enc);
                        string text = sr.ReadToEnd();

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
                        doc.Add(new iTextSharp.text.Paragraph(text, font));

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
    }
}
