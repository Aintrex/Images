using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Windows.Forms.DataVisualization.Charting;

namespace Imagesss
{
    public partial class HistogramForm : Form
    {
        private Bitmap image;
        private Bitmap interImg;
        private List<PointF> points = new List<PointF>();
        float[] cubicpix = new float[256];
        public HistogramForm(Bitmap img)
        {

            InitializeComponent();
            image = img;
            pictureBox1.Image = image;
            interImg = image;
            DrawHistogram();
            this.Show();
            //if (LoadPointsFromFile())
            //{
            //    this.Show();
            //    cubicpix = CubicInter();
            //    ApplyGradationPreobr();
            //    DrawHistogram();
            //    DrawCurve();
            //}
            //else { this.Close(); }
        }

        private void DrawHistogram()
        {
            int[] N = new int[256];
            int w = image.Width;
            int h = image.Height;
            int hh = pictureBox2.Height;
            int ww = pictureBox2.Width;
            byte[] data = GetImgBytes(interImg);

            Bitmap histogram = new Bitmap(ww, hh);
            for (int i = 0; i < data.Length; i += 4)
            {
                int c = (data[i] + data[i + 1] + data[i + 2]) / 3;
                N[c]++;
            }
            int max = N.Max();
            float k = (float)hh / max;
            float j = (float)ww / 256;
            
            using (Graphics g = Graphics.FromImage(histogram))
            {
                g.Clear(Color.White);
                for (int i = 0; i < N.Length; i++)
                {
                    int x1 = (int)(i * j);
                    int x2 = (int)((i + 1) * j);
                    int y1 = hh - 1;
                    int y2 = hh - 1 - (int)(N[i] * k);
                    g.FillRectangle(Brushes.Black, x1, y2, x2 - x1, hh - y2);
                }
            }
            
            pictureBox2.Image = histogram;
        }
        private void DrawCurve()
        {
            if (points.Count < 2) return;

            Bitmap curveBitmap = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            using (Graphics g = Graphics.FromImage(curveBitmap))
            {
                g.Clear(Color.White);
                Pen pen = new Pen(Color.Blue, 2);

                PointF[] curvePoints = new PointF[256];


                for (int i = 0; i < 256; i++)
                {
                    curvePoints[i] = new PointF(i * (curveBitmap.Width / 256f), curveBitmap.Height - cubicpix[i] * (curveBitmap.Height / 255f));
                }

                g.DrawCurve(pen, curvePoints);
            }

            pictureBox3.Image = curveBitmap;
        }
        private void ApplyGradationPreobr()
        {
            if (points.Count < 2) return;

            Bitmap res = new Bitmap(image.Width, image.Height);
            byte[] data = GetImgBytes(image);
            byte[] resdata = new byte[data.Length];
            for (int i = 0; i < data.Length; i += 4)
            {
                resdata[i] = (byte)Math.Max(0, Math.Min(255, cubicpix[data[i]]));
                resdata[i + 1] = (byte)Math.Max(0, Math.Min(255, cubicpix[data[i + 1]]));
                resdata[i + 2] = (byte)Math.Max(0, Math.Min(255, cubicpix[data[i + 2]]));
                resdata[i + 3] = 255;
            }
            SetImgBytes(res, resdata);
            // Обновление PictureBox с преобразованным изображением
            pictureBox1.Image = res;
            interImg = res;
        }
        private float[] CubicInter()
        {
            int n = points.Count;
            float[] x = points.Select(p => p.X).ToArray();
            float[] y = points.Select(p => p.Y).ToArray();
            float[] h = new float[n - 1];
            float[] a = new float[n];
            float[] b = new float[n];
            float[] c = new float[n];
            float[] d = new float[n];
            for (int i = 0; i < n - 1; i++)
                h[i] = x[i + 1] - x[i];

            float[] alpha = new float[n - 1];
            float[] beta = new float[n - 1];
            float[] A3 = new float[n];
            for (int i = 1; i < n - 1; i++)
            {
                float A1 = h[i - 1];
                float A2 = 2 * (h[i - 1] + h[i]);
                A3[i] = h[i];
                float F = 6 * ((y[i + 1] - y[i]) / h[i] - (y[i] - y[i - 1]) / h[i - 1]);
                if (i == 1)
                {
                    alpha[i] = A2;
                    b[i] = F;
                }
                else
                {
                    alpha[i] = A2 - (A1 * A3[i - 1]) / alpha[i - 1];
                    beta[i] = F - beta[i - 1] * (A1) / alpha[i - 1];
                }

            }
            c[n - 1] = 0;
            for (int i = n - 2; i > 0; i--)
                c[i] = (beta[i] - h[i] * c[i + 1]) / alpha[i];

            c[0] = 0;

            for (int i = 0; i < n - 1; i++)
            {
                a[i] = y[i];
                b[i] = (y[i + 1] - y[i]) / h[i] - (h[i] * (c[i + 1] + 2 * c[i])) / 6;
                d[i] = (c[i + 1] - c[i]) / h[i];
            }
            float[] cubicpix = new float[256];

            for (int i = 0; i < 256; i++)
            {
                int segment = FindSegment(x, i);
                float xi = i - x[segment];
                cubicpix[i] = a[segment] + b[segment] * xi + c[segment] / 2 * xi * xi + d[segment] / 6 * xi * xi * xi;
                cubicpix[i] = Math.Max(0, Math.Min(255, cubicpix[i]));
            }

            return cubicpix;
        }

        private int FindSegment(float[] x, float value)
        {
            for (int i = 0; i < x.Length - 1; i++)
            {
                if (value >= x[i] && value <= x[i + 1])
                    return i;
            }
            return x.Length - 2;
        }
        private static byte[] GetImgBytes(Bitmap img)
        {
            byte[] byts = new byte[img.Width * img.Height * 4];
            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                img.PixelFormat);
            Marshal.Copy(data.Scan0, byts, 0, byts.Length);
            img.UnlockBits(data);
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
        private bool LoadPointsFromFile()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                points.Clear();
                ofd.InitialDirectory = Directory.GetCurrentDirectory();
                ofd.Filter = "Texts (txt) | *.txt";
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var line in File.ReadLines(ofd.FileName))
                    {
                        var parts = line.Split(' ');
                        if (parts.Length == 2 && float.TryParse(parts[0], out float x) && float.TryParse(parts[1], out float y))
                        {
                            points.Add(new PointF(x, y));
                        }
                        else
                        {
                            MessageBox.Show("Inccorrect points");
                            return false;
                        }
                    }
                }
            }
            points = points.OrderBy(p => p.X).ToList();
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (LoadPointsFromFile())
            {
                cubicpix = CubicInter();
                ApplyGradationPreobr();
                DrawHistogram();
                DrawCurve();
            }
        }
    }
}
