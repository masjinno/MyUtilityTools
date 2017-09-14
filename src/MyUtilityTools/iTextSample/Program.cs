///
/// http://logicalerror.seesaa.net/article/374104005.html
///
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // itext();

                CreatePDF();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("処理が終了しました");

        }

        #region 別のサンプル
        static void itext()
        {
            string thisPath = Assembly.GetExecutingAssembly().Location;
            string thisDir = Path.GetDirectoryName(thisPath);
            Console.WriteLine(thisDir);

            string pdf = thisDir + @"\pdftest.pdf";

            Document doc = new Document();

            // 出力先を指定し、文書をPDFとして出力
            PdfWriter pw = PdfWriter.GetInstance(
                doc,
                new FileStream(pdf, FileMode.Create)
            );

            // 出力開始
            doc.Open();

            // Windows ディレクトリを取得
            string windir = System.Environment.GetEnvironmentVariable("windir");
            Console.WriteLine(windir);

            // 日本語フォントの設定
            Font font = new Font(
                BaseFont.CreateFont(
                    windir + @"\Fonts\meiryo.ttc,0",
                    BaseFont.IDENTITY_H,
                    BaseFont.EMBEDDED
                 )
            );
            /*
            // フリーフォントでテスト
            // ttf の場合は ファイル名だけで指定します( ttc は複数のフォントが入っています )
                        Font font = new Font( 
                            BaseFont.CreateFont(
                                @"C:\Users\lightbox\Desktop\作業\FONT\ArmedBanana\ArmedBanana\ArmedBanana.ttf", 
                                BaseFont.IDENTITY_H, 
                                BaseFont.EMBEDDED
                             )
                        );
            */

            Font font2 = new Font(
                BaseFont.CreateFont(
                    windir + @"\Fonts\meiryo.ttc,1",
                    BaseFont.IDENTITY_H,
                    BaseFont.EMBEDDED
                 )
            );
            // *****************************************************************
            // 座標指定する為のオブジェクト
            // *****************************************************************
            // 一番上のレイヤー
            PdfContentByte pcb = pw.DirectContent;

            // 一番上のレイヤーに直線を引く
            pcb.MoveTo(0, 820);
            pcb.LineTo(595, 820);
            pcb.Stroke();

#if false
            ColumnText ct = new ColumnText(pcb);

            Phrase myText = null;
            String line_text = null;

            int x1 = 400;
            int x2 = 595;

            line_text = "1234567890123456789012345678901234567890";

            myText = new Phrase(line_text, font);
            ct.SetSimpleColumn(
                myText,
                x1, 0,      // 書き込み対象の左下の座標
                x2, 810,    // 書き込み対象の右上の座標
                20,         // 行間
                Element.ALIGN_LEFT
            );
            ct.Go();

            line_text = "ニ行目";

            myText = new Phrase(line_text, font);
            ct.SetSimpleColumn(
                myText,
                x1, 0,
                x2, 750,
                0,
                Element.ALIGN_LEFT
            );
            ct.Go();

            line_text = "三行目";

            myText = new Phrase(line_text, font);
            ct.SetSimpleColumn(
                myText,
                x1, 0,
                x2, 735,
                0,
                Element.ALIGN_LEFT
            );
            ct.Go();

            line_text = "四行目";

            // *****************************************************************
            // 右寄せ
            // *****************************************************************
            int right_offset = 50;
            myText = new Phrase(line_text, font);
            ct.SetSimpleColumn(
                myText,
                x1, 0,
                x2 - right_offset, 720,
                0,
                Element.ALIGN_LEFT
            );
            ct.Go();

            int top = 700;
            myText = new Phrase("left:" + doc.PageSize.Left, font2);
            ct.SetSimpleColumn(
                myText,
                x1, 0,
                x2 - right_offset, top,
                0,
                Element.ALIGN_RIGHT
            );
            ct.Go();

            top -= 10;
            myText = new Phrase("right:" + doc.PageSize.Right, font2);
            ct.SetSimpleColumn(
                myText,
                x1, 0,
                x2 - right_offset, top,
                0,
                Element.ALIGN_RIGHT
            );
            ct.Go();

            top -= 10;
            myText = new Phrase("top:" + doc.PageSize.Top, font2);
            ct.SetSimpleColumn(
                myText,
                x1, 0,
                x2 - right_offset, top,
                0,
                Element.ALIGN_RIGHT
            );
            ct.Go();

            top -= 10;
            myText = new Phrase("bottom:" + doc.PageSize.Bottom, font2);
            ct.SetSimpleColumn(
                myText,
                x1, 0,
                x2 - right_offset, top,
                0,
                Element.ALIGN_RIGHT
            );
            ct.Go();
#endif

            // *****************************************************************
            // 真ん中のレイヤー( 通常の文字列や画像・グラフィックの追加 )
            // *****************************************************************

            //Image img = Image.GetInstance("_img.jpg");
            //img.SetAbsolutePosition(140, 620);
            //img.ScalePercent(50);
            //doc.Add(img);

            //doc.Add(new Paragraph(" ", font));  // 改行
            //doc.Add(new Paragraph(" ", font));
            //doc.Add(new Paragraph(" ", font));
            //doc.Add(new Paragraph(" ", font));
            //doc.Add(new Paragraph(" ", font));

            doc.Add(new Paragraph("文を 一行出力", font));
            doc.Add(new Paragraph("文を 一行出力", font));
            doc.Add(new Paragraph("文を 一行出力", font));
            doc.Add(new Paragraph("文を 一行出力", font));
            doc.Add(new Paragraph("文を 一行出力", font));

            doc.Add(new Paragraph(" ", font));

            Paragraph p = new Paragraph();
            for (int i = 0; i < 25; i++)
            {
                p.Add(
                    new Chunk(
                        "日本語表示の一般テキストを繰り返し表示しています。"
                        , font
                    )
                );
            }
            doc.Add(p);

            doc.Add(new Paragraph("hogehogeほげほげホゲホゲ", font));
            // 出力終了
            doc.Close();

        }
        #endregion

        private static void CreatePDF()
        {
            string fileName = string.Empty;
            DateTime fileCreationDatetime = DateTime.Now;
            fileName = string.Format("{0}.pdf", fileCreationDatetime.ToString(@"yyyyMMdd") + "_" + fileCreationDatetime.ToString(@"HHmmss"));
            string pdfPath = @"C:\DEV\Tools\GitHub\MyUtilityTools\src\MyUtilityTools\PDFConverter\output.pdf";
            //Server.MapPath(@"~\PDFs\") + fileName;

            using (FileStream msReport = new FileStream(pdfPath, FileMode.Create))
            {
                //step 1
                using (Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 140f, 10f))
                {
                    try
                    {
                        // step 2
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, msReport);
                        pdfWriter.PageEvent = new iTextSample.ITextEvents();

                        //open the stream 
                        pdfDoc.Open();

                        for (int i = 0; i < 10; i++)
                        {
                            Paragraph para = new Paragraph("Hello world. Checking Header Footer", new Font(Font.FontFamily.HELVETICA, 22));
                            para.Alignment = Element.ALIGN_CENTER;
                            pdfDoc.Add(para);
                            pdfDoc.NewPage();
                        }

                        pdfDoc.Close();
                    }
                    catch (Exception ex)
                    {
                        //handle exception
                    }
                    finally
                    {
                    }
                }
            }
        }

    }
}
