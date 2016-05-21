using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicRate
{
    class RateableImage
    {
        public readonly Image Image;
        public int? Score;

        public RateableImage(Image image, int? score = null)
        {
            Image = image;
            Score = score;
        }
    }
}
