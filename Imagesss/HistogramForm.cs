using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Forms.DataVisualization.Charting;

namespace Imagesss
{
    public partial class HistogramForm : Form
    {
        private Bitmap image;
        private PictureBox pictureBox;
       // private Chart histogramChart;
        private TextBox transformationFunctionTextBox;
        public HistogramForm(Bitmap img)
        {
            var image = img;
            InitializeComponent();
        }
    }
}
