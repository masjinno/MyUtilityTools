using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFConverter
{
    public class DocumentMargin
    {
        public float left { get; set; }
        public float right { get; set; }
        public float top { get; set; }
        public float bottom { get; set; }

        public DocumentMargin()
        {
        }

        public DocumentMargin(float left, float right, float top, float bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
    }
}
