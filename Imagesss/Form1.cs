using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using ImgLyr;

namespace Imagesss
{


    public partial class Form1 : Form
    {
        private List<ImgLayer> layers = new List<ImgLayer>();
        public Form1()
        {
            InitializeComponent();
        }


        private void AddImg()
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Images (png, jpg, bmp) |*.png;*.jpg;*.bmp;|All files (*.*)|*.*\"";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in openFileDialog.FileNames)
                {
                    Bitmap img = new Bitmap(file);
                    string imgname = Path.GetFileNameWithoutExtension(file);
                    if (imgname.Length > 15)
                    {
                        imgname = imgname.Substring(0, 12) + "...";
                    }
                    var lr = new ImgLayer(img, imgname);
                    layers.Add(lr);
                    AddLayerToUI(lr);
                }
                ////LayersBox.Items.Add(imgname);
                //Bitmap thumbnail = new Bitmap(img, new Size(50, 50));
                //imageList.Images.Add(imgname, thumbnail);

                //// Добавляем элемент в ListView
                //ListViewItem item = new ListViewItem { Text = imgname, ImageIndex = imageList.Images.Count - 1 };
                //layersListView.Items.Add(item); 952, 243
                //Redraw();
            }
        }

        private void AddLayerToUI(ImgLayer layer)
        {
            Panel panel = new Panel {Width = 230, Height = 300, Dock = DockStyle.Top, BorderStyle = BorderStyle.FixedSingle };

            PictureBox pictureBox = new PictureBox
            {
                Image = new Bitmap(layer.Image, new Size(220, 180)),
                Size = new Size(220, 180),
                Location = new Point(5, 5),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label label = new Label { Text = layer.Name, AutoSize = true, Location = new Point(5, 185) };

            layer.ChannelList.Location = new Point(5, 210);
            layer.ChannelList.Width = 60;

            layer.OperList.Location = new Point(80, 210);
            layer.OperList.Width = 100;

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
            };

            Button histogramButton = new Button
            {
                Text = "Show Histogram",
                Location = new Point(5, 260),
                Width = 100
            };
            histogramButton.Click += (sender, e) => HistogramWindow(sender, e, pictureBox);

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(label);
            panel.Controls.Add(layer.ChannelList);
            panel.Controls.Add(layer.OperList);
            panel.Controls.Add(opacityTrackBar);
            panel.Controls.Add(opacityLabel);
            panel.Controls.Add(histogramButton);
            lyrBox2.Controls.Add(panel);
        }
        private void HistogramWindow(object sender, EventArgs e, PictureBox picturebox)
        {
            Bitmap img = new Bitmap(picturebox.Image);

            HistogramForm histogramForm = new HistogramForm(img);
            histogramForm.Show();
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
                lyr.Image = Resize(lyr.Image, 1920, 1080);
            }
        }
        Bitmap svimg = default;
        private void ProceedOperation()
        {
            //  Bitmap res = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Bitmap res = new Bitmap(1920, 1080);
            Graphics.FromImage(res).Clear(Color.Black);
            for (int k = layers.Count - 1; k >= 0; k--)
            {
                var Opp = layers[k].Op;
                int w = layers[0].Image.Width;
                int h = layers[0].Image.Height;
                var img1=layers[k].Image;
                var img2 = res;
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        Color pix1 = img1.GetPixel(j, i);
                        Color pix2 = img2.GetPixel(j, i);

                        string ch = layers[k].Channel;
                        pix1 = Channels(ch, pix1);
                       // pix1 = Color.FromArgb((int)(pix1.A * layers[k].Opacity), pix1.R, pix1.G, pix1.B);
                        var r = Operations((int)(pix1.R * layers[k].Opacity), pix2.R, Opp);
                        var g = Operations((int)(pix1.G * layers[k].Opacity), pix2.G, Opp);
                        var b = Operations((int)(pix1.B * layers[k].Opacity), pix2.B, Opp);
                        res.SetPixel(j, i, Color.FromArgb(r, g, b));
                        svimg = res;
                    }
                }
            }
            pictureBox1.Image = res;
            
        }
        private static Color Channels(string ch, Color pix)
        {
            return ch switch
            {
                "R" => Color.FromArgb(pix.R, 0, 0),
                "G" => Color.FromArgb(0, pix.G, 0),
                "B" => Color.FromArgb(0, 0, pix.B),
                "RG" => Color.FromArgb(pix.R, pix.G, 0),
                "RB" => Color.FromArgb(pix.R, 0, pix.B),
                "GB" => Color.FromArgb(0, pix.G, pix.B),
                _ => pix
            };
        }
        private static int Operations(int p1, int p2, string op)
        {
            return op switch
            {
                "Summ" => Math.Min(255, p1 + p2),
                "Mult" => (p1 * p2) / 255,
                "Max" => Math.Max(p1, p2),
                "Min" => Math.Min(p1, p2),
                "Avg" => (p1 + p2) / 2,
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
        }

        private void proButton_Click(object sender, EventArgs e)
        {
            int k = layers.Count - 1;
           // MessageBox.Show($"Перед операцией adapt: ch1 = {layers[k].ChannelList.SelectedItem?.ToString()}");
            ImgAdapt();
           // MessageBox.Show($"Перед операцией adapt: ch1 = {layers[k].Opacity}");
            ProceedOperation();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveImg();
        }
    }
}
