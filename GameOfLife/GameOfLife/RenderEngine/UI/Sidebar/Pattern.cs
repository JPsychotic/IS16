using System;
using GameOfLife.RenderEngine.UI.Elements;
using SlimDX;
using SlimDX.Direct3D11;
using System.Drawing;

namespace GameOfLife.RenderEngine.UI.Sidebar
{
  internal class Pattern
  {
    public Texture2D Texture;
    public Rectangle2D rect;
    private DrawableString NameString;
    public string Path, World, Name;
    private int SizeX, SizeY, MaxX, MaxY;

    public Pattern(Vector2 loc, int MaxsizeX, int MaxsizeY, string path, string world, string name)
    {
      Texture = Texture2D.FromFile(RenderFrame.Instance.device, path);

      int sizex = 0, sizey = 0;

      var aspect = Texture.Description.Width / (float)Texture.Description.Height;
      if (MaxsizeX / aspect > MaxsizeY)
      {
        sizex = (int)(MaxsizeY * aspect);
        sizey = MaxsizeY;
      }
      else
      {
        sizex = MaxsizeX;
        sizey = (int)(MaxsizeX / aspect);
      }

      Path = path;
      World = world;
      Name = name.Substring(0, Math.Min(name.Length, 12));
      SizeX = sizex;
      SizeY = sizey;
      MaxX = MaxsizeX;
      MaxY = MaxsizeY;
      rect = new Rectangle2D(new Vector2(loc.X + (sizex - MaxsizeX) / 2f, loc.Y + (sizey - MaxsizeY) / 2f), sizex, sizey, Texture);
      UpdateString();
    }

    private void UpdateString()
    {
      NameString?.Dispose();
      var measure = DrawableString.Measure(Name);
      NameString = new DrawableString(Name, rect.Location + new Vector2(rect.Size.X / 2 - measure.X / 2, -measure.Y * 1.5f), Color.White);
    }

    public void SetLocation(int x, int y)
    {
      rect.SetPosition((int)(x - (SizeX - MaxX) / 2f), (int)(y - (SizeY - MaxY) / 2f));
      UpdateString();
    }

    public void Update()
    {

    }

    public void Draw(SpriteBatch sb)
    {
      sb.Draw(rect);
      sb.DrawString(NameString);
    }

    public void Dispose()
    {
      Texture.Dispose();
      NameString.Dispose();
      rect.Dispose();
    }
  }
}
