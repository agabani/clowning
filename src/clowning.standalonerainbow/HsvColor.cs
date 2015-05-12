using System;

namespace clowning.standalonerainbow
{
    public class HsvColor
    {
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Value { get; set; }

        public RgbColor ToRgbColor()
        {
            double hue = Hue;
            double saturation = Saturation;
            double value = Value;

            while (hue < 0)
            {
                hue += 360;
            }
            while (hue >= 360)
            {
                hue -= 360;
            }

            double red, green, blue;

            if (value <= 0)
            {
                return new RgbColor
                {
                    Red = 0,
                    Blue = 0,
                    Green = 0
                };
            }

            if (saturation <= 0)
            {
                return new RgbColor
                {
                    Red = (int) value,
                    Blue = (int) value,
                    Green = (int) value
                };
            }

            double hf = hue/60.0;
            int i = (int) Math.Floor(hf);
            double f = hf - i;
            double pv = value*(1 - saturation);
            double qv = value*(1 - saturation*f);
            double tv = value*(1 - saturation*(1 - f));
            switch (i)
            {
                case 0:
                    red = value;
                    green = tv;
                    blue = pv;
                    break;
                case 1:
                    red = qv;
                    green = value;
                    blue = pv;
                    break;
                case 2:
                    red = pv;
                    green = value;
                    blue = tv;
                    break;
                case 3:
                    red = pv;
                    green = qv;
                    blue = value;
                    break;
                case 4:
                    red = tv;
                    green = pv;
                    blue = value;
                    break;
                case 5:
                    red = value;
                    green = pv;
                    blue = qv;
                    break;
                case 6:
                    red = value;
                    green = tv;
                    blue = pv;
                    break;
                case -1:
                    red = value;
                    green = pv;
                    blue = qv;
                    break;
                default:
                    red = green = blue = value;
                    break;
            }

            return new RgbColor
            {
                Red = Clamp((int) (red*255.0)),
                Green = Clamp((int) (green*255.0)),
                Blue = Clamp((int) (blue*255.0))
            };
        }

        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}