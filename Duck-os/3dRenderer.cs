namespace Duck_os
{
    public class Renderer
    {
        private static float fovAdjustment = 1.0f / 1.61977519054f;//MathF.Tan(90 / 2);
        private static readonly float ScaleFactor = Global.height / 500;

        public static ScreenPoint ProjectVertex(Vertex vertex)
        {
            // Cache results
            if (vertex.cachedFrame == Kernel.global.frame)
            {
                return (ScreenPoint)vertex.cachedScreenPoint;
            }

            // Perform projection
            ScreenPoint screenPoint = _ProjectVertex(vertex);
            vertex.cachedScreenPoint = screenPoint;
            vertex.cachedFrame = Kernel.global.frame;

            return screenPoint;
        }

        private static ScreenPoint _ProjectVertex(Vertex vertex)
        {
            if (vertex.z < 0.1f)
            {
                return new ScreenPoint(-1, -1);
            }

            float xNDC = (vertex.x * fovAdjustment * ScaleFactor) / vertex.z;
            float yNDC = (-vertex.y * fovAdjustment * ScaleFactor) / vertex.z;

            float xScreen = (xNDC) * 0.5f * Global.height;
            float yScreen = (yNDC) * 0.5f * Global.height;

            return new ScreenPoint((int)(xScreen + Global.width * 0.5), (int)(yScreen + Global.height * 0.5), vertex.z);
        }
    }
}
