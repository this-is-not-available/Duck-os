using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Cosmos.System.Graphics;
using Duck_os;
using Vector3 = Duck_os.Vector3;

namespace Duck_os
{
    public class Renderer
    {
        public static ScreenPoint ProjectVertex(Vertex vertex)
        {
            const int FocalLength = 90;
            float xProjected = 0;
            float yProjected = 0;
            float scaleFactor = Global.height / 3;

            xProjected = (FocalLength * vertex.x ) / (FocalLength + vertex.z);
            yProjected = (FocalLength * -vertex.y) / (FocalLength + vertex.z);

            return new ScreenPoint((int)(xProjected * scaleFactor + Global.width / 2), (int)(yProjected * scaleFactor + Global.height / 2)); // 400 = width / 2    300 = height / 2
        }

        public static ScreenPoint ProjectVertex2(Vertex vertex)
        {
            // Calculate the perspective projection parameters
            float fovAdjustment = 1.0f / 1.61977519054f;//MathF.Tan(90 / 2);
            float z = vertex.z;

            // Ensure the point is within the near and far clipping planes
            if (z < 0.1f /*|| z > 100f*/) // Using a hardcoded far plane for simplicity
            {
                return new ScreenPoint(-1, -1); // You could return an indication of invisibility
            }

            // Manually calculate x and y in normalized device coordinates (NDC)
            float xNDC = (vertex.x * fovAdjustment * (Global.height / 500)) / z;
            float yNDC = (-vertex.y * fovAdjustment * (Global.height / 500)) / z;

            // Convert NDC to screen coordinates
            float xScreen = (xNDC) * 0.5f * Global.height; // Map from [-1, 1] to [-0.5 * screenWidth,  0.5 * screenWidth]
            float yScreen = (yNDC) * 0.5f * Global.height; // Map from [-1, 1] to [-0.5 * screenHeight, 0.5 * screenHeight]

            return new ScreenPoint((int)(xScreen + Global.width / 2), (int)(yScreen + Global.height / 2), z);
        }
    }
}
