using System;
using System.Collections.Generic;
using Cosmos.System.Graphics;

namespace Duck_os
{
    public static class ObjParser
    {
        private static int ParseIntSafe(string str)
        {
            int result;
            if (int.TryParse(str, out result))
            {
                return result;
            }
            return 1;
        }

        private static float ParseFloatSafe(string str)
        {
            float result;
            if (float.TryParse(str, out result))
            {
                return result;
            }
            return 1f;
        }

        public static Mesh GetDefaultMesh()
        {
            Vertex vert1 = new Vertex(-3, -3, -3);
            Vertex vert2 = new Vertex(3, -3, -3);
            Vertex vert3 = new Vertex(-3, -3, 3);
            Vertex vert4 = new Vertex(3, -3, 3);
            Vertex vert5 = new Vertex(0, 3, 0);

            Edge edge1 = new Edge(1, 2);
            Edge edge2 = new Edge(1, 3);
            Edge edge3 = new Edge(1, 5);
            Edge edge4 = new Edge(2, 4);
            Edge edge5 = new Edge(2, 5);
            Edge edge6 = new Edge(3, 4);
            Edge edge7 = new Edge(3, 5);
            Edge edge8 = new Edge(4, 5);

            Face face1 = new Face(1, 5, 2);
            Face face2 = new Face(1, 3, 5);
            Face face3 = new Face(5, 3, 4);
            Face face4 = new Face(2, 5, 4);
            Face face5 = new Face(3, 1, 4);
            Face face6 = new Face(1, 2, 4);

            return new Mesh(new List<Vertex> { vert1, vert2, vert3, vert4, vert5 }, new List<Edge> { edge1, edge2, edge3, edge4, edge5, edge6, edge7, edge8 }, new List<Face> { face1, face2, face3, face4, face5, face6 });
        }

        private static Face ParseTriangleFace(string[] parts)
        {
            Face face = new Face();
            for (int i = 1; i < parts.Length; i++)
            {
                var subParts = parts[i].Split('/');
                face.VertexIndices.Add(ParseIntSafe(subParts[0]) - 1);

                if (subParts.Length > 1 && !string.IsNullOrEmpty(subParts[1]))
                {
                    face.TextureIndices.Add(ParseIntSafe(subParts[1]) - 1);
                }

                if (subParts.Length > 2)
                {
                    face.NormalIndices.Add(ParseIntSafe(subParts[2]) - 1);
                }
            }

            return face;
        }

        private static List<Face> ParseQuadFace(string[] parts)
        {
            List<Face> faces = new List<Face>();

            // Split the quad into two triangles
            string[] triangle1 = { parts[0], parts[1], parts[2], parts[3] };
            string[] triangle2 = { parts[0], parts[3], parts[4], parts[1] };

            // Parse the triangles
            faces.Add(ParseTriangleFace(triangle1));
            faces.Add(ParseTriangleFace(triangle2));

            return faces;
        }

        private static List<Face> ParseFace(string[] parts)
        {
            List<Face> faces = new List<Face>();

            if (parts.Length - 1 == 3)
            {
                faces.Add(ParseTriangleFace(parts));
            }
            else if (parts.Length - 1 == 4)
            {
                foreach (Face face in ParseQuadFace(parts))
                {
                    faces.Add(face);
                }
            }

            return faces;
        }
        public static Mesh Parse(string fileContent, Bitmap texture)
        {
            Mesh model = Parse(fileContent);
            model.texture = texture;
            return model;
        }

        public static Mesh Parse(string fileContent)
        {
            Mesh model = new Mesh();
            List<Vertex> vertices = new List<Vertex>();
            List<Edge> edges = new List<Edge>();
            List<Face> faces = new List<Face>();
            List<Normal> normals = new List<Normal>();
            List<UV> UVs = new List<UV>();

            string[] lines = fileContent.Split('\n');

            //try
            //{
                foreach (string line1 in lines)
                {
                    string line = line1.TrimEnd(new char[] { '\r', '\n' });
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;

                    switch (parts[0])
                    {
                        case "v":
                            // Vertex

                            Vertex vertex = new Vertex(ParseFloatSafe(parts[1]), ParseFloatSafe(parts[2]), ParseFloatSafe(parts[3]));
                            vertices.Add(vertex);
                            break;

                        case "vt":
                            // Texture Coordinate
                            UV texture = new UV
                            (
                                float.Parse(parts[1]),
                                float.Parse(parts[2])
                            );
                            UVs.Add(texture);
                            break;

                        case "vn":
                            // Normal
                            Normal normal = new Normal
                            {
                                X = float.Parse(parts[1]),
                                Y = float.Parse(parts[2]),
                                Z = float.Parse(parts[3])
                            };
                            normals.Add(normal);
                            break;

                        case "f":
                            // Face
                            List<Face> parsedFaces = ParseFace(parts);
                            foreach (Face face in parsedFaces)
                            {
                                faces.Add(face);
                            }
                            break;

                        default:
                            // Ignore unexpected lines
                            break;
                    }
                }
            /*}
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return GetDefaultMesh();
            }*/

            model.vertices = vertices;
            model.faces = faces;
            model.edges = edges;
            model.UVs = UVs;
            model.normals = normals;

            return model;
        }
    }
}