using System;
using GameOfLife.RenderEngine.UI.Elements;
using SlimDX;
using GameOfLife.Storage;
using System.Drawing;
using GameOfLife.RenderEngine;
using GameOfLife.RenderEngine.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GameOfLife.UI
{
  class SideBar
  {
    TextureInput inputHandler;

    public int Width
    {
      get
      {
        return width;
      }
    }
    private int width = 400;
    public int MinimizedWidth
    {
      get
      {
        return minimizedWidth;
      }
    }
    private int minimizedWidth = 20;
    public bool Minimized
    {
      get
      {
        return minimized;
      }
      set
      {
        minimized = value;
      }
    }
    bool minimized;
    readonly Vector2 offset = new Vector2(0, (int)(0.0185 * Config.Height));
    readonly Color activeTabColor = Color.FromArgb(0, 0, 0, 0);

    Rectangle2D sideBarBackground;
    Rectangle2D leftTab;
    Rectangle2D rightTab;
    Rectangle2D minimize;
    Rectangle2D maximize;

    Rectangle2D pauseBtn;
    Rectangle2D clearBtn;

    Rectangle2D oneBirth;
    Rectangle2D twoBirth;
    Rectangle2D threeBirth;
    Rectangle2D fourBirth;
    Rectangle2D fiveBirth;
    Rectangle2D sixBirth;
    Rectangle2D sevenBirth;
    Rectangle2D eigthBirth;

    Rectangle2D oneDeath;
    Rectangle2D twoDeath;
    Rectangle2D threeDeath;
    Rectangle2D fourDeath;
    Rectangle2D fiveDeath;
    Rectangle2D sixDeath;
    Rectangle2D sevenDeath;
    Rectangle2D eigthDeath;

    List<DrawableString> leftTabStrings;
    List<DrawableString> rightTabStrings;

    DrawableString leftTabString;
    DrawableString rightTabString;
    DrawableString minimizeString;
    DrawableString maximizeString;

    public SideBar(TextureInput iHandler)
    {
      minimized = true;
      inputHandler = iHandler;
      leftTabStrings = new List<DrawableString>();
      rightTabStrings = new List<DrawableString>();
      sideBarBackground = new Rectangle2D(new Vector2(0, 0), width, Config.Height, Color.FromArgb(200, 200, 200, 200));
      leftTab = new Rectangle2D(new Vector2(0, 0), width / 2, (int)(0.074 * Config.Height), Color.DimGray);
      rightTab = new Rectangle2D(new Vector2(width / 2, 0), width / 2, (int)(0.074 * Config.Height), activeTabColor);
      minimize = new Rectangle2D(new Vector2(0, Config.Height - (int)(0.093 * Config.Height)), width, (int)(0.093 * Config.Height), Color.DimGray);
      maximize = new Rectangle2D(new Vector2(0, 0), minimizedWidth, Config.Height, Color.FromArgb(200, 200, 200, 200));

      leftTabString = new DrawableString("Muster", new Vector2(65, 5) + offset, Color.White);
      rightTabString = new DrawableString("Einstellungen", new Vector2(width / 2 + 25, 5) + offset, Color.White);
      minimizeString = new DrawableString("Einklappen", new Vector2(140, Config.Height - (int)(0.06 * Config.Height)), Color.White);
      maximizeString = new DrawableString(">", new Vector2(5, Config.Height / 2), Color.White);

      pauseBtn = new Rectangle2D(new Vector2(25, (int)(0.12 * Config.Height)), 150, (int)(0.093 * Config.Height), Color.DimGray);
      clearBtn = new Rectangle2D(new Vector2(width / 2 + 25, (int)(0.12 * Config.Height)), 150, (int)(0.093 * Config.Height), Color.DimGray);

      oneBirth = new Rectangle2D(new Vector2(70, (int)(-0.011 * Config.Height)) + 19 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      oneDeath = new Rectangle2D(new Vector2(250, (int)(-0.011 * Config.Height)) + 19 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      twoBirth = new Rectangle2D(new Vector2(70, (int)(-0.011 * Config.Height)) + 22 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      twoDeath = new Rectangle2D(new Vector2(250, (int)(-0.011 * Config.Height)) + 22 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      threeBirth = new Rectangle2D(new Vector2(70, (int)(-0.011 * Config.Height)) + 25 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      threeDeath = new Rectangle2D(new Vector2(250, (int)(-0.011 * Config.Height)) + 25 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      fourBirth = new Rectangle2D(new Vector2(70, (int)(-0.011 * Config.Height)) + 28 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      fourDeath = new Rectangle2D(new Vector2(250, (int)(-0.011 * Config.Height)) + 28 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      fiveBirth = new Rectangle2D(new Vector2(70, (int)(-0.011 * Config.Height)) + 31 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      fiveDeath = new Rectangle2D(new Vector2(250, (int)(-0.011 * Config.Height)) + 31 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      sixBirth = new Rectangle2D(new Vector2(70, (int)(-0.011 * Config.Height)) + 34 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      sixDeath = new Rectangle2D(new Vector2(250, (int)(-0.011 * Config.Height)) + 34 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      sevenBirth = new Rectangle2D(new Vector2(70, (int)(-0.011 * Config.Height)) + 37 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      sevenDeath = new Rectangle2D(new Vector2(250, (int)(-0.011 * Config.Height)) + 37 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      eigthBirth = new Rectangle2D(new Vector2(70, (int)(-0.011 * Config.Height)) + 40 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);
      eigthDeath = new Rectangle2D(new Vector2(250, (int)(-0.011 * Config.Height)) + 40 * offset, 100, (int)(0.046 * Config.Height), Color.DimGray);

      rightTabStrings.Add(new DrawableString("Pause", new Vector2(65, 5) + 8 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("Leeren", new Vector2(260, 5) + 8 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("Leben", new Vector2(80, 0) + 15 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("Tod", new Vector2(280, 0) + 15 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("1", new Vector2(10, 0) + 19 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("2", new Vector2(10, 0) + 22 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("3", new Vector2(10, 0) + 25 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("4", new Vector2(10, 0) + 28 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("5", new Vector2(10, 0) + 31 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("6", new Vector2(10, 0) + 34 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("7", new Vector2(10, 0) + 37 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("8", new Vector2(10, 0) + 40 * offset, Color.White));
    }

    internal void Dispose()
    {
      sideBarBackground.Dispose();
      leftTab.Dispose();
      rightTab.Dispose();
      leftTabString.Dispose();
      rightTabString.Dispose();
      foreach (DrawableString d in rightTabStrings)
      {
        d.Dispose();
      }
      foreach (DrawableString d in leftTabStrings)
      {
        d.Dispose();
      }
      oneBirth.Dispose();
      oneDeath.Dispose();
      twoBirth.Dispose();
      twoDeath.Dispose();
      threeBirth.Dispose();
      threeDeath.Dispose();
      fourBirth.Dispose();
      fourDeath.Dispose();
      fiveBirth.Dispose();
      fiveDeath.Dispose();
      sixBirth.Dispose();
      sixDeath.Dispose();
      sevenBirth.Dispose();
      sevenDeath.Dispose();
      eigthBirth.Dispose();
      eigthDeath.Dispose();
      pauseBtn.Dispose();
      clearBtn.Dispose();
      minimize.Dispose();
      minimizeString.Dispose();
      maximize.Dispose();
      maximizeString.Dispose();
    }

    public void Draw(SpriteBatch sb)
    {
      if (!minimized)
      {
        sideBarBackground.Draw(sb);
        leftTab.Draw(sb);
        rightTab.Draw(sb);
        sb.DrawString(leftTabString);
        sb.DrawString(rightTabString);
        if (rightTab.Color == activeTabColor)
        {
          pauseBtn.Draw(sb);
          clearBtn.Draw(sb);
          foreach (DrawableString d in rightTabStrings)
          {
            sb.DrawString(d);
          }
          oneBirth.Draw(sb);
          oneDeath.Draw(sb);
          twoBirth.Draw(sb);
          twoDeath.Draw(sb);
          threeBirth.Draw(sb);
          threeDeath.Draw(sb);
          fourBirth.Draw(sb);
          fourDeath.Draw(sb);
          fiveBirth.Draw(sb);
          fiveDeath.Draw(sb);
          sixBirth.Draw(sb);
          sixDeath.Draw(sb);
          sevenBirth.Draw(sb);
          sevenDeath.Draw(sb);
          eigthBirth.Draw(sb);
          eigthDeath.Draw(sb);
        }
        else if (leftTab.Color == activeTabColor)
        { }
        minimize.Draw(sb);
        sb.DrawString(minimizeString);
      }
      else
      {
        maximize.Draw(sb);
        sb.DrawString(maximizeString);
      }
    }

    public void HandleMouseClick(object sender, MouseEventArgs a)
    {
      if(a.X <= width / 2 && a.Y <= (int)(0.074 * Config.Height)) // Linker Tab
      {
        leftTab.Color = activeTabColor;
        rightTab.Color = Color.DimGray;
      }
      else if(a.X > 200 && a.Y <= (int)(0.074 * Config.Height)) // Rechter Tab
      {
        leftTab.Color = Color.DimGray;
        rightTab.Color = activeTabColor;
      }
      else if (a.Y >= Config.Height - (int)(0.093 * Config.Height)) //Einklappen
      {
        minimized = true;
      }
      else if(leftTab.Color == activeTabColor)
      {
        
      }
      else if(rightTab.Color == activeTabColor)
      { 
        if (a.X >= 25 && a.X <= 175 && a.Y >= (int)(0.12 * Config.Height) && a.Y <= (int)(0.213 * Config.Height)) // Pause
        {
          Config.Paused = !Config.Paused;
        }
        else if(a.X >= 225 && a.X <= 375 && a.Y >= (int)(0.12 * Config.Height) && a.Y <= (int)(0.213 * Config.Height)) //Clear
        {
          inputHandler.ClearWorld();
        }
      }
    }
  }

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
      pause = new DrawableString("PAUSED (P)", position + new Vector2(Config.Width - 10 * 14, 0), Color.OrangeRed);
      fpsString = new DrawableString("FPS: 0", position, Color.White);
      thickness = new DrawableString("Thickness: " + Config.LineThickness, position + offset, Color.White);
      sideBar = new SideBar(inputHandler);
    }

    public void Update(float elapsed)
    {
      timeElapsed += elapsed;
      if (thick != Config.LineThickness)
      {
        thickness.Dispose();
        thickness = new DrawableString("Thickness: " + Config.LineThickness, position + offset, Color.White);
        thick = Config.LineThickness;
      }

      if (DateTime.Now.Second != Second)
      {
        Second = DateTime.Now.Second;
        fpsString.Dispose();
        fpsString = new DrawableString("FPS: " + frames, position, Color.White);
        frames = 0;
      }
      frames++;
    }

    public void Draw(SpriteBatch sb)
    {
      sideBar.Draw(sb);
      //sb.DrawString(thickness);
      //sb.DrawString(fpsString);

      if (Config.Paused)// && timeElapsed % 1 > 0.5)
      {
        sb.DrawString(pause);
      }
    }

    internal void Dispose()
    {
      fpsString.Dispose();
      thickness.Dispose();
      pause.Dispose();
      sideBar.Dispose();
    }

    public bool OnMouseClick(object sender, MouseEventArgs a)
    {
      if (a.Button == MouseButtons.Left && a.X <= sideBar.Width && !sideBar.Minimized)
      {
        sideBar.HandleMouseClick(sender, a);
        return true;
      }
      else if(a.Button == MouseButtons.Left && a.X <= sideBar.MinimizedWidth && sideBar.Minimized && a.Clicks == 1)
      {
        sideBar.Minimized = false;
        return true;
      }
      else
        return false;
    }
  }
}