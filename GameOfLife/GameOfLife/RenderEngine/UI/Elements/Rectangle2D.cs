using System.Drawing;
using GameOfLife.UI;
using GameOfLife.UI.Elements;
using SlimDX;

namespace GameOfLife.RenderEngine.UI.Elements
{
  class Rectangle2D
  {
    private Rect2D rect;
    public Rectangle BoundingBox { get { return rect.BoundingBox; } }

    public Rectangle2D(int x, int y, int sizeX, int sizeY, Color col)
    {
      rect = new Rect2D(x, y, sizeX, sizeY, col);
    }

    public Rectangle2D(Vector2 pos, int sizeX, int sizeY, Color col)
    {
      rect = new Rect2D((int)pos.X, (int)pos.Y, sizeX, sizeY, col);
    }

    public void SetPosition(Vector2 pos)
    {
      rect.X = (int)pos.X;
      rect.Y = (int)pos.Y;
      rect.Recreate();
    }

    public void SetSize(Vector2 size)
    {
      rect.SizeX = (int)size.X;
      rect.SizeY = (int)size.Y;
      rect.Recreate();
    }

    public void SetPositionAndSize(Vector2 pos, Vector2 size)
    {
      rect.X = (int)pos.X;
      rect.Y = (int)pos.Y;
      rect.SizeX = (int)size.X;
      rect.SizeY = (int)size.Y;
      rect.Recreate();
    }

    public void Update()
    {
      rect.Update();
    }

    public void Draw(SpriteBatch sb)
    {
      sb.Draw(rect);
    }

    public void Dispose()
    {
      rect.Dispose();
    }

    public Color Color
    {
      get
      {
        return rect.Color;
      }
      set
      {
        rect.Color = value;
        rect.Recreate();
      }
    }
  }
}
