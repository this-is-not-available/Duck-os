using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Cosmos.System.Graphics;
using Duck_os;
using Vector3 = Duck_os.Vector3;

namespace Renderer
{
    public struct ScreenPoint
    {
        public ScreenPoint(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public int x { get; set; } = 0;
        public int y { get; set; } = 0;
    }

    public struct ScreenVertex
    {
        public ScreenVertex(float X, float Y, float U, float V)
        {
            x = X;
            y = Y;
            u = U;
            v = V;
        }

        public ScreenVertex(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public float x { get; set; } = 0;
        public float y { get; set; } = 0;
        public float u { get; set; } = 0;
        public float v { get; set; } = 0;
    }

    public struct Vertex
    {
        public Vertex(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public float x { get; set; } = 0f;
        public float y { get; set; } = 0f;
        public float z { get; set; } = 0f;

        public static Vertex RotateX(Vertex point, double angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            return new Vertex
            {
                x = point.x,
                y = cos * point.y - sin * point.z,
                z = sin * point.y + cos * point.z
            };
        }

        public static Vertex RotateY(Vertex point, double angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            return new Vertex
            {
                x = cos * point.x + sin * point.z,
                y = point.y,
                z = -sin * point.x + cos * point.z
            };
        }

        public static Vertex RotateZ(Vertex point, double angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            return new Vertex
            {
                x = cos * point.x - sin * point.y,
                y = sin * point.x + cos * point.y,
                z = point.z
            };
        }
    }


    public struct Edge
    {
        public Edge(int vert1, int vert2)
        {
            vertex1index = vert1;
            vertex2index = vert2;
        }
        public int vertex1index { get; set; }
        public int vertex2index { get; set; }
    }

    public class Face
    {
        public Face()
        {
        }
        public Face(int vert1, int vert2, int vert3)
        {
            VertexIndices.Add(vert1);
            VertexIndices.Add(vert2);
            VertexIndices.Add(vert3);
        }
        public List<int> VertexIndices = new List<int>();
        public List<int> TextureIndices = new List<int>();
        public List<int> NormalIndices = new List<int>();
    }

    public class UV
    {
        public float U, V;

        public UV(float u, float v)
        {
            U = u;
            V = v;
        }
    }

    public class Normal
    {
        public float X, Y, Z;
    }

    public struct Mesh
    {
        public Mesh()
        {
        }
        public Mesh(List<Vertex> verts, List<Edge> Edges, List<Face> Faces, Vector3 Position, List<UV> uvs, List<Normal> Normals)
        {
            vertices = verts;
            edges = Edges;
            position = Position;
            faces = Faces;
            UVs = uvs;
            normals = Normals;
        }

        public Mesh(List<Vertex> verts, List<Edge> Edges, List<Face> Faces, Vector3 Position)
        {
            vertices = verts;
            edges = Edges;
            position = Position;
            faces = Faces;
        }

        public List<Vertex> vertices = new List<Vertex>();
        public List<Edge> edges = new List<Edge>();
        public List<Face> faces = new List<Face>();
        public List<UV> UVs = new List<UV>();
        public List<Normal> normals = new List<Normal>();

        public Vector3 position = new Vector3(0, 0, 0);
        public float scale = 1;

        public double xRotation = 0;
        public double yRotation = 0;
        public double zRotation = 0;

        public Vertex transformVertex(Vertex vertex)
        {
            Vertex rotatedVertex = Vertex.RotateX(vertex, xRotation);
            rotatedVertex = Vertex.RotateY(rotatedVertex, yRotation);
            rotatedVertex = Vertex.RotateZ(rotatedVertex, zRotation);

            rotatedVertex.x *= scale;
            rotatedVertex.y *= scale;
            rotatedVertex.z *= scale;

            rotatedVertex.x += position.X;
            rotatedVertex.y += position.Y;
            rotatedVertex.z += position.Z;
            return rotatedVertex;
        }

        public void RenderVertices(Canvas canvas)
        {
            foreach (Vertex vertex in vertices)
            {
                Vertex Vertex = transformVertex(vertex);

                ScreenPoint rendered = Renderer.ProjectVertex2(Vertex);
                canvas.DrawPoint(Color.Black, (int)rendered.x, (int)rendered.y);
            }
        }

        public void RenderWireframeMesh(Canvas canvas)
        {
            foreach (Edge edge in edges)
            {
                Vertex vertex1 = vertices[edge.vertex1index - 1];
                Vertex vertex2 = vertices[edge.vertex2index - 1];

                vertex1 = transformVertex(vertex1);
                vertex2 = transformVertex(vertex2);

                ScreenPoint rendered1 = Renderer.ProjectVertex2(vertex1);
                ScreenPoint rendered2 = Renderer.ProjectVertex2(vertex2);

                int X1 = (int)rendered1.x;
                int Y1 = (int)rendered1.y;
                int X2 = (int)rendered2.x;
                int Y2 = (int)rendered2.y;

                canvas.DrawLine(Color.Black, X1, Y1, X2, Y2);
            }
        }

        public void RenderMesh(Canvas canvas)
        {
            foreach (Face face in faces)
            {
                Vertex vertex1 = vertices[face.VertexIndices[0] - 1];
                Vertex vertex2 = vertices[face.VertexIndices[1] - 1];
                Vertex vertex3 = vertices[face.VertexIndices[2] - 1];

                vertex1 = transformVertex(vertex1);
                vertex2 = transformVertex(vertex2);
                vertex3 = transformVertex(vertex3);

                ScreenPoint rendered1 = Renderer.ProjectVertex2(vertex1);
                ScreenPoint rendered2 = Renderer.ProjectVertex2(vertex2);
                ScreenPoint rendered3 = Renderer.ProjectVertex2(vertex3);

               /* rendered1.u = UVs[face.VertexIndices[0] - 1].U;
                rendered1.v = UVs[face.VertexIndices[0] - 1].V;
                rendered2.u = UVs[face.VertexIndices[1] - 1].U;
                rendered2.v = UVs[face.VertexIndices[1] - 1].V;
                rendered3.u = UVs[face.VertexIndices[2] - 1].U;
                rendered3.v = UVs[face.VertexIndices[2] - 1].V;*/

                Triangle.Triangle.drawTriangle(rendered1, rendered2, rendered3, canvas, Color.Empty);
            }
        }
    }

    public class Renderer
    {
        public static ScreenPoint ProjectVertex(Vertex vertex)
        {
            const int FocalLength = 90;
            float xProjected = 0;
            float yProjected = 0;
            float scaleFactor = Kernel.height / 3;

            xProjected = (FocalLength * vertex.x ) / (FocalLength + vertex.z);
            yProjected = (FocalLength * -vertex.y) / (FocalLength + vertex.z);

            return new ScreenPoint((int)(xProjected * scaleFactor + Kernel.width / 2), (int)(yProjected * scaleFactor + Kernel.height / 2)); // 400 = width / 2    300 = height / 2
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
            float xNDC = (vertex.x * fovAdjustment * (Kernel.height / 500)) / z;
            float yNDC = (-vertex.y * fovAdjustment * (Kernel.height / 500)) / z;

            // Convert NDC to screen coordinates
            float xScreen = (xNDC) * 0.5f * Kernel.height; // Map from [-1, 1] to [-0.5 * screenWidth,  0.5 * screenWidth]
            float yScreen = (yNDC) * 0.5f * Kernel.height; // Map from [-1, 1] to [-0.5 * screenHeight, 0.5 * screenHeight]

            return new ScreenPoint((int)(xScreen + Kernel.width / 2), (int)(yScreen + Kernel.height / 2));
        }
    }
}
