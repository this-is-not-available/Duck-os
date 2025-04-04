using System;
using System.Collections.Generic;
using System.Numerics;
using HAL = Cosmos.HAL;
using IL2CPU.API;
using Renderer;
using Cosmos.Debug;
using System.Diagnostics;

namespace ObjParser
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

        /*public static Mesh ParseObj(string path)
		{
			string[] lines = File.ReadAllText(path).Split('\n');

            List<Vertex> vertices = new List<Vertex>();
			List<Edge> edges = new List<Edge>();
			List<Face> faces = new List<Face>();
			Vector3 position = new Vector3(0, 0, 8);

			foreach (string line in lines)
			{
				string[] parts = line.Split(' ');
				if (line.StartsWith("f "))
				{
					if (parts.Length == 5)
					{
						Console.WriteLine("Quad face detected, converting to triangles");
						Console.WriteLine(ParseIntSafe(parts[1]).ToString() + ParseIntSafe(parts[2]).ToString() + ParseIntSafe(parts[3]).ToString());

                        faces.Add(new Face(ParseIntSafe(parts[1]), ParseIntSafe(parts[2]), ParseIntSafe(parts[3])));
						faces.Add(new Face(ParseIntSafe(parts[1]), ParseIntSafe(parts[3]), ParseIntSafe(parts[4])));
					}
					else if (parts.Length == 4)
					{
						faces.Add(new Face(ParseIntSafe(parts[1]), ParseIntSafe(parts[2]), ParseIntSafe(parts[3])));
					}
                }
				else if (line.StartsWith("v "))
                {
                    Console.WriteLine(ParseFloatSafe(parts[1]).ToString() + ParseFloatSafe(parts[2]).ToString() + ParseFloatSafe(parts[3]).ToString());
                    vertices.Add(new Vertex(ParseFloatSafe(parts[1]), ParseFloatSafe(parts[2]), ParseFloatSafe(parts[3])));
                }
            }

			return new Mesh(vertices, edges, faces, position);
		}*/

        public static Mesh Parse(string fileContent)
        {
            var model = new Mesh();
            var vertices = new List<Vertex>();
            var faces = new List<Face>();
            var normals = new List<Normal>();
            var UVs = new List<UV>();

            string[] lines = fileContent.Split('\n');

            try
            {
                foreach (string line1 in lines)
                {
                    string line = line1.TrimEnd(new char[] { '\r', '\n' });
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0) continue;

                    switch (parts[0])
                    {
                        case "v":
                            // Vertex

                            float x = ParseFloatSafe(parts[1]);
                            float y = ParseFloatSafe(parts[2]);
                            float z = ParseFloatSafe(parts[3]);
                            //Console.WriteLine(x.ToString() + " " + y.ToString() + " " + z.ToString());

                            var vertex = new Vertex(x, y, z);
                            vertices.Add(vertex);
                            break;

                        case "vt":
                            // Texture Coordinate
                            var texture = new UV
                            (
                                float.Parse(parts[1]),
                                float.Parse(parts[2])
                            );
                            UVs.Add(texture);
                            break;

                        case "vn":
                            // Normal
                            var normal = new Normal
                            {
                                X = float.Parse(parts[1]),
                                Y = float.Parse(parts[2]),
                                Z = float.Parse(parts[3])
                            };
                            normals.Add(normal);
                            break;

                        case "f":
                            // Face
                            var face = new Face();
                            for (int i = 1; i < parts.Length; i++)
                            {
                                var subParts = parts[i].Split('/');
                                face.VertexIndices.Add(ParseIntSafe(subParts[0])); // OBJ indices are 1-based
                                if (subParts.Length > 1 && !string.IsNullOrEmpty(subParts[1]))
                                {
                                    face.TextureIndices.Add(ParseIntSafe(subParts[1]) - 1);
                                }
                                if (subParts.Length > 2)
                                {
                                    face.NormalIndices.Add(ParseIntSafe(subParts[2]) - 1);
                                }
                            }
                            faces.Add(face);
                            break;

                        default:
                            // Ignore unexpected lines
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            model.vertices = vertices;
            model.faces = faces;
            model.UVs = UVs;
            model.normals = normals;

            for (int i = 0; i < vertices.Count; i++)
            {
                Console.WriteLine(vertices[i].x.ToString() + " " + vertices[i].y.ToString() + " " + vertices[i].z.ToString());
            }

            foreach (var face in faces)
            {
                int vertexCount = face.VertexIndices.Count;
                for (int i = 0; i < vertexCount; i++)
                {
                    // Each edge is created from two vertices
                    int v1 = face.VertexIndices[i];
                    int v2 = face.VertexIndices[(i + 1) % vertexCount]; // Wrap around to create a closed loop

                    model.edges.Add(new Edge(v1, v2));
                }
            }

            return model;
        }
    }
}