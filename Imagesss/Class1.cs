using Imagesss;
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
        public Bitmap AdaptedImg { get; set; }
        public string Name { get; set; }
        public string Channel { get; set; }
        public string Op {  get; set; }

        public float Opacity { get; set; }
        public ImgLayer(Bitmap img, string name)
        {
            Image = img;
            Name = name;
            Channel = "RGB";
            Op = "Summ";
            Opacity = 1.0f;
           
            
        }
        public void UpdChannel(string channel)
        {
            Channel = channel;
        }
        public void UpdOp(string op) 
        {
            Op = op; 
        }
    }
}
