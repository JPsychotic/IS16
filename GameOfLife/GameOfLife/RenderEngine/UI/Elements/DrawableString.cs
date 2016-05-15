using System.Collections.Generic;
using System.Drawing;
using GameOfLife.Storage;
using SlimDX;

namespace GameOfLife.RenderEngine.UI.Elements
{
    class DrawableString
    {
        readonly int sizeX = Config.Width, sizeY = Config.Height;
        public static int FontHeight = 11; // font is 10px + 2px abstand
        public static int FontWidth = 6; // 6 breit + 2 abstand
        public static float TexWidth = 2048;
        public static float Scale = 2f;
        public Mesh mesh;

        public DrawableString(string s, Vector2 textPos, Color col)
        {
            Vector2 screenMiddle = new Vector2(sizeX / 2f, sizeY / 2f);
            Vector2 tmp = (textPos - screenMiddle);
            Vector2 textPosScreenSpace = new Vector2(tmp.X / screenMiddle.X, -tmp.Y / screenMiddle.Y);
            Vector3 color = new Vector3(col.R / 255f, col.G / 255f, col.B / 255f);

            List<Vertex> vertices = new List<Vertex>();
            List<short> indices = new List<short>();

            float charWidth = (float)FontWidth / Config.Width * 2f * Scale; // *2 weil nicht von 0 bis 1 sondern -1 bis 1
            float charHeight = (float)FontHeight / Config.Height * 2f * Scale;

            float offsetx = textPosScreenSpace.X;
            if (string.IsNullOrEmpty(s)) s = " ";

            foreach (var c in s)
            {
                float posXl = offsetx;
                offsetx += charWidth;
                float posXr = offsetx;

                float texL = (c * FontWidth) / TexWidth;
                float texR = texL + FontWidth / TexWidth;

                int count = vertices.Count;
                //vertices:
                // 0  1
                // 2  3

                indices.Add((short)(count + 0));
                indices.Add((short)(count + 2));
                indices.Add((short)(count + 1));

                indices.Add((short)(count + 1));
                indices.Add((short)(count + 2));
                indices.Add((short)(count + 3));

                vertices.Add(new Vertex(new Vector4(posXl, textPosScreenSpace.Y, texL, 0), color));
                vertices.Add(new Vertex(new Vector4(posXr, textPosScreenSpace.Y, texR, 0), color));
                vertices.Add(new Vertex(new Vector4(posXl, textPosScreenSpace.Y - charHeight, texL, 1), color));
                vertices.Add(new Vertex(new Vector4(posXr, textPosScreenSpace.Y - charHeight, texR, 1), color));
            }

            mesh = new Mesh(vertices, indices);
        }


        public void Draw()
        {
            mesh.Render();
        }

        public void Dispose()
        {
            mesh.Dispose();
        }

        public static Vector2 Measure(string s)
        {
            return new Vector2(s.Length * FontWidth * 2, FontHeight*2);
        }
    }
}
