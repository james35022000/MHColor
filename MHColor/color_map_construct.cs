using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHColor
{
    static class color_map_construct
    {
        public static Boolean[,] Color_map = new Boolean[256, 256];
        public static void construct(int R, int G, int B, int offset, List<MabinogiHeroColor.Form1.RGBinfo> database, int i, int j)
        {
            foreach (MabinogiHeroColor.Form1.RGBinfo data in database)
            {
                if (data.R - offset <= R && R <= data.R + offset &&
                    data.G - offset <= G && G <= data.G + offset &&
                    data.B - offset <= B && B <= data.B + offset)
                {
                    Color_map[i, j] = true;
                    break;
                }
                else
                    Color_map[i, j] = false;
            }
        }
    }
}
