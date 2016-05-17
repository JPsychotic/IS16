using System;
using System.Drawing;
using GameOfLife.RenderEngine.UI.Elements.Intern;
using GameOfLife.RenderEngine.UI.Sidebar;
using SlimDX;
using SlimDX.Direct3D11;

namespace GameOfLife.RenderEngine.UI.Elements
{
  class Rectangle2D : IDrawable2DElement
  {
    private Rect2D rect;
    public object Data;

    public event ChangedEventHandler GotInput;

    public RectangleF BoundingBox => rect.BoundingBox;

    public Rectangle2D(int x, int y, int sizeX, int sizeY, Color4 col)
    {
      rect = new Rect2D(x, y, sizeX, sizeY, col);
    }

    public Rectangle2D(Vector2 pos, int sizeX, int sizeY, Color4 col, Texture2D tex = null)
    {
      rect = new Rect2D((int)pos.X, (int)pos.Y, sizeX, sizeY, col, tex);
    }

    public Rectangle2D(int x, int y, int sizeX, int sizeY, Texture2D tex = null)
    {
      rect = new Rect2D(x, y, sizeX, sizeY, tex);
    }

    public Rectangle2D(Vector2 pos, int sizeX, int sizeY, Texture2D tex = null)
    {
      rect = new Rect2D((int)pos.X, (int)pos.Y, sizeX, sizeY, tex);
    }

    public Rectangle2D(Vector2 pos, int sizeX, int sizeY, Color4 col, Action<object> act, SideBarState state, object data = null, Texture2D tex = null) : this(pos, sizeX, sizeY, col, tex)
    {
      Data = data;
      GotInput += s => { if (state.HasFlag(s)) act(this); };
    }
    
    public Vector2 Location => new Vector2(rect.BoundingBox.Location.X, rect.BoundingBox.Location.Y);
    public Vector2 Size => new Vector2(rect.BoundingBox.Size.Width, rect.BoundingBox.Size.Height);

    public void SetPosition(int x, int y)
    {
      rect.SetPosition(x, y);
    }

    public void SetSize(int sizeX, int sizeY)
    {
      rect.SetSize(sizeX, sizeY);
    }

    public void SetPositionAndSize(int x, int y, int sizeX, int sizeY)
    {
      rect.SetPositionAndSize(x, y, sizeX, sizeY);
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

    public Color4 Color
    {
      get
      {
        return rect.Color;
      }
      set
      {
        rect.Color = value;
        Update();
      }
    }
  }
}
