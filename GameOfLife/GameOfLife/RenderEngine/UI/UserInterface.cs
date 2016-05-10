using System;
using System.Drawing;
using System.Windows.Forms;
using GameOfLife.RenderEngine.UI.Elements;
using GameOfLife.Storage;
using SlimDX;

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

    SideBar sideBar;

    public Userinterface(TextureInput inputHandler)
    {
      string s = "PAUSED";
      pause = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s).X * 2 - position.X, position.Y * 5), Color.OrangeRed);
      fpsString = new DrawableString("FPS: 0", position + new Vector2(Config.Width - 10 * 14, offset.Y), Color.White);
      s = "Thickness: " + Config.LineThickness;
      thickness = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s).X * 2 - position.X, position.Y * 3), Color.White);
      sideBar = new SideBar(inputHandler);
    }

    public void Update(float elapsed)
    {
      timeElapsed += elapsed;
      if (thick != Config.LineThickness)
      {
        thickness.Dispose();
        string s = "Thickness: " + Config.LineThickness;
        thickness = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s).X * 2 - position.X, position.Y *3), Color.White);
        thick = Config.LineThickness;
      }

      if (DateTime.Now.Second != Second)
      {
        Second = DateTime.Now.Second;
        fpsString.Dispose();
        string s = "FPS: " + frames;
        fpsString = new DrawableString(s, new Vector2(Config.Width - DrawableString.Measure(s).X * 2 - position.X, position.Y), Color.White);
        frames = 0;
      }
      frames++;
    }

    public void Draw(SpriteBatch sb)
    {
      sideBar.Draw(sb);
      sb.DrawString(thickness);
      sb.DrawString(fpsString);

      if (Config.Paused && timeElapsed % 1 > 0.5)
      {
        sb.DrawString(pause);
      }
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
      return sideBar.IsPointInsideSidebar(location);
    }
  }
}