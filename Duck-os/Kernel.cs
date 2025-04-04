using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using Sys = Cosmos.System;
using HAL = Cosmos.HAL;
using Renderer;
using Cosmos.Core_Plugs;
using System.Numerics;
using System.IO;
using IL2CPU.API.Attribs;
using System.ComponentModel;
using Cosmos.HAL;

namespace Duck_os
{
    public class Kernel : Sys.Kernel
    {
        private Canvas canvas;
        private readonly Font font = PCScreenFont.Default;
        public static Kernel Instance = new Kernel();

        int test = 200;
        public int counter = 0;

        public static uint width  = 1920;//800;
        public static uint height = 1080;//600;

        static Vertex vert1 = new Vertex(-3, -3, -3);
        static Vertex vert2 = new Vertex( 3, -3, -3);
        static Vertex vert3 = new Vertex(-3, -3,  3);
        static Vertex vert4 = new Vertex( 3, -3,  3);
        static Vertex vert5 = new Vertex( 0,  3,  0);

        static Edge edge1 = new Edge(1, 2);
        static Edge edge2 = new Edge(1, 3);
        static Edge edge3 = new Edge(1, 5);
        static Edge edge4 = new Edge(2, 4);
        static Edge edge5 = new Edge(2, 5);
        static Edge edge6 = new Edge(3, 4);
        static Edge edge7 = new Edge(3, 5);
        static Edge edge8 = new Edge(4, 5);

        static Face face1 = new Face(1, 5, 2);
        static Face face2 = new Face(1, 3, 5);
        static Face face3 = new Face(5, 3, 4);
        static Face face4 = new Face(2, 5, 4);
        static Face face5 = new Face(3, 1, 4);
        static Face face6 = new Face(1, 2, 4);

        static Vector3 position = new Vector3(0, 0, 5);
        static float scale = 1f;

        int deltaT = 0;
        int frames = 0;
        int fps = 0;
        float dt = 0;

        Mesh mesh = /*new Mesh(); */new Mesh(new List<Vertex> {vert1, vert2, vert3, vert4, vert5}, new List<Edge> {edge1, edge2, edge3, edge4, edge5, edge6, edge7, edge8}, new List<Face> {face1, face2, face3, face4, face5, face6}, position);

        [ManifestResourceStream(ResourceName = "Duck-os.data.duck.obj")]
        public static byte[] file;
        string fileContent = System.Text.Encoding.UTF8.GetString(file);

        protected override void BeforeRun()
        {
            Console.Clear();
            Console.WriteLine("Welcome to DuckOS!");
            HAL.Global.PIT.Wait(2000); // 2s

            try
            {
                mesh = ObjParser.ObjParser.Parse(fileContent);
                mesh.position = position;
                mesh.scale = scale;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Loaded model with " + mesh.edges.Count.ToString() + " edges");

            HAL.Global.PIT.Wait(2000); // 2s

            Console.WriteLine("Switching to drawing the mesh :)");

            HAL.Global.PIT.Wait(2000); // 2s
            canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(width, height, ColorDepth.ColorDepth32)); //800, 600
            canvas.Clear(Color.Red);

            canvas.Display();
        }

        protected override void Run()
        {
            if (deltaT != RTC.Second)
            {
                fps = frames;
                frames = 0;
                deltaT = RTC.Second;
            }
            dt = 1f / (float)fps * 100;

            try
            {
                canvas.Clear(Color.Blue);
                Instance.counter = 0;

                mesh.yRotation += 0.05;
                mesh.RenderMesh(canvas);

                canvas.DrawString("Omg guys look a spinning duck!", font, Color.Yellow, test, 100);
                canvas.DrawString(Instance.counter.ToString(), font, Color.Yellow, 0, 0);
                canvas.DrawString(fps.ToString(), font, Color.Yellow, 0, 50);
                canvas.DrawString(dt.ToString(), font, Color.Yellow, 0, 75);

                canvas.Display();
                test++;

            }
            catch (Exception ex)
            {
                canvas.DrawString(ex.Message, font, Color.Yellow, (int) width/2, (int) height/2);
            }

            frames++;
            HAL.Global.PIT.Wait((uint)(0));
        }
    }
}
