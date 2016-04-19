using System;
using System.Drawing;
using GameOfLife.UI.Elements;
using SlimDX;

namespace GameOfLife.RenderEngine.UI.Elements
{
  class Rectangle2D : IDrawable2DElement
  {
    private Rect2D rect;
    private Action action;
    public object Data;

    public event ChangedEventHandler GotInput;

    public Rectangle BoundingBox { get { return rect.BoundingBox; } }

    public Rectangle2D(int x, int y, int sizeX, int sizeY, Color col)
    {
      rect = new Rect2D(x, y, sizeX, sizeY, col);
    }

    public Rectangle2D(Vector2 pos, int sizeX, int sizeY, Color col)
    {
      rect = new Rect2D((int)pos.X, (int)pos.Y, sizeX, sizeY, col);
    }

    public Rectangle2D(Vector2 pos, int sizeX, int sizeY, Color col, Action<object> act, SideBarState state, object data = null) : this(pos, sizeX, sizeY, col)
    {
      Data = data;
      GotInput += (s) => { if (state.HasFlag(s)) act(this); };
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

    public void Dispose()
    {
      rect.Dispose();
      if (GotInput == null) return;
      foreach (var d in GotInput.GetInvocationList())
      {
        GotInput -= (ChangedEventHandler)d;
      }
    }

    public void Draw()
    {
      rect.Draw();
    }

    public void HandleInput(Point loc, SideBarState state)
    {
      if (GotInput != null && rect.BoundingBox.Contains(loc)) GotInput(state);
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
