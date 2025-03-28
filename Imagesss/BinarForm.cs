using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Imagesss
{
    public partial class BinarForm : Form
    {
        private Bitmap image;
        public BinarForm(Bitmap img)
        {
            InitializeComponent();
            image = img;
            this.Show();
            
            comboBox1.SelectedIndexChanged += (s, e) =>
            {
                Binarization();
            };
        }
        public void Binarization()
        {
           // MessageBox.Show($"{3/2}");
            switch(comboBox1.Text)
            {
                case "Niblack":
                    NiblackBin();
                    break;
                case "No":
                    pictureBox1.Image = image;
                    break;
            }
            
        }

        private void NiblackBin()
        {
            byte[] data = new byte[image.Width * image.Height * 4];
            data = GetImgBytes(image);
            int a = 2;
            float k = -0.2f;
            int w = image.Width;
            int h = image.Height;
            byte[] resdata = new byte[w * h * 4];
            Bitmap res = new Bitmap(w, h);
            while (a % 2 == 0)
            {
                string input = Interaction.InputBox("Write down odd number for box:", "Input", "3");
                if (string.IsNullOrEmpty(input))
                    return;
                if (int.TryParse(input, out a) && a % 2 == 1)
                    break;
                MessageBox.Show("'a' must be odd number", "Error", MessageBoxButtons.OK);

            }

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float mx = 0;
                    float mxmx = 0;
                    int count = 0;
                    for (int dy = -a / 2; dy <= a / 2; dy++)
                    {
                        for (int dx = -a / 2; dx <= a / 2; dx++)
                        {
                            int xx = x + dx;
                            int yy = y + dy;
                            if (xx >= 0 && xx < w && yy >= 0 && yy < h)
                            {
                                int index = (yy * w + xx) * 4;

                                byte gr = (byte)(0.2125 * data[index + 0] + 0.7154 * data[index + 1] +0.0721 * data[index + 0]); //data[index];
                                mx += gr;
                                mxmx += gr * gr;
                                count++;
                                data[index] = gr;
                            }
                        }
                    }
                    if (count == 0)
                        continue;
                    mx /= count;
                    float Dx = mxmx/count - mx * mx;
                    float sigma = (float)Math.Sqrt(Dx);

                    float t = mx + k * sigma;

                    int ind = (y * w + x) * 4;

                    byte binar = (data[ind] <= t) ? (byte)(0) : (byte)(255);

                    resdata[ind + 1] = binar;
                    resdata[ind] = binar; //data[ind+1];
                    resdata[ind + 2] = binar;//data[ind + 2] ;
                    resdata[ind + 3] = data[ind + 3];
                }

            }
            SetImgBytes(res, resdata);
            pictureBox1.Image = res;

        }


        private static byte[] GetImgBytes(Bitmap img)
        {
            if (img == null || img.Width == 0 || img.Height == 0)
            {
                MessageBox.Show("Invalid image.");
                return [0,1];
            }
            byte[] byts = new byte[img.Width * img.Height * 4];
            var data1 = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                img.PixelFormat);
            Marshal.Copy(data1.Scan0, byts, 0, byts.Length);
            img.UnlockBits(data1);
            return byts;
        }

        private static void SetImgBytes(Bitmap img, byte[] bytes)
        {
            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                img.PixelFormat);
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            img.UnlockBits(data);
        }
    }
}
