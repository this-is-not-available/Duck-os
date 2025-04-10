namespace Duck_os
{
    public class Global
    {
        public static string OSName = "DuckOS";
        public static string OSVersion = "1.0.0";

        public static bool LimitFPS = false;

        public static uint width = 1024;
        public static uint height = 768;

        public float[] depthBuffer = new float[width * height];

        public float fps = 0;
        public float dt = 0;
        public int frame = 1;

        public void ClearDepthBuffer()
        {
            for (int i = 0; i < depthBuffer.Length - 1; i++)
            {
                depthBuffer[i] = float.MaxValue;
            }
        }
    }
}
