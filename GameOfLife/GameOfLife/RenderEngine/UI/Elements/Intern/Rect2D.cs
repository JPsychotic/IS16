using System.Collections.Generic;
using System.Drawing;
using GameOfLife.Storage;
using SlimDX;

namespace GameOfLife.RenderEngine.UI.Elements.Intern
{
  class Rect2D
  {
    #region AlternativeConstructors

    public Rect2D(Vector2 pos, Vector2 size, Color col)
        : this((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, col)
    { }

    public Rect2D(Vector2 pos, int sizeX, int sizeY, Color col)
        : this((int)pos.X, (int)pos.Y, sizeX, sizeY, col)
    { }

    public Rect2D(int x, int y, Vector2 size, Color col)
        : this(x, y, (int)size.X, (int)size.Y, col)
    { }

    #endregion AlternativeConstructors

    internal static float width = Config.Width;
    internal static float height = Config.Height;
    internal static List<short> indices = new List<short> { 1, 0, 3, 1, 3, 2 };

    internal Mesh mesh;
    internal int X, Y, SizeY, SizeX;
    internal Color Color;
    internal Rectangle BoundingBox;

    public Rect2D(int x, int y, int sizeX, int sizeY, Color col)
    {
      X = x;
      Y = y;
      SizeX = sizeX;
      SizeY = sizeY;
      Color = col;
      Recreate();
    }

    public void Recreate()
    {
      BoundingBox = new Rectangle(X, Y, SizeX, SizeY);
      mesh?.Dispose();
      mesh = CreateMesh();
    }

    internal Mesh CreateMesh()
    {
      List<Vertex> vertices = new List<Vertex>();

      Vector2 ol = new Vector2((X / width) * 2 - 1, -((Y / height) * 2 - 1));
      Vector2 or = new Vector2((X + SizeX) / width * 2 - 1, -(Y / height * 2 - 1));
      Vector2 ul = new Vector2(X / width * 2 - 1, -((Y + SizeY) / height * 2 - 1));
      Vector2 ur = new Vector2((X + SizeX) / width * 2 - 1, -((Y + SizeY) / height * 2 - 1));

      vertices.Add(new Vertex(new Vector4(ur, 0, 0), new Vector4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f)));
      vertices.Add(new Vertex(new Vector4(ul, 0, 0), new Vector4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f)));
      vertices.Add(new Vertex(new Vector4(ol, 0, 0), new Vector4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f)));
      vertices.Add(new Vertex(new Vector4(or, 0, 0), new Vector4(Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f)));

      return new Mesh(vertices, indices);
    }

    public void Update()
    {

    }

    public void Draw()
    {
      mesh.Render();
    }

    public void Dispose()
    {
      mesh.Dispose();
    }
  }
}
