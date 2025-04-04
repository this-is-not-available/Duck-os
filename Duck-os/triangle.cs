using System;
using System.Drawing;
using System.Numerics;
using Cosmos.System.Graphics;
using Renderer;
using Kernel = Duck_os.Kernel;

namespace Triangle
{
    public static class Triangle
    {
        private static float edgeFunction(ScreenPoint a, ScreenPoint b, ScreenPoint c)
        {
            return (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        }

        public static UV ConvertBarycentricToUV(ScreenVertex uv1, ScreenVertex uv2, ScreenVertex uv3, float a, float b, float c)
        {
            float u = a * uv1.u + b * uv2.u + c * uv3.u;
            float v = a * uv1.v + b * uv2.v + c * uv3.v;

            return new UV(u, v);
        }

        public static void drawTriangle(ScreenPoint a, ScreenPoint b, ScreenPoint c, Canvas canvas, Color color)
        {
            int minX = (int)Math.Max(Math.Min(Math.Min(a.x, b.x), c.x), 0);
            int minY = (int)Math.Max(Math.Min(Math.Min(a.y, b.y), c.y), 0);
            int maxX = (int)Math.Min(Math.Max(Math.Max(a.x, b.x), c.x), (int)Kernel.width);
            int maxY = (int)Math.Min(Math.Max(Math.Max(a.y, b.y), c.y), (int)Kernel.height);

            /*ScreenPoint ap = new ScreenPoint((int)a.x, (int)a.y);
            ScreenPoint bp = new ScreenPoint((int)b.x, (int)b.y);
            ScreenPoint cp = new ScreenPoint((int)c.x, (int)c.y);*/

            float ABC = edgeFunction(a, b, c);
            if (ABC == 0)
            {
                //return;
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
                        if (color != Color.Empty)
                        {
                            canvas.DrawPoint(color, x, y);
                        }
                        else
                        {
                            float weightA = BCP / ABC;
                            float weightB = CAP / ABC;
                            float weightC = ABP / ABC;

                            int cr = (byte)(255 * weightA + 0 * weightB + 0 * weightC);
                            int cg = (byte)(0 * weightA + 255 * weightB + 0 * weightC);
                            int cb = (byte)(0 * weightA + 0 * weightB + 255 * weightC);

                            //UV uv = ConvertBarycentricToUV(a, b, c, weightA, weightB, weightC);
                            //int cr = (byte)(uv.U * 255);
                            //int cg = (byte)(uv.V * 255);
                            //int cb = 0;

                            canvas.DrawPoint(Color.FromArgb(cr, cg, cb), x, y);

                            Kernel.Instance.counter++;
                        }
                    }
                }
            }
        }

    }
}