using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System.Graphics;

namespace Duck_os
{
    public struct ScreenPoint
    {
        public ScreenPoint(int X, int Y, float Depth = 0)
        {
            x = X;
            y = Y;
            depth = Depth;
        }

        public int x { get; set; }
        public int y { get; set; }
        public float depth { get; set; }
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

        public ScreenPoint? cachedScreenPoint = null;
        public int cachedFrame = 0;

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

        public Mesh(List<Vertex> verts, List<Face> Faces, Vector3 Position, List<UV> uvs, List<Normal> Normals)
        {
            vertices = verts;
            position = Position;
            faces = Faces;
            UVs = uvs;
            normals = Normals;
        }

        public Mesh(List<Vertex> verts, List<Face> Faces, Vector3 Position)
        {
            vertices = verts;
            position = Position;
            faces = Faces;
        }

        public Mesh(List<Vertex> verts, List<Face> Faces)
        {
            vertices = verts;
            faces = Faces;
        }

        public List<Vertex> vertices = new List<Vertex>();
        public List<Face> faces = new List<Face>();
        public List<UV> UVs = new List<UV>();
        public List<Normal> normals = new List<Normal>();

        public Bitmap texture = null;

        public Vector3 position = new Vector3(0, 0, 0);
        public double xRotation = 0;
        public double yRotation = 0;
        public double zRotation = 0;
        public float scale = 1;

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

                ScreenPoint rendered = Renderer.ProjectVertex(Vertex);
                canvas.DrawPoint(Color.Black, (int)rendered.x, (int)rendered.y);
            }
        }

        public void RenderMesh(Canvas canvas)
        {
            foreach (Face face in faces)
            {
                Vertex vertex1 = vertices[face.VertexIndices[0]];
                Vertex vertex2 = vertices[face.VertexIndices[1]];
                Vertex vertex3 = vertices[face.VertexIndices[2]];

                vertex1 = transformVertex(vertex1);
                vertex2 = transformVertex(vertex2);
                vertex3 = transformVertex(vertex3);

                ScreenPoint rendered1 = Renderer.ProjectVertex(vertex1);
                ScreenPoint rendered2 = Renderer.ProjectVertex(vertex2);
                ScreenPoint rendered3 = Renderer.ProjectVertex(vertex3);

                UV uv1 = UVs[face.TextureIndices[0]];
                UV uv2 = UVs[face.TextureIndices[1]];
                UV uv3 = UVs[face.TextureIndices[2]];

                float brightness = 1f;// Math.Min(Math.Max(Dot(normals[face.NormalIndices[0]], new Vector3(0, 0, 1)), 1), 0);

                Triangle.drawTriangle(rendered1, rendered2, rendered3, uv1, uv2, uv3, canvas, texture, brightness);
            }
        }
    }
}
