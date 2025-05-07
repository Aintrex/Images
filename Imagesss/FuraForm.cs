using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using fft2d;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Diagnostics;

namespace Imagesss
{
    public partial class FuraForm : Form
    {
        Bitmap img;

        public FuraForm(Bitmap image)
        {
            this.Show();
            img = image;
            InitializeComponent();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Получаем выбранное значение из ComboBox
            string selectedFilter = comboBox1.SelectedItem?.ToString();

            // Устанавливаем дефолтные значения для TextBox в зависимости от выбранного фильтра
            if (selectedFilter == "Идеальный")
            {
                textBox1.Text = "0,0,0,10"; // Пример значений для идеального фильтра
            }
            else if (selectedFilter == "Баттерворт ФВЧ" || selectedFilter == "Баттерворт ФНЧ" )
            {
                textBox1.Text = "0,0,-10,10"; // Пример значений для фильтра Баттерворта
            }
            else if (selectedFilter == "Гаусс ФВЧ" || selectedFilter == "Гаусс ФНЧ")
            {
                textBox1.Text = "0,0,-10, 10"; // Пример значений для гауссовского фильтра
            }
            else
            {
                textBox1.Text = "0,0,50,100"; // По умолчанию
            }
        }

        private static byte[] GetImgBytes(Bitmap img)
        {
            if (img == null || img.Width == 0 || img.Height == 0)
            {
                MessageBox.Show("Invalid image.");
                return new byte[0];
            }
            byte[] byts = new byte[img.Width * img.Height * 3]; // Используем 3 байта для RGB
            var data1 = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb); // Используем Format24bppRgb для 3 байт
            Marshal.Copy(data1.Scan0, byts, 0, byts.Length);
            img.UnlockBits(data1);
            return byts;
        }

