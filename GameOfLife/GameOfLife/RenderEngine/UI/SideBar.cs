using GameOfLife.RenderEngine.UI.Elements;
using GameOfLife.Storage;
using GameOfLife.UI;
using GameOfLife.UI.Elements;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GameOfLife.RenderEngine.UI
{
  class SideBar
  {
    TextureInput inputHandler;

    public int Width = 400;
    public int MinimizedWidth = 20;
    public SideBarState State = SideBarState.Minimized;
    readonly Vector2 offset = new Vector2(0, (int)(0.0185 * Config.Height));
    readonly Color activeTabColor = Color.FromArgb(0, 0, 0, 0);

    List<IDrawable2DElement> sideBarBackground = new List<IDrawable2DElement>();
    List<IDrawable2DElement> leftTab = new List<IDrawable2DElement>();
    List<IDrawable2DElement> rightTab = new List<IDrawable2DElement>();
    Rectangle2D maximize;

    List<DrawableString> leftTabStrings = new List<DrawableString>();
    List<DrawableString> rightTabStrings = new List<DrawableString>();

    DrawableString maximizeString;

    public delegate void ClickEventHandler(Point p, SideBarState s);
    public event ClickEventHandler GotInputClick;

    List<Rectangle2D> birth = new List<Rectangle2D>();
    List<Rectangle2D> death = new List<Rectangle2D>();

    public SideBar(TextureInput iHandler)
    {
      inputHandler = iHandler;

      sideBarBackground.Add(new Rectangle2D(new Vector2(0, 0), Width, Config.Height, Color.FromArgb(200, 200, 200, 200)));
      sideBarBackground.Add(new Rectangle2D(new Vector2(0, 0), Width / 2, (int)(0.074 * Config.Height), Color.DimGray, (s) => State = SideBarState.LeftTab, SideBarState.RightTab)); // left tab
      GotInputClick += sideBarBackground.Last().HandleInput;
      sideBarBackground.Add(new Rectangle2D(new Vector2(Width / 2, 0), Width / 2, (int)(0.074 * Config.Height), activeTabColor, (s) => State = SideBarState.RightTab, SideBarState.LeftTab)); //right tab
      GotInputClick += sideBarBackground.Last().HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(0, Config.Height - (int)(0.093 * Config.Height)), Width, (int)(0.093 * Config.Height), Color.DimGray, (s) => State = SideBarState.Minimized, SideBarState.LeftTab | SideBarState.RightTab));
      GotInputClick += rightTab.Last().HandleInput;
      maximize = new Rectangle2D(new Vector2(0, 0), MinimizedWidth, Config.Height, Color.FromArgb(200, 200, 200, 200), (s) => State = SideBarState.RightTab, SideBarState.Minimized);
      GotInputClick += maximize.HandleInput;

      var leftTabString = new DrawableString("Muster", new Vector2((float)0.1625 * Width, 5) + offset, Color.White);
      var rightTabString = new DrawableString("Einstellungen", new Vector2(Width / 2 + (float)0.0625 * Width, 5) + offset, Color.White);
      var minimizeString = new DrawableString("Einklappen", new Vector2((float)0.35 * Width, Config.Height - (int)(0.06 * Config.Height)), Color.White);
      maximizeString = new DrawableString(">", new Vector2((float)0.0125 * Width, Config.Height / 2), Color.White);

      rightTab.Add(new Rectangle2D(new Vector2((float)0.0625 * Width, (int)(0.12 * Config.Height)), (int)(0.375 * Width), (int)(0.093 * Config.Height), Color.DimGray, (s) => Config.Paused = !Config.Paused, SideBarState.RightTab));
      GotInputClick += rightTab.Last().HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(Width / 2 + (float)0.0625 * Width, (int)(0.12 * Config.Height)), (int)(0.375 * Width), (int)(0.093 * Config.Height), Color.DimGray, (s) => inputHandler.ClearWorld(), SideBarState.RightTab));
      GotInputClick += rightTab.Last().HandleInput;

      // Birth setting buttons
      for (int i = 0; i < 9; i++)
      {
        birth.Add(new Rectangle2D(new Vector2((float)0.175 * Width, (int)(-0.011 * Config.Height)) + (19 + i * 3) * offset, (int)(0.25 * Width), (int)(0.046 * Config.Height), (Config.BirthRule & 1 << i) > 0 ? Color.Green : Color.DimGray, OnBirthChanged, SideBarState.RightTab, i));

      }
      // Death setting buttons
      for (int i = 0; i < 9; i++)
      {
        death.Add(new Rectangle2D(new Vector2((float)0.625 * Width, (int)(-0.011 * Config.Height)) + (19 + i * 3) * offset, (int)(0.25 * Width), (int)(0.046 * Config.Height), (Config.DeathRule & 1 << i) > 0 ? Color.Green : Color.DimGray, OnDeathChanged, SideBarState.RightTab, i));
      }

      foreach (var r in birth.Concat(death))
      {
        rightTab.Add(r);
        GotInputClick += r.HandleInput;
      }

      rightTabStrings.Add(new DrawableString("Pause", new Vector2((float)0.1625 * Width, 5) + 8 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("Leeren", new Vector2((float)0.65 * Width, 5) + 8 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("Leben", new Vector2((float)0.2 * Width, 0) + 15 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("Tod", new Vector2((float)0.7 * Width, 0) + 15 * offset, Color.White));
      for (int i = 0; i < 9; i++)
      {
        rightTabStrings.Add(new DrawableString(i.ToString(), new Vector2((float)0.025 * Width, 0) + (19 + 3 * i) * offset, Color.White));
      }
      rightTabStrings.Add(minimizeString);

      rightTabStrings.Add(rightTabString);
      leftTabStrings.Add(rightTabString);

      rightTabStrings.Add(leftTabString);
      leftTabStrings.Add(leftTabString);
    }

    private void OnBirthChanged(object sender)
    {
      int index = (int)((Rectangle2D)sender).Data;
      Config.BirthRule ^= (uint)(1 << index);
      ((Rectangle2D)sender).Color = (Config.BirthRule & 1 << index) > 0 ? Color.Green : Color.DimGray;
    }

    private void OnDeathChanged(object sender)
    {
      int index = (int)((Rectangle2D)sender).Data;
      Config.DeathRule ^= (uint)(1 << index);
      ((Rectangle2D)sender).Color = (Config.DeathRule & 1 << index) > 0 ? Color.Green : Color.DimGray;
    }

    public void Draw(SpriteBatch sb)
    {
      if (State == SideBarState.Minimized)
      {
        sb.Draw(maximize);
        sb.DrawString(maximizeString);
        return;
      }

      sb.Draw(sideBarBackground);
      if (State == SideBarState.LeftTab)
      {
        sb.Draw(leftTab);
        sb.DrawString(leftTabStrings);

      }
      else if (State == SideBarState.RightTab)
      {
        sb.Draw(rightTab);
        sb.DrawString(rightTabStrings);
      }
    }

    public bool IsPointInsideSidebar(Point loc)
    {
      return ((loc.X <= Width && State != SideBarState.Minimized) || (loc.X <= MinimizedWidth && State == SideBarState.Minimized));
    }

    public bool HandleMouseMove(Point loc)
    {
      return IsPointInsideSidebar(loc);
    }

    public bool HandleMouseClick(Point loc)
    {
      if (IsPointInsideSidebar(loc))
      {
        GotInputClick(loc, State);
        return true;
      }
      return false;
    }

    internal void Dispose()
    {
      maximize?.Dispose();
      maximizeString?.Dispose();

      foreach (var r in sideBarBackground) r.Dispose();
      foreach (var s in rightTabStrings) s.Dispose();
      foreach (var s in leftTabStrings) s.Dispose();
      foreach (var r in leftTab) r.Dispose();
      foreach (var r in rightTab) r.Dispose();
    }
  }

  [Flags]
  public enum SideBarState
  {
    Minimized = 0,
    LeftTab = 1,
    RightTab = 2,
  }
}
