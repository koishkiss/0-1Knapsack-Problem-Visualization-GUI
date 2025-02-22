using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.bag
{
    internal class Random_Generator
    {
        public static Random random = new Random();
        public static Color[] darkColorCollection;
        public static int darkColorCollectionCur = 0;

        static Random_Generator()
        {
            // 生成23种不同的颜色，Y值在 0 - 180 间
            darkColorCollection = new Color[23];
            darkColorCollection[0] = Color.FromRgb(0, 0, 0);
            darkColorCollection[1] = Color.FromRgb(0, 0, 110);
            darkColorCollection[2] = Color.FromRgb(0, 0, 223);
            darkColorCollection[3] = Color.FromRgb(0, 114, 0);
            darkColorCollection[4] = Color.FromRgb(0, 114, 114);
            darkColorCollection[5] = Color.FromRgb(0, 114, 227);
            darkColorCollection[6] = Color.FromRgb(0, 225, 0);
            darkColorCollection[7] = Color.FromRgb(0, 225, 116);
            darkColorCollection[8] = Color.FromRgb(0, 225, 245);
            darkColorCollection[9] = Color.FromRgb(120, 0, 0);
            darkColorCollection[10] = Color.FromRgb(120, 0, 124);
            darkColorCollection[11] = Color.FromRgb(120, 0, 235);
            darkColorCollection[12] = Color.FromRgb(120, 117, 0);
            darkColorCollection[13] = Color.FromRgb(120, 117, 122);
            darkColorCollection[14] = Color.FromRgb(120, 117, 248);
            darkColorCollection[15] = Color.FromRgb(120, 234, 0);
            darkColorCollection[16] = Color.FromRgb(120, 234, 125);
            darkColorCollection[17] = Color.FromRgb(247, 0, 0);
            darkColorCollection[18] = Color.FromRgb(247, 0, 113);
            darkColorCollection[19] = Color.FromRgb(247, 0, 238);
            darkColorCollection[20] = Color.FromRgb(247, 121, 0);
            darkColorCollection[21] = Color.FromRgb(247, 121, 116);
            darkColorCollection[22] = Color.FromRgb(247, 121, 243);
            random.Shuffle(darkColorCollection);
        }

        public static double randomGeneratorTendencyRate = 0.3;
        public static int NextSmallerRandom(int min, int max)
        {
            if (min >=  max)
            {
                return min;
            }
            double randomGeneratorTendency = randomGeneratorTendencyRate * (max - min + 1);
            double below = Math.Pow(randomGeneratorTendency, 1.0 / 3);
            double top = Math.Pow((max - min + 1) - randomGeneratorTendency, 1.0 / 3);
            int result;
            do
            {
                result = (int)(Math.Floor(Math.Pow(random.NextDouble() * (top + below) - below, 3)) + randomGeneratorTendency + min);
            } while (result < min || result > max);
            return result;
            // int result = (int)Math.Pow(random.Next((int)Math.Pow(min, 3), (int)Math.Pow(max + 1, 3)), 1.0/3);
            // return min + (max - result);
        }

/*        public static Color NextColor(int minY = 0, int maxY = 255)
        {
            int iSeed = 10;
            Random ro = new Random(iSeed);
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            int Y;
            int R; 
            int B; 
            int G;
            do
            {
                R = ran.Next(255);
                G = ran.Next(255);
                B = ran.Next(255);
                B = (R + G > 400) ? R + G - 400 : B;//0 : 380 - R - G;
                B = (B > 255) ? 255 : B;

                Y = (int)(0.299 * R + 0.587 * G + 0.114 * B);
            } while (!(minY < Y && Y < maxY));

            return Color.FromRgb((byte)R, (byte)G, (byte)B);
        }*/

        public static Color NextDarkColor()
        {
            if (darkColorCollectionCur <  darkColorCollection.Length)
            {
                return darkColorCollection[darkColorCollectionCur++];
            } 
            else
            {
                darkColorCollectionCur = 0;
                return darkColorCollection[darkColorCollectionCur++];
            }
        }

    }
}