        private static void SetImgBytes(Bitmap img, byte[] byts)
        {
            var data1 = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb); // Используем Format24bppRgb для 3 байт
            Marshal.Copy(byts, 0, data1.Scan0, byts.Length);
            img.UnlockBits(data1);
        }

        public static double F(double x)
        {
            return Math.Log(x + 1);
        }

        public static double Butter(double x, double y, double wx, double n, double dx = 0, double dy = 0, double G = 1.0, double h = 0)
        {
            double D = Math.Sqrt((x - dx) * (x - dx) + (y - dy) * (y - dy)) - h;
            return G / (1 + Math.Pow(D / wx, 2 * n));
        }

        public static double Gauss(double x, double y, double wx, double dx = 0, double dy = 0, double G = 1.0, double h = 0)
        {
            double D = Math.Sqrt((x - dx) * (x - dx) + (y - dy) * (y - dy)) - h;
            return G * Math.Exp(-(D * D / (2.0 * wx * wx)));
        }

        public static (Bitmap, Bitmap, Bitmap) Fura(
            Bitmap input,
            string filter_type,
            string filter,
            double in_filter_zone = 1.0,
            double out_filter_zone = 0.0,
            double furier_multiplyer = 1.0
        )
        {
            int width = input.Width;
            int height = input.Height;

            int new_width = width;
            int new_height = height;

            var p = Math.Log2(width);
            if (p != Math.Floor(p))
                new_width = (int)Math.Pow(2, Math.Ceiling(p));
            p = Math.Log2(height);
            if (p != Math.Floor(p))
                new_height = (int)Math.Pow(2, Math.Ceiling(p));

            using Bitmap _tmp = new Bitmap(new_width, new_height, PixelFormat.Format24bppRgb);
            _tmp.SetResolution(input.HorizontalResolution, input.VerticalResolution);

            byte[] new_bytes = new byte[new_width * new_height * 3];
            byte[] furier_ma_bytes = new byte[new_width * new_height * 3];
            byte[] filter_bytes = new byte[new_width * new_height * 3];

            using Graphics g = Graphics.FromImage(_tmp);
            g.DrawImageUnscaled(input, 0, 0);

            byte[] old_bytes = GetImgBytes(_tmp);

            var ss = StringSplitOptions.RemoveEmptyEntries;
            var filter_params_strings = filter.Split("\n", ss);
            filter_params_strings = filter_params_strings.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            var cult = new CultureInfo("en-US");
            var filter_params_double = filter_params_strings.Select(a => a.Split(",", ss)
                .Select(b => Convert.ToDouble(b.Trim(), cult)).ToArray()).ToArray();

            Complex[] complex_bytes = new Complex[new_width * new_height];

            for (int color = 0; color <= 2; color++)
            {
                for (int i = 0; i < new_width * new_height; ++i)
                {
                    int y = i / new_width;
                    int x = i - y * new_width;
                    complex_bytes[i] = Math.Pow(-1, x + y) * old_bytes[i * 3 + color];
                }

                complex_bytes = FFT.ditfft2d(complex_bytes, new_width, new_height);

                var max_ma = complex_bytes.Max(x => F(x.Imaginary));

                Complex[] complex_bytes_filtered = null;

                // Применение фильтров
                if (filter_type == "Идеальный") // идеальный
                {
                    complex_bytes_filtered = complex_bytes.Select((a, i) =>
                    {
                        int y = i / new_width;
                        int x = i - y * new_width;
                        y -= new_height / 2;
                        x -= new_width / 2;

                        foreach (var v in filter_params_double)
                        {
                            if ((x - v[0]) * (x - v[0]) + (y - v[1]) * (y - v[1]) >= v[2] * v[2] &&
                                (x - v[0]) * (x - v[0]) + (y - v[1]) * (y - v[1]) <= v[3] * v[3])
                            {
                                filter_bytes[i * 3 + color] = clmp(255 * in_filter_zone);
                                return a * in_filter_zone;
                            }
                        }
                        filter_bytes[i * 3 + color] = clmp(255 * out_filter_zone);
                        return a * out_filter_zone;
                    }).ToArray();
                }
                else
                if (filter_type == "Баттерворт ФНЧ") //Баттерворта ФНЧ
                {
                    complex_bytes_filtered = complex_bytes.Select((a, i) =>
                    {
                        var val = filter_params_double.Select(v =>
                        {
                            int y = i / new_width;
                            int x = i - y * new_width;
                            y -= new_height / 2;
                            x -= new_width / 2;

                            double wc = 0.5 * v[3] - 0.5 * v[2];
                            double h = v[3] - wc;
                            double b = Butter(x, y, wc, (int)out_filter_zone, v[0], v[1], in_filter_zone, h);
                            return b;
                        }).Max();
                        filter_bytes[i * 3 + color] = clmp(255 * val);
                        return a * val;
                    }).ToArray();
                }
                else if (filter_type == "Баттерворт ФВЧ")          //Баттерворта ФВЧ
                {
                    complex_bytes_filtered = complex_bytes.Select((a, i) =>
                    {
                        var val = filter_params_double.Select(v =>
                        {
                            int y = i / new_width;
                            int x = i - y * new_width;
                            y -= new_height / 2;
                            x -= new_width / 2;

                            double wc = 0.5 * v[3] - 0.5 * v[2];
                            double h = v[3] - wc;
                            double b = in_filter_zone - Butter(x, y, wc, (int)out_filter_zone, v[0], v[1], in_filter_zone, h);
                            return b;
                        }).Min();
                        filter_bytes[i * 3 + color] = clmp(255 * val);
                        return a * val;
                    }).ToArray();
                }
                else if (filter_type == "Гаусс ФНЧ")  //Гаусса ФНЧ
                {
                    complex_bytes_filtered = complex_bytes.Select((a, i) =>
                    {
                        var val = filter_params_double.Select(v =>
                        {
                            int y = i / new_width;
                            int x = i - y * new_width;
                            y -= new_height / 2;
                            x -= new_width / 2;
                            double wc = 0.5 * v[3] - 0.5 * v[2];
                            double h = v[3] - wc;
                            double b = Gauss(x, y, wc, v[0], v[1], in_filter_zone, h);
                            return b;
                        }).Max();
                        filter_bytes[i * 3 + color] = clmp(255 * val);
                        return a * val;
                    }).ToArray();
                }
                else if (filter_type == "Гаусс ФВЧ") //Гаусса ФВЧ
                {
                    complex_bytes_filtered = complex_bytes.Select((a, i) =>
                    {

                        var val = filter_params_double.Select(v =>
                        {
                            int y = i / new_width;
                            int x = i - y * new_width;
                            y -= new_height / 2;
                            x -= new_width / 2;
                            double wc = 0.5 * v[3] - 0.5 * v[2];
                            double h = v[3] - wc;
                            double b = in_filter_zone - Gauss(x, y, wc, v[0], v[1], in_filter_zone, h);
                            return b;
                        }).Min();
                        filter_bytes[i * 3 + color] = clmp(255 * val);
                        return a * val;
                    }).ToArray();
                }


                var complex_bytes_result = FFT.ditifft2d(complex_bytes_filtered, new_width, new_height);

                for (int i = 0; i < new_width * new_height; ++i)
                {
                    int y = i / new_width;
                    int x = i - y * new_width;
                    y -= new_height / 2;
                    x -= new_width / 2;
                    new_bytes[i * 3 + color] = clmp(Math.Round((Math.Pow(-1, x + y) * complex_bytes_result[i]).Real));
                    furier_ma_bytes[i * 3 + color] = clmp(furier_multiplyer * F(complex_bytes[i].Magnitude) * 255 / max_ma);
                }
            }

            //формируем восстановленное изображение
            using Bitmap new_bitmap = new Bitmap(new_width, new_height, PixelFormat.Format24bppRgb);
            new_bitmap.SetResolution(input.HorizontalResolution, input.VerticalResolution);
            SetImgBytes(new_bitmap, new_bytes);

            //рисуем восстановленное изображение на новом, размер которого совпадает с исходным
            Bitmap new_bitamp_ret = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            new_bitamp_ret.SetResolution(input.HorizontalResolution, input.VerticalResolution);
            using (Graphics g1 = Graphics.FromImage(new_bitamp_ret))
            {
                g1.DrawImageUnscaled(new_bitmap, 0, 0);
            }

            //рисуем Фурье-образ и рисуем на нем оверлеи.
            Bitmap new_bitmap_re = new Bitmap(new_width, new_height, PixelFormat.Format24bppRgb);
            new_bitmap_re.SetResolution(input.HorizontalResolution, input.VerticalResolution);
            SetImgBytes(new_bitmap_re, furier_ma_bytes);
            using var g_fur = Graphics.FromImage(new_bitmap_re);
            foreach (var v in filter_params_double)
            {
                g_fur.DrawEllipse(Pens.GreenYellow, (int)v[0] - (int)v[2] + new_width / 2, (int)v[1] - (int)v[2] + new_height / 2, (int)v[2] * 2, (int)v[2] * 2);
                g_fur.DrawEllipse(Pens.GreenYellow, (int)v[0] - (int)v[3] + new_width / 2, (int)v[1] - (int)v[3] + new_height / 2, (int)v[3] * 2, (int)v[3] * 2);
            }

            //рисуем маску фильтра
            Bitmap new_bitmap_mask = new Bitmap(new_width, new_height, PixelFormat.Format24bppRgb);
            new_bitmap_mask.SetResolution(input.HorizontalResolution, input.VerticalResolution);
            SetImgBytes(new_bitmap_mask, filter_bytes);

            return (new_bitamp_ret, new_bitmap_re, new_bitmap_mask);
        }

        static byte clmp(double d)
        {
            if (d > 255)
                return 255;
            if (d < 0)
                return 0;
            return (byte)d;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ftype = comboBox1.Text;
            string frtype = comboBox2.Text;
            string filparams = textBox1.Text;

            var filterParamsArray = filparams.Split(',').Select(val => double.Parse(val.Trim())).ToArray();
            Stopwatch time = new Stopwatch();
            time.Start();
            var result = Fura(img, ftype, filparams);
            time.Stop();
            label1.Text=time.ElapsedMilliseconds.ToString()+"ms";
           // pictureBoxSpectrum.Image = result.Item2; // Спектр
            pictureBoxFilter.Image = result.Item3; // Фильтр
            pictureBoxFiltered.Image = result.Item1;
        }
    }
}
