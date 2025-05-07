using Accessibility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Imagesss
{
    public partial class Form2 : Form
    {
        private Bitmap img;
        public Form2(Bitmap image)
        {
            img = image;
            InitializeComponent();
            this.Show();
            pictureBox1.Image = img;
        }

        private async void Median()
        {
            if (int.TryParse(textBox1.Text, out int msize))
            {
                int w = img.Width;
                int h = img.Height;
                byte[] data = GetImgBytes(img);
                Bitmap res = new Bitmap(w, h);
                byte[] resdata = new byte[w * h * 4];

                Stopwatch time = new Stopwatch();
                time.Start();

                await Task.Run(() =>
                {
                    Parallel.For(0, h, i =>
                    {
                        int[] R = new int[msize * msize];
                        int[] G = new int[msize * msize];
                        int[] B = new int[msize * msize];

                        for (int j = 0; j < w; j++)
                        {
                            for (int ii = 0; ii < msize; ii++)
                            {
                                for (int jj = 0; jj < msize; jj++)
                                {
                                    int _i = i + ii - msize / 2;
                                    if (_i < 0) _i *= -1;
                                    if (_i >= h) _i = 2 * h - _i - 1;

                                    int _j = j + jj - msize / 2;
                                    if (_j < 0) _j *= -1;
                                    if (_j >= w) _j = 2 * w - _j - 1;

                                    int srcIndex = (_i * w + _j) * 4;
                                    R[ii * msize + jj] = data[srcIndex + 2];
                                    G[ii * msize + jj] = data[srcIndex + 1];
                                    B[ii * msize + jj] = data[srcIndex + 0];
                                }
                            }

                            int dstIndex = (i * w + j) * 4;
                            resdata[dstIndex + 0] = (byte)quickselect2(B, 0, msize * msize, msize * msize / 2);
                            resdata[dstIndex + 1] = (byte)quickselect2(G, 0, msize * msize, msize * msize / 2);
                            resdata[dstIndex + 2] = (byte)quickselect2(R, 0, msize * msize, msize * msize / 2);
                            resdata[dstIndex + 3] = 255;
                        }
                    });
                });

                SetImgBytes(res, resdata);
                pictureBox1.Image = res;
                DrawHistogram(res);

                time.Stop();
                label3.Text = time.ElapsedMilliseconds.ToString() + "ms";
            }
            else
            {
                MessageBox.Show("Matrix size is incorrect");
            }
        }


        private async void MaskFilt()
        {
            try
            {
                if (int.TryParse(textBox1.Text, out int msize))
                {
                    if (msize % 2 != 0)
                    {

                        int w = img.Width;
                        int h = img.Height;
                        byte[] data = GetImgBytes(img);
                        Bitmap res = new Bitmap(w, h);
                        byte[] resdata = new byte[w * h * 4];
                        int half = msize / 2;
                        var arr = GetArrFromGrid();
                        Stopwatch time = new Stopwatch();
                        time.Start();
                        await Task.Run(() =>
                        {
                            Parallel.For(0, h, i =>
                            {
                                for (int j = 0; j < w; j++)
                                {
                                    double s1 = 0, s2 = 0, s3 = 0;

                                    for (int ii = 0; ii < msize; ii++)
                                    {
                                        int _i = i + ii - msize / 2;
                                        if (_i < 0) _i *= -1;
                                        if (_i >= h) _i = 2 * h - _i - 1;

                                        for (int jj = 0; jj < msize; jj++)
                                        {
                                            int _j = j + jj - msize / 2;
                                            if (_j < 0) _j *= -1;
                                            if (_j >= w) _j = 2 * w - _j - 1;

                                            int indx = (_i * w + _j) * 4;
                                            s1 += data[indx + 2] * arr[ii, jj];
                                            s2 += data[indx + 1] * arr[ii, jj];
                                            s3 += data[indx + 0] * arr[ii, jj];
                                        }
                                    }
                                    int dstIndex = (i * w + j) * 4;
                                    resdata[dstIndex + 2] = ClampToByte(s1);
                                    resdata[dstIndex + 1] = ClampToByte(s2);
                                    resdata[dstIndex + 0] = ClampToByte(s3);
                                    resdata[dstIndex + 3] = 255;
                                }
                            });
                        });

                        SetImgBytes(res, resdata);
                        pictureBox1.Image = res;
                        time.Stop();
                        label3.Text = time.ElapsedMilliseconds.ToString() + "ms";
                        DrawHistogram(res);
                    }
                    else
                    {
                        MessageBox.Show("Matrix size should be an odd number");
                    }
                }
                else
                {
                    MessageBox.Show("Incorrect matrix size");
                }
            }
            catch
            {
                MessageBox.Show("Error!\nCheck your matrix size input and create it");
            }
        }


        private void DrawHistogram(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            int[] N = new int[256];
            int hh = pictureBox2.Height;
            int ww = pictureBox2.Width;
            byte[] data = GetImgBytes(img);
            Bitmap histogram = new Bitmap(ww, hh);

            for (int i = 0; i < data.Length; i+=4)
            {
                int c = (data[i] + data[i + 1] + data[i + 2])/3;
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
                    int x2 = (int)((i + 1)*j);
                    int y1 = hh - 1;
                    int y2 = hh - 1-(int)(N[i]*k);
                    g.FillRectangle(Brushes.Black, x1, y2, x2 - x1, hh - y2);
                }
            }
            pictureBox2.Image = histogram;
        }
        private byte ClampToByte(double value)
        {
            return (byte)Math.Max(0, Math.Min(255, value));
        }
        private double[,] GetArrFromGrid()
        {
            int rows = dataGridView1.RowCount;
            int cols = dataGridView1.ColumnCount;
            double[,] arr = new double[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (double.TryParse(dataGridView1[j, i].Value?.ToString(), out double val))
                        arr[i, j] = val;
                    else
                        arr[i, j] = 0; // на случай, если в ячейке не число
                }
            }

            return arr;
        }

        private void GenMatrix()
        {
            if (int.TryParse(textBox1.Text, out int size))
            {

            }
            else
            {
                MessageBox.Show("Incorrect matrix size");
                return;
            }
            if (size % 2 != 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.Rows.Clear();
                // dataGridView1.Columns.Add(size);

                for (int i = 0; i < size; i++)
                {
                    dataGridView1.Columns.Add($"col{i}", "");
                    dataGridView1.Columns[i].Width = 80;
                }
                dataGridView1.Rows.Add(size);
                for (int i = 0; i < size; i++)
                {
                    dataGridView1.Rows[i].Height = 40;
                }
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                            dataGridView1[i, j].Value = 0;
                    }
                }
            }
            else MessageBox.Show("Matrix size should be an odd number");
        }
        private void Gauss()
        {
           
            if (int.TryParse(textBox1.Text, out int msize) && int.TryParse(textBox2.Text, out int sigma))
            {
                if (msize % 2 != 0)
                {
                    double[,] arr = GaussGen(msize, sigma);
                    for (int i = 0; i < arr.GetLength(0); i++)
                    {
                        for (int j = 0; j < arr.GetLength(1); j++)
                        {
                            dataGridView1[j, i].Value = arr[i, j];
                        }
                    }
                }
                else MessageBox.Show("Matrix size should be an odd number");
            }
        }
        private static double[,] GaussGen(int msize, int sigma)
        {
            double[,] arr = new double[msize, msize];
            double s = 0;
            int half = msize / 2;
            double twopisigkv = 2 * 3.14 * sigma * sigma;
            double twosigkv = 2 * sigma * sigma;
            for (int i = -half; i <= half; i++)
            {
                for (int j = -half; j <= half; j++)
                {
                    arr[i + half, j + half] = Math.Round(1.0 / twopisigkv * Math.Exp(-(i * i + j * j) / twosigkv), 5);
                    s += arr[i + half, j + half];
                }
            }
            return arr;
        }

        private static int quickselect2(int[] arr, int left, int right, int k)
        {
            if (right - left == 1)
                return arr[left];

            int left_count = 0;
            int eqv_count = 0;
            int tmp = 0;

            for (int i = left; i < right - 1; ++i)
            {
                if (arr[i] < arr[right - 1])
                {
                    tmp = arr[i];
                    arr[i] = arr[left + left_count];
                    arr[left + left_count] = tmp;
                    left_count++;
                }
            }
            for (int i = left + left_count; i < right - 1; ++i)
            {
                if (arr[i] == arr[right - 1])
                {
                    tmp = arr[i];
                    arr[i] = arr[left + left_count + eqv_count];
                    arr[left + left_count + eqv_count] = tmp;
                    eqv_count++;
                }
            }
            tmp = arr[right - 1];
            arr[right - 1] = arr[left + left_count + eqv_count];
            arr[left + left_count + eqv_count] = tmp;


            if (k < left_count)
                return quickselect2(arr, left, left + left_count, k);
            else if (k < left_count + eqv_count)
                return arr[left + left_count];
            else
                return quickselect2(arr, left + left_count + eqv_count, right, k - left_count - eqv_count);

        }
        private static byte[] GetImgBytes(Bitmap img)
        {
            if (img == null || img.Width == 0 || img.Height == 0)
            {
                MessageBox.Show("Invalid image.");
                return [0, 1];
            }
            byte[] byts = new byte[img.Width * img.Height * 4];
            var data1 = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
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

        private async void button1_Click(object sender, EventArgs e)
        {


                // код, который выполняется в фоновом потоке
                Median();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            GenMatrix();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            GenMatrix();
            Gauss();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            //GenMatrix();
            MaskFilt();
        }
    }
}



//private int QuickSort(int[] arr, int lh, int rh)
//{
//    int i = lh;
//    int j = rh;
//    int tmp;
//    int pivot = arr[i];
//    while (i <= j)
//    {
//        while (arr[i] < pivot)
//            i++;
//        while (arr[j] >= pivot)
//            j--;
//        if (i <= j)
//        {
//            if (arr[i] >= arr[j])
//            {
//                tmp = arr[i];
//                arr[i] = arr[j];
//                arr[j] = tmp;
//                i++;
//                j--;
//            }
//        }
//    }
//    if (lh < j) return QuickSort(arr, lh, j);
//    if (i < rh) return QuickSort(arr, i, rh);
//    return arr[arr.Length / 2];
//}
