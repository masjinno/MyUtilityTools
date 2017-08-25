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
        private int[] margin = new int[] { 10, 10, 30, 10 };

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

            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            this.outputFilePath = System.IO.Path.GetDirectoryName(exePath) + @"\OUTPUT.pdf";
        }

        ~MainWindow()
        {

        }

        private Document SetDocument(string path)
        {
            Document retDoc = new Document(PageSize.A4, margin[0], margin[1], margin[2], margin[3]);
            FileStream pdfStream = new FileStream(path, FileMode.Create);
            PdfWriter pdfWriter = PdfWriter.GetInstance(retDoc, pdfStream);

            return retDoc;
        }

        private Font GetBaseFont()
        {
            const float FONT_SIZE = 7.0f;
            const string FONT_NAME = "MS Gothic"; //"MS Micho";
            FontFactory.RegisterDirectories();
            Font retFont = FontFactory.GetFont(FONT_NAME, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, FONT_SIZE);

            return retFont;
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

            Font baseFont = GetBaseFont();

            List<string> eachPdfPathList = new List<string>();
            foreach (string textPath in pathList)
            {
                string pdfPath = textPath + ".pdf";
                eachPdfPathList.Add(pdfPath);
                //string textName = System.IO.Path.GetFileName(textPath);
                Document doc = this.SetDocument(pdfPath);
                //PdfWriter pdfWriter = PdfWriter.GetInstance(doc, new FileStream(pdfPath, FileMode.Create));
                doc.Open();

                //PdfContentByte pcb = pdfWriter.DirectContent;
                //pcb.MoveTo(0, );
                //pcb.LineTo(595, 820);
                //pcb.Stroke();

                ///TODO: ファイルエンコードの取得と設定
                ///932:SJIS
                Encoding enc = Encoding.GetEncoding(932);
                StreamReader sr = new StreamReader(textPath, enc);
                string text = sr.ReadToEnd();
                sr.Close();
                doc.Add(new iTextSharp.text.Paragraph(text, baseFont));

                doc.Close();
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

                /// 出力するPDFのプロパティを設定
                //objITextDoc.AddKeywords("キーワードです。");
                //objITextDoc.AddAuthor("zero0nine.com");
                //objITextDoc.AddTitle("結合したPDFファイルです。");
                //objITextDoc.AddCreator("PDFファイル結合くん");
                //objITextDoc.AddSubject("結合したPDFファイル");

                /// ソートが必要ない場合は、コメント
                //pdfList.Sort();

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

    class MyHeaderFooterEvent : PdfPageEventHelper
    {
        Font FONT = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD);

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfContentByte canvas = writer.DirectContent;
            ColumnText.ShowTextAligned(
              canvas, Element.ALIGN_LEFT,
              new Phrase("Header", FONT), 10, 810, 0
            );
            ColumnText.ShowTextAligned(
              canvas, Element.ALIGN_LEFT,
              new Phrase("Footer", FONT), 10, 10, 0
            );
        }
    }
}
