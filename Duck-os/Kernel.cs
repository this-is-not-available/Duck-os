using System;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Sys = Cosmos.System;
using HAL = Cosmos.HAL;
using IL2CPU.API.Attribs;
using CosmosPNG.PNGLib.Decoders.PNG;
using System.IO;

namespace Duck_os
{
    public class Kernel : Sys.Kernel
    {
        private Canvas canvas;
        private readonly Font font = PCScreenFont.Default;
        public static Global global = new Global();

        int test = 200;
        public int counter = 0;

        static Vector3 position = new Vector3(0, 0, 5);
        static float scale = 1f;

        int deltaT = 0;
        int frames = 0;
        int fps = 0;
        float dt = 0;

        Mesh mesh;

        [ManifestResourceStream(ResourceName = "Duck-os.data.duck2.obj")]
        public static byte[] objFile;
        string objFileContent = System.Text.Encoding.UTF8.GetString(objFile);

        [ManifestResourceStream(ResourceName = "Duck-os.data.duck.png")]
        public static byte[] pngFile;

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("Welcome to DuckOS!");

            Bitmap Image = new PNGDecoder().GetBitmap(pngFile);

            mesh = ObjParser.Parse(objFileContent, Image);
            mesh.position = position;
            mesh.scale = scale;

            Console.WriteLine("Loaded model with " + mesh.vertices.Count.ToString() + " verts");
            Console.WriteLine("Switching to drawing the mesh :)");

            
            // Initialize the canvas
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(Global.width, Global.height, ColorDepth.ColorDepth32)); //800, 600
            canvas.Clear(Color.Yellow);
            canvas.Display();
        }

        private void ClearDepthBuffer()
        {
            for (int i = 0; i < global.depthBuffer.Length - 1; i++)
            {
                global.depthBuffer[i] = float.MaxValue;
            }
        }

        protected override void Run()
        {
            if (deltaT != HAL.RTC.Second)
            {
                fps = frames;
                frames = 0;
                deltaT = HAL.RTC.Second;
            }
            dt = 1f / (float)fps * 100;

            try
            {
                canvas.Clear(Color.Blue);
                ClearDepthBuffer();

                mesh.yRotation += 0.05;
                mesh.RenderMesh(canvas);

                canvas.DrawString("Omg guys look a spinning duck!", font, Color.Yellow, test, 100);
                canvas.DrawString("FPS " + fps.ToString(), font, Color.Yellow, 0, 0);
                canvas.DrawString("DT  " + dt. ToString(), font, Color.Yellow, 0, 16);

                canvas.Display();
                test++;

            }
            catch (Exception ex)
            {
                canvas.DrawString(ex.Message, font, Color.Yellow, (int) Global.width / 2, (int)Global.height / 2);
            }

            frames++;
        }
    }
}
