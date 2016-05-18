using System;
using System.Drawing;
using System.Windows.Forms;
using GameOfLife.RenderEngine.UI.Elements;
using GameOfLife.RenderEngine.UI.Sidebar;
using GameOfLife.Storage;
using SlimDX;
using SlimDX.Direct3D11;

namespace GameOfLife.RenderEngine.UI
{
  class Userinterface
  {
    int frames, Second, thick = Config.LineThickness;
    readonly Vector2 position = new Vector2(10, 10);
    readonly Vector2 offset = new Vector2(0, 20);
    float timeElapsed;

    DrawableString thickness, pause;
    DrawableString fpsString;
    DrawableString stef, tobi, jerry;
    Rectangle2D oth;
    Texture2D othTex;

    SideBar sideBar;

    public Userinterface(TextureInput inputHandler)
    {
      string s = "PAUSED";
      pause = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s).X - position.X, position.Y * 5), Color.OrangeRed);
      fpsString = new DrawableString("FPS: 0", position + new Vector2(Config.Width - 10 * 14, offset.Y), Color.White);
      s = "Thickness: " + Config.LineThickness;
      thickness = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s).X - position.X, position.Y * 3), Color.White);
      sideBar = new SideBar(inputHandler);
      s = "Stefan P" + (char)('o' + 42) + "lloth";
      stef = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s, 1).X - position.X, Config.Height - position.Y * 4), Color.White, 1);
      s = "Tobias Nickl";
      tobi = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s, 1).X - position.X, Config.Height - position.Y * 3), Color.White, 1);
      s = "Jeremy Probst";
      jerry = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s, 1).X - position.X, Config.Height - position.Y * 2), Color.White, 1);

      float scale = 1 / 12f;
      othTex = Texture2D.FromFile(RenderFrame.Instance.device, @".\Content\oth.png");
      oth = new Rectangle2D(new Vector2(Config.Width - Config.Width * scale * 1.2f - 10, stef.Location.Y - (int)(Config.Height * scale) - 10), (int)(Config.Width * scale * 1.2f), (int)(Config.Height * scale), othTex);
      oth.Color = new Color4(0.4f, 1, 1, 1);
    }

    public void Update(float elapsed)
    {
      timeElapsed += elapsed;
      if (thick != Config.LineThickness)
      {
        thickness.Dispose();
        string s = "Thickness: " + Config.LineThickness;
        thickness = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s).X - position.X, position.Y * 3), Color.White);
        thick = Config.LineThickness;
      }

      if (DateTime.Now.Second != Second)
      {
        Second = DateTime.Now.Second;
        fpsString.Dispose();
        string s = "FPS: " + frames;
        fpsString = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s).X - position.X, position.Y), Color.White);
        frames = 0;
      }
      frames++;
    }

    public void Draw(SpriteBatch sb)
    {
      sideBar.Draw(sb);
      sb.Draw(oth);
      sb.DrawString(thickness);
      sb.DrawString(fpsString);
      sb.DrawString(stef);
      sb.DrawString(tobi);
      sb.DrawString(jerry);

      if (Config.Paused && timeElapsed % 1 > 0.5)
      {
        sb.DrawString(pause);
      }
    }

    public bool OnMouseDown(object sender, MouseEventArgs a)
    {
      if (a.Button != MouseButtons.Left || a.Button == MouseButtons.None) return false;
      return sideBar.OnMouseDown(a.Location);
    }

    public bool OnMouseClick(object sender, MouseEventArgs a)
    {
      if (a.Button != MouseButtons.Left || a.Button == MouseButtons.None) return false;
      return sideBar.HandleMouseClick(a.Location);
    }

    public bool OnMouseMove(object sender, MouseEventArgs a)
    {
      if (a.Button != MouseButtons.Left || a.Button == MouseButtons.None) return false;
      return sideBar.HandleMouseMove(a.Location);
    }

    internal void Dispose()
    {
      fpsString.Dispose();
      thickness.Dispose();
      pause.Dispose();
      sideBar.Dispose();
    }

    internal bool IsPointInUI(Point location)
    {
      return sideBar.IsPointOnUI(location);
    }
  }
}