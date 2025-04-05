namespace Duck_os
{
    public class Global
    {
        public static string OSName = "DuckOS";
        public static string OSVersion = "1.0.0";

        public static uint width = 1920;
        public static uint height = 1080;

        public float[] depthBuffer = new float[width * height];
    }
}
