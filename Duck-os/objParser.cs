﻿using System;
using System.Collections.Generic;
using System.IO;
using Cosmos.Core;
using Cosmos.Core.Memory;
using Cosmos.System.Graphics;
using CosmosPNG.PNGLib.Decoders.PNG;
using IL2CPU.API.Attribs;

namespace Duck_os
{
    public static class ObjParser
    {
        [ManifestResourceStream(ResourceName = "Duck-os.data.uvs.png")]
        private static byte[] uvFile;

        private static int ParseIntSafe(string str)
        {
            return int.TryParse(str, out int result) ? result : 1;
        }

        private static float ParseFloatSafe(string str)
        {
            return float.TryParse(str, out float result) ? result : 1f;
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private static string Function(char character, int i, int lenght)
        {
            return new string(character, i) + new string(' ', lenght - i);
        }

        public static Mesh GetDefaultMesh()
        {
            Vertex vert1 = new Vertex(-3, -3, -3);
            Vertex vert2 = new Vertex(3, -3, -3);
            Vertex vert3 = new Vertex(-3, -3, 3);
            Vertex vert4 = new Vertex(3, -3, 3);
            Vertex vert5 = new Vertex(0, 3, 0);

            Face face1 = new Face(1, 5, 2);
            Face face2 = new Face(1, 3, 5);
            Face face3 = new Face(5, 3, 4);
            Face face4 = new Face(2, 5, 4);
            Face face5 = new Face(3, 1, 4);
            Face face6 = new Face(1, 2, 4);

            Mesh mesh = new Mesh(new List<Vertex> { vert1, vert2, vert3, vert4, vert5 }, new List<Face> { face1, face2, face3, face4, face5, face6 });
            mesh.texture = new PNGDecoder().GetBitmap(uvFile);
            /*mesh.UVs.Add(new UV(0f, 0));
            mesh.UVs.Add(new UV(0.2f, 0));
            mesh.UVs.Add(new UV(0.4f, 0));
            mesh.UVs.Add(new UV(0.6f, 0));
            mesh.UVs.Add(new UV(0.8f, 0));
            mesh.UVs.Add(new UV(1f, 0));*/

            mesh.normals.Add(new Normal(1f, 0, 0));
            return mesh;
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
            string[] lines = fileContent.Split('\n');
            int i = 0;
            Console.WriteLine("");

            while (i < lines.Length)
            {
                string line = lines[i].TrimEnd(new char[] { '\r', '\n' });
                if (string.IsNullOrWhiteSpace(line))
                {
                    i++;
                    continue;
                }

                string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) {
                    i++;
                    continue;
                }

                switch (parts[0])
                {
                    case "v":
                        // Vertex
                        model.vertices.Add(new Vertex(ParseFloatSafe(parts[1]), ParseFloatSafe(parts[2]), ParseFloatSafe(parts[3])));
                        break;

                    case "vt":
                        // Texture Coordinate
                        model.UVs.Add(new UV(float.Parse(parts[1]), 1 - float.Parse(parts[2])));
                        break;

                    case "vn":
                        // Normal
                        model.normals.Add(new Normal
                        (
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3])
                        ));
                        break;

                    case "f":
                        // Face
                        model.faces.AddRange(ParseFace(parts));
                        break;

                    default:
                        // Ignore unexpected lines
                        break;
                }

                if (i % 25 == 0)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    ClearCurrentConsoleLine();
                    Console.WriteLine("Loading" + Function('.', (i / 500) % 4, 3) + " [" + Function('#', (int)(25f * i / lines.Length), 25) + "] " + (i * 100 / lines.Length).ToString() + "%");
                    Heap.Collect();
                }

                i++;
            }

            lines = null;
            return model;
        }
    }
}