using System;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Sys = Cosmos.System;
using HAL = Cosmos.HAL;
using IL2CPU.API.Attribs;
using CosmosPNG.PNGLib.Decoders.PNG;
using Cosmos.Core;
using Cosmos.Core.Memory;

namespace Duck_os
{
    public class Kernel : Sys.Kernel
    {
        private Canvas canvas;
        private readonly Font font = PCScreenFont.Default;
        public static Global global = new Global();

        ulong secondCycles;
        int GCCount = 0;

        Mesh mesh;

        [ManifestResourceStream(ResourceName = "Duck-os.data.duck.png")]
        public static byte[] pngFile;

        [ManifestResourceStream(ResourceName = "Duck-os.data.duck_full.obj")]
        public static byte[] objFile;
        string objFileContent = System.Text.Encoding.UTF8.GetString(objFile);

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("Welcome to " + Global.OSName + "!");
            Console.WriteLine("Version " + Global.OSVersion);

            Bitmap Image = new PNGDecoder().GetBitmap(pngFile);

            mesh = ObjParser.Parse(objFileContent, Image);
            mesh.position = new Vector3(0, 0, 5);
            mesh.scale = 1f;

            ulong startCycle = CPU.GetCPUUptime();
            HAL.Global.PIT.Wait(100);
            secondCycles = (CPU.GetCPUUptime() - startCycle) * 10;
            Heap.Collect();

            // Initialize the canvas
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(Global.width, Global.height, ColorDepth.ColorDepth32)); //800, 600
            canvas.Clear(Color.Yellow);
            canvas.Display();
        }

        protected override void Run()
        {
            float startTime = CPU.GetCPUUptime();
            canvas.Clear(Color.Blue);
            global.ClearDepthBuffer();

            mesh.yRotation -= 2 * global.dt;
            mesh.RenderMesh(canvas);
            canvas.DrawString("FPS " + global.fps.ToString(), font, Color.Yellow, 0, 0);
            canvas.Display();

            Sys.KeyEvent keyEvent;
            Sys.KeyboardManager.TryReadKey(out keyEvent);

            // Only works on PS/2 keyboards
            if (keyEvent.Key == Sys.ConsoleKeyEx.Escape)
            {
                Sys.Power.Shutdown();
            }

            // Garbage collect every 4 frames
            if (GCCount < 5) { GCCount++; }
            else
            {
                Heap.Collect();
                GCCount = 0;
            }

            // Limit the framerate to 30 FPS
            global.dt = (CPU.GetCPUUptime() - startTime) / secondCycles;
            global.fps = 1f / global.dt;
            if (Global.LimitFPS)
            {
                ulong endCycle = (ulong)(CPU.GetCPUUptime() + ((0.03333333333 - global.dt) * secondCycles));
                while (true)
                {
                    if (CPU.GetCPUUptime() >= endCycle || CPU.GetCPUUptime() <= startTime)
                    {
                        break;
                    }
                }
            }

            global.frame++;
        }
    }
}
