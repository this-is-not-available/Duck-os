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

        public static void drawTriangle(ScreenPoint a, ScreenPoint b, ScreenPoint c, UV u, UV v, UV w, Canvas canvas, Bitmap image)
        {
            int minX = (int)Math.Max(Math.Min(Math.Min(a.x, b.x), c.x), 0);
            int minY = (int)Math.Max(Math.Min(Math.Min(a.y, b.y), c.y), 0);
            int maxX = (int)Math.Min(Math.Max(Math.Max(a.x, b.x), c.x), (int)Global.width - 1);
            int maxY = (int)Math.Min(Math.Max(Math.Max(a.y, b.y), c.y), (int)Global.height - 1);

            float ABC = edgeFunction(a, b, c);
            if (ABC < 0)
            {
                // Swap vertices to ensure the triangle is oriented correctly
                /*ScreenPoint temp = a;
                a = b;
                b = temp;
                ABC = -ABC;*/

                return;
            }

            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    ScreenPoint p = new ScreenPoint(x, y);
                    float ABP = edgeFunction(a, b, p);
                    float BCP = edgeFunction(b, c, p);
                    float CAP = edgeFunction(c, a, p);

                    if (ABP >= 0 && BCP >= 0 && CAP >= 0)
                    {
                        float weightA = BCP / ABC;
                        float weightB = CAP / ABC;
                        float weightC = ABP / ABC;

                        float depth = weightA * a.depth + weightB * b.depth + weightC * c.depth;
                        
                        if (depth < Global.depthBuffer[Global.width / 2 * x + y])
                        {
                            Global.depthBuffer[Global.width / 2 * x + y] = depth;
                            
                            (float, float) uv = ConvertBarycentricToUV(u, v, w, weightA, weightB, weightC);
                            //int cr = (byte)(uv.Item1 * 255);
                            //int cg = (byte)(uv.Item2 * 255);
                            //int cb = 0;
                            //Color pixelColor = Color.FromArgb(cr, cg, cb);

                            Color pixelColor = image.GetPixel((int)(uv.Item1 * image.Width), (int)((1-uv.Item2) * image.Height));
                        
                            canvas.DrawPoint(pixelColor, x, y);
                        }
                    }
                }
            }
        }
    }
}