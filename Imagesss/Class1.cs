using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgLyr
{
    public class ImgLayer
    {
        public Bitmap Image { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public ComboBox ChannelList { get; set; }
        public string Op {  get; set; }
        public ComboBox OperList { get; set; }

        public float Opacity { get; set; }
        public ImgLayer(Bitmap img, string name)
        {
            Image = img;
            Name = name;
            Channel = "RGB";
            Op = "No";
            Opacity = 1.0f;
            ChannelList = new ComboBox();
            ChannelList.DropDownStyle = ComboBoxStyle.DropDownList;
            ChannelList.Items.AddRange(new string[] { "RGB", "R", "G", "B", "RG", "RB", "GB" });
            ChannelList.SelectedIndex = 0;
            ChannelList.SelectedIndexChanged += (s, e) => Channel = ChannelList.SelectedItem.ToString();
            OperList= new ComboBox();
            OperList.DropDownStyle = ComboBoxStyle.DropDownList;
            OperList.Items.AddRange(new string[] { "No", "Summ", "Mult", "Max", "Min", "Avg"});
            OperList.SelectedIndex = 0;
            OperList.SelectedIndexChanged += (s, e) => Op = OperList.SelectedItem.ToString();
        }

    }
}
