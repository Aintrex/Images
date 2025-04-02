using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using ImgLyr;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;

namespace Imagesss
{
    

    public partial class Form1 : Form
    {
        private List<ImgLayer> layers = new List<ImgLayer>();
        private float MainOpacity = 1.0f;
        private TrackBar trackBar1;
        public Form1()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
         
        }

       

        private void AddImg()
        {
          
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            { 
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFileDialog.Filter = "Images (png, jpg, bmp) |*.png;*.jpg;*.bmp;|All files (*.*)|*.*\"";
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName == null)
                        return;
                    foreach (var file in openFileDialog.FileNames)
                    {
                       
                        Bitmap img = new Bitmap(file);
                        string imgname = Path.GetFileNameWithoutExtension(file);
                        if (imgname.Length > 25)
                        {
                            imgname = imgname.Substring(0, 22) + "...";
                        }
                        var lr = new ImgLayer(img, imgname);
                        layers.Add(lr);
                        AddLayerToUI(lr);
                    }
                    MainImgOpacity();
                    ImgAdapt();
                    ProceedOperation();
                }
                
            }
           
        }

       private void MainImgOpacity()
        {
            if (trackBar1 != null) return;
            trackBar1 = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 100,
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 5,
                Location = new Point(12, 512),
                Width = 300,
                Visible = true
            };
            Label MainOpcLbl = new Label
            {
                Text = "100%",
                AutoSize = true,
                Location = new Point(trackBar1.Width+20, 512),
                Visible = true
            };
            trackBar1.Scroll += (sender, e) =>
            {
                int opacity = trackBar1.Value;
                MainOpacity = opacity / 100.0f;
                MainOpcLbl.Text = $"{opacity}%";
                ProceedOperation();
                //MessageBox.Show($"Прозрачность: {layer.Opacity}%");
            };
            
            Button histBut = new Button
            {
                Text = "Show Histogram",
                Location = new Point(trackBar1.Width + 60, 512),
                Width = 200
            };
            histBut.Click += (sender, e) => HistogramWindow(sender, e, (Bitmap)pictureBox1.Image);
             Button binBut = new Button
            {
                Text = "Show binarization",
                Location=new Point(trackBar1.Width + 260, 512),
                Width = 200
            };
            binBut.Click += (sender, e) => BinWindow(sender, e, (Bitmap)pictureBox1.Image);
            this.Controls.Add(trackBar1);
            this.Controls.Add(MainOpcLbl);
            this.Controls.Add(histBut);
            this.Controls.Add(binBut);
        }

        private void AddLayerToUI(ImgLayer layer)
        {
            Panel panel = new Panel {Width = 210, Height = 360, Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle };

            PictureBox pictureBox = new PictureBox
            {
                Image = new Bitmap(layer.Image, new Size(200, 180)),
                Size = new Size(200, 180),
                Location = new Point(5, 5),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label { Text = layer.Name, AutoSize = true, Location = new Point(5, 185) };
            
            ComboBox ChannelList = new ComboBox();
            ChannelList.DropDownStyle = ComboBoxStyle.DropDownList;
            ChannelList.Items.AddRange(new string[] { "RGB", "R", "G", "B", "RG", "RB", "GB" });
            ChannelList.SelectedIndex = 0;
            ChannelList.SelectedIndexChanged += (s, e) =>
            {
                 layer.UpdChannel(ChannelList.SelectedItem.ToString());
                ProceedOperation();
            };
            ChannelList.Location = new Point(5, 210);
            ChannelList.Width = 60;
            ComboBox OperList = new ComboBox();
            OperList.DropDownStyle = ComboBoxStyle.DropDownList;
            OperList.Items.AddRange(new string[] { "No", "Summ", "Mult", "Max", "Min", "Avg" });
            OperList.SelectedIndex = 1;
            OperList.SelectedIndexChanged += (s, e) =>
            {
                layer.UpdOp( OperList.SelectedItem.ToString());
                ProceedOperation();
            };
            OperList.Location = new Point(80, 210);
            OperList.Width = 100;

            TrackBar opacityTrackBar = new TrackBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 100, 
                TickFrequency = 10,
                LargeChange = 10,
                SmallChange = 5,
                Location = new Point(5, 250),
                Width = 160
            };

            Label opacityLabel = new Label
            {
                Text = "100%",
                AutoSize = true,
                Location = new Point(170, 250)
            };
            opacityTrackBar.Scroll += (sender, e) =>
            {
                int opacity = opacityTrackBar.Value;
                layer.Opacity = opacity / 100.0f; 
                opacityLabel.Text = $"{opacity}%";
                ProceedOperation();
                //MessageBox.Show($"Прозрачность: {layer.Opacity}%");
            };

            Button histogramButton = new Button
            {
                Text = "Show Histogram",
                Location = new Point(5, 300),
                Width = 200
            };
            histogramButton.Click += (sender, e) => HistogramWindow(sender, e, layer.Image);

            Button binButton = new Button
            {
                Text = "Show binarization",
                Location=new Point(5, 330),
                Width = 200
            };
            binButton.Click += (sender, e) => BinWindow(sender, e, layer.Image);
            panel.Controls.Add(pictureBox);
            panel.Controls.Add(label);
            panel.Controls.Add(ChannelList);
            panel.Controls.Add(OperList);
            panel.Controls.Add(opacityTrackBar);
            panel.Controls.Add(opacityLabel);
            panel.Controls.Add(histogramButton);
            panel.Controls.Add(binButton);
            lyrBox2.Controls.Add(panel);
        }
        private void BinWindow(object sender, EventArgs e, Bitmap pb)
        {
            Bitmap img = new Bitmap(pb);
            
                this.Hide();

               
                BinarForm binarForm = new BinarForm(img);

                binarForm.FormClosed += (s, args) => this.Show();
        }
         private void  HistogramWindow(object sender, EventArgs e, Bitmap picturebox)
        {
            Bitmap img = new Bitmap(picturebox);
            this.Hide();
            HistogramForm histogramForm = new HistogramForm(img);
            histogramForm.FormClosed += (s, args) => this.Show();
        }
        private static Bitmap Resize(Bitmap img, int nw, int nh)
        {
            Bitmap imgr = new Bitmap(nw, nh);
            using (Graphics g = Graphics.FromImage(imgr))
            {
                g.DrawImage(img, 0, 0, nw, nh);
            }
            return imgr;
        }
        public void ImgAdapt()
        {
            //if (layers.Count != 2)
            //    return;
            //int mw = 0;
            //int mh = 0;
            //foreach (ImgLayer layer in layers)
            //{
            //    if (layer.Image.Width > mw) mw = layer.Image.Width;
            //    if (layer.Image.Height > mh) mh = layer.Image.Height;
            //}
            //foreach (ImgLayer layer in layers)
            //{
            //    if (layer.Image.Width != mw || layer.Image.Height != mh)
            //        layer.Image = Resize(layer.Image, mw, mh);
            //}
            foreach (var lyr in layers)
            {
                //lyr.Image = Resize(lyr.Image, pictureBox1.Width, pictureBox1.Height);
                lyr.AdaptedImg = Resize(lyr.Image, 1920, 1080);
            }
        }
        Bitmap svimg = default;
        public void ProceedOperation()
        {
            //  Bitmap res = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Bitmap res = new Bitmap(1920, 1080);
            Graphics.FromImage(res).Clear(Color.Black);
            
            int w = layers[0].AdaptedImg.Width;
            int h = layers[0].AdaptedImg.Height;
            byte[] resb = new byte[w*h*4];// *4 так как каждый пиксель=4 байта
           // for (int k = layers.Count - 1; k >= 0; k--)
           foreach (var layer in layers.AsEnumerable().Reverse())
            {
                byte[] layb = GetImgBytes(layer.AdaptedImg);
                float opacity = layer.Opacity;
                var op = layer.Op;
                var ch = layer.Channel;
                ApplyLayer(resb, layb, w, h, opacity, op, ch);
            }
           SetImgBytes(res, resb);
            svimg = res;
            pictureBox1.Image = res;
            
            
        }
        protected void ApplyLayer(byte[] resultData, byte[] layerData, int w, int h, float opacity, string op, string ch)
        {
            int l = w * h;
            Parallel.For(0, l, i =>
            {
                int index = i * 4;// опять же, пиксель 4 байта 0 1 2 3 для нулевого. Хранится BGRA, а не RGBA, поэтому такие индексы)
                byte b1 = layerData[index];
                byte g1 = layerData[index + 1];
                byte r1 = layerData[index + 2];

                Channels(ch, ref r1, ref g1, ref b1);

                byte b2= resultData[index];
                byte g2 = resultData[index + 1];
                byte r2=resultData[index + 2];

                byte r = Operations((byte)(r1*opacity), r2, op);
                byte g = Operations((byte)(g1 * opacity), g2, op);
                byte b = Operations((byte)(b1 * opacity), b2, op);
                Console.WriteLine($"{i}");

                resultData[index] = b;
                resultData[index+1] = g;
                resultData[index+2] = r;
                resultData[index + 3] = (byte)(255*MainOpacity); //(byte)(255*opacity);
            });
        }
        private static byte[] GetImgBytes(Bitmap img)
        {
            byte[] byts = new byte[img.Width * img.Height*4];
            var data = img.LockBits(new Rectangle(0,0,img.Width,img.Height),
                ImageLockMode.ReadOnly,
                img.PixelFormat);
            Marshal.Copy(data.Scan0, byts, 0, byts.Length);
            img.UnlockBits(data);
            return byts;
        }

        private static void SetImgBytes(Bitmap img, byte[] bytes)
        {
            var data = img.LockBits(new Rectangle(0,0,img.Width,img.Height),
                ImageLockMode.ReadOnly,
                img.PixelFormat);
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            img.UnlockBits(data);
        }
        private static void Channels(string ch, ref byte r, ref byte g, ref byte b)
        {
            switch (ch)
            {
                case "R":
                    g = 0; b = 0;
                    break;
                case "G":
                    r = 0; b = 0;
                    break;
                case "B":
                    r = 0; g = 0;
                    break;
                case "RG":
                    b = 0;
                    break;
                case "RB":
                    g = 0;
                    break;
                case "GB":
                    r = 0;
                    break;
                default:
                    break;
            }
        }
        private static byte Operations(byte p1, byte p2, string op)
        {
            return op switch
            {
                "Summ" => (byte)Math.Min(255, p1 + p2),
                "Mult" => (byte)((p1 * p2) / 255),
                "Max" => (byte)Math.Max(p1, p2),
                "Min" => (byte)Math.Min(p1, p2),
                "Avg" => (byte)((p1 + p2) / 2),
                _ => p2
            };

        }

        private void SaveImg()
        {
            svimg = Resize(svimg, 1920, 1080);
            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveFileDialog.Filter = "PNG Image|*.png";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
               // pictureBox1.Image.Save(saveFileDialog.FileName);
               svimg.Save(saveFileDialog.FileName);
            }
        }

       

        private void AddImage_Click(object sender, EventArgs e)
        {
            AddImg();
           // ImgAdapt();
        }

        private void proButton_Click(object sender, EventArgs e)
        {
            int k = layers.Count - 1;
           // MessageBox.Show($"Перед операцией adapt: ch1 = {layers[k].ChannelList.SelectedItem?.ToString()}");
            
           // MessageBox.Show($"Перед операцией adapt: ch1 = {layers[k].Opacity}");
            ProceedOperation();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveImg();
        }
    }
}
