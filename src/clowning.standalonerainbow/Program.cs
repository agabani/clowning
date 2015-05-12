using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using clowning.blyncclient;

namespace clowning.standalonerainbow
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int i = 120;
            if (args.Length == 1)
            {
                i = int.Parse(args[0]);
            }
            var client = new BlyncClient();

            var hsvColor = new HsvColor
            {
                Hue = 1,
                Saturation = 1,
                Value = 1
            };

            while (true)
            {
                var rgbColor = hsvColor.ToRgbColor();
                client.SetColor(0, rgbColor.Red, rgbColor.Green, rgbColor.Blue);

                hsvColor.Hue++;
                if (hsvColor.Hue > 360)
                {
                    hsvColor.Hue = 1;
                }

                Thread.Sleep((i*1000)/360);
            }
        }
    }
}