using System;
using System.Drawing;
using Cosmos.System.Graphics;
using CosmosPNG.GraphicsKit;

namespace Duck_os
{
    public class Triangle
    {
        static Global Global = Kernel.global;
        private static float edgeFunction(ScreenPoint a, ScreenPoint b, ScreenPoint c)
        {
            return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        }

        public static (float, float) ConvertBarycentricToUV(UV uv1, UV uv2, UV uv3, float a, float b, float c)
        {
            float u = a * uv1.U + b * uv2.U + c * uv3.U;
            float v = a * uv1.V + b * uv2.V + c * uv3.V;

            return (u, v);
        }

        public static void drawTriangle(ScreenPoint a, ScreenPoint b, ScreenPoint c, UV u, UV v, UV w, Canvas canvas, Bitmap image, float brightness)
        {
            int minX = Math.Max(Math.Min(Math.Min(a.x, b.x), c.x), 0);
            int minY = Math.Max(Math.Min(Math.Min(a.y, b.y), c.y), 0);
            int maxX = Math.Min(Math.Max(Math.Max(a.x, b.x), c.x), (int)Global.width - 1);
            int maxY = Math.Min(Math.Max(Math.Max(a.y, b.y), c.y), (int)Global.height - 1);

            float ABC = edgeFunction(a, b, c);
            if (ABC < 0)
            {
                return;
            }

            float A01_Change = a.y - b.y, B01_Change = b.x - a.x;
            float A12_Change = b.y - c.y, B12_Change = c.x - b.x;
            float A20_Change = c.y - a.y, B20_Change = a.x - c.x;

            ScreenPoint p = new ScreenPoint(minX, minY);
            float ABP_Row = edgeFunction(a, b, p);
            float BCP_Row = edgeFunction(b, c, p);
            float CAP_Row = edgeFunction(c, a, p);

            // Calculate the index change for each row
            long indexChange = Global.width - (maxX - minX + 1) + Global.height;

            for (int y = minY; y <= maxY; y++)
            {
                long index = Global.height * y + minX;
                float ABP = ABP_Row;
                float BCP = BCP_Row;
                float CAP = CAP_Row;

                for (int x = minX; x <= maxX; x++)
                {
                    if (ABP >= 0 && BCP >= 0 && CAP >= 0)
                    {
                        float weightA = BCP / ABC;
                        float weightB = CAP / ABC;
                        float weightC = ABP / ABC;

                        float depth = weightA * a.depth + weightB * b.depth + weightC * c.depth;
                        
                        if (depth < Global.depthBuffer[index])
                        {
                            Global.depthBuffer[index] = depth;
                            
                            (float, float) uv = ConvertBarycentricToUV(u, v, w, weightA, weightB, weightC);
                            Color pixelColor = image.GetPixel((int)(uv.Item1 * image.Width), (int)(uv.Item2 * image.Height));
                            //pixelColor = Color.FromArgb((int)(pixelColor.R * brightness), (int)(pixelColor.G * brightness), (int)(pixelColor.B * brightness));

                            canvas.DrawPoint(pixelColor, x, y);
                        }
                    }

                    index++;
                    ABP += A01_Change;
                    BCP += A12_Change;
                    CAP += A20_Change;
                }

                ABP_Row += B01_Change;
                BCP_Row += B12_Change;
                CAP_Row += B20_Change;
            }
        }
    }
}