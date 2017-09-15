using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDFConverter
{
    class ITextEvents : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        public BaseFont baseFont { get; set; } = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;

        // Margin of page
        public DocumentMargin Margin { get; set; }

        // Base font
        public iTextSharp.text.Font font { get; set; }

        // Text file name
        public string convertedFileName { get; set; }

        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);

            /// Create PdfTable object
            PdfPTable pdfHeaderTable = new PdfPTable(1);
            PdfPTable pdfFooterTable = new PdfPTable(1);

            /// Header Setting
            Phrase headerPhrase = new Phrase(this.convertedFileName, font);
            PdfPCell pdfHeaderCell = new PdfPCell(headerPhrase);
            pdfHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT;
            pdfHeaderCell.VerticalAlignment = Element.ALIGN_CENTER;
            pdfHeaderCell.Border = 0;
            pdfHeaderTable.AddCell(pdfHeaderCell);
            pdfHeaderTable.TotalWidth = document.PageSize.Width - (Margin.left + Margin.right);
            pdfHeaderTable.WidthPercentage = 100;
            pdfHeaderTable.WriteSelectedRows(0, -1, Margin.left, document.PageSize.Height - Margin.top * 0.5F, writer.DirectContent);

            //Move the pointer and draw line to separate header section from rest of page
            cb.MoveTo(Margin.left, document.PageSize.Height - Margin.top);
            cb.LineTo(document.PageSize.Width - Margin.right, document.PageSize.Height - Margin.top);
            cb.Stroke();

            /// Footer Setting
            StringBuilder sb = new StringBuilder();
            sb.Append("< ").Append(writer.PageNumber).Append(" >");
            Phrase footerPhrase = new Phrase(sb.ToString());
            PdfPCell pdfFooterCell = new PdfPCell(footerPhrase);
            pdfFooterCell.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfFooterCell.VerticalAlignment = Element.ALIGN_CENTER;
            pdfFooterCell.Border = 0;
            pdfFooterTable.AddCell(pdfFooterCell);
            pdfFooterTable.TotalWidth = document.PageSize.Width - (Margin.left + Margin.right);
            pdfFooterTable.WidthPercentage = 100;
            pdfFooterTable.WriteSelectedRows(0, -1, Margin.left, Margin.bottom * 0.8F, writer.DirectContent);

            //Move the pointer and draw line to separate footer section from rest of page
            cb.MoveTo(Margin.left, document.PageSize.GetBottom(Margin.bottom));
            cb.LineTo(document.PageSize.Width - Margin.right, document.PageSize.GetBottom(Margin.bottom));
            cb.Stroke();
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(baseFont, 12);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber).ToString());
            headerTemplate.EndText();

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(baseFont, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber).ToString());
            footerTemplate.EndText();
        }
    }
}
