using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using GameOfLife.RenderEngine.UI.Elements;
using GameOfLife.Storage;
using SlimDX;
using SlimDX.Direct3D11;

namespace GameOfLife.RenderEngine.UI.Sidebar
{
  class SideBar
  {
    TextureInput inputHandler;
    PatternManager pm;

    public int Width = 400;
    public int MinimizedWidth = 20;
    public SideBarState State = SideBarState.Minimized;
    readonly Vector2 offset = new Vector2(0, (int)(0.0185 * Config.Height));
    Texture2D screenshotTex, fileLoadTex, plusTex, minusTex, playTex, pauseTex, exitTex, clearTex, cornerTex;

    List<IDrawable2DElement> sideBarBackground = new List<IDrawable2DElement>();
    List<IDrawable2DElement> leftTab = new List<IDrawable2DElement>();
    List<IDrawable2DElement> rightTab = new List<IDrawable2DElement>();
    Rectangle2D maximize, slider, sliderbackground;

    List<DrawableString> leftTabStrings = new List<DrawableString>();
    List<DrawableString> rightTabStrings = new List<DrawableString>();

    DrawableString maximizeString;

    public delegate void ClickEventHandler(Point p, SideBarState s);
    public event ClickEventHandler GotInputClick;

    List<Rectangle2D> birth = new List<Rectangle2D>();
    List<Rectangle2D> death = new List<Rectangle2D>();
    float speed = 0;

    public SideBar(TextureInput iHandler)
    {
      inputHandler = iHandler;

      var d = RenderFrame.Instance.device;
      screenshotTex = Texture2D.FromFile(d, @".\Content\floppy_save.png");
      fileLoadTex = Texture2D.FromFile(d, @".\Content\file_open.png");
      minusTex = Texture2D.FromFile(d, @".\Content\minus.png");
      plusTex = Texture2D.FromFile(d, @".\Content\plus.png");

      playTex = Texture2D.FromFile(d, @".\Content\playpause.png");
      clearTex = Texture2D.FromFile(d, @".\Content\clear.png");
      exitTex = Texture2D.FromFile(d, @".\Content\exit.png");
      cornerTex = Texture2D.FromFile(d, @".\Content\corner.png");

      // hintergrund
      sideBarBackground.Add(new Rectangle2D(new Vector2(0, 0), Width, Config.Height, Color.FromArgb(200, 200, 200, 200)));
      // 2 button oben zum umschalten zwischen den tabs
      rightTab.Add(new Rectangle2D(new Vector2(0, 0), Width / 2, (int)(0.074 * Config.Height), Color.DimGray, (s) => State = SideBarState.LeftTab, SideBarState.RightTab)); // left tab
      GotInputClick += rightTab.Last().HandleInput;
      leftTab.Add(new Rectangle2D(new Vector2(Width / 2f, 0), Width / 2, (int)(0.074 * Config.Height), Color.DimGray, (s) => State = SideBarState.RightTab, SideBarState.LeftTab)); //right tab
      GotInputClick += leftTab.Last().HandleInput;

      sideBarBackground.Add(new Rectangle2D(new Vector2(0, Config.Height - (int)(0.074 * Config.Height)), Width, (int)(0.093 * Config.Height), Color.DimGray, (s) => State = SideBarState.Minimized, SideBarState.LeftTab | SideBarState.RightTab));
      GotInputClick += sideBarBackground.Last().HandleInput;
      maximize = new Rectangle2D(new Vector2(0, 0), MinimizedWidth, Config.Height, Color.FromArgb(200, 200, 200, 200), (s) => State = SideBarState.RightTab, SideBarState.Minimized);
      GotInputClick += maximize.HandleInput;

      var leftTabString = new DrawableString("Muster", new Vector2((float)0.1625 * Width, 5) + offset, Color.White);
      var rightTabString = new DrawableString("Allgemein", new Vector2(Width / 2f + (float)0.11 * Width, 5) + offset, Color.White);
      var minimizeString = new DrawableString("Einklappen", new Vector2((float)0.35 * Width, Config.Height - (int)(0.05 * Config.Height)), Color.White);
      maximizeString = new DrawableString(">", new Vector2((float)0.0125 * Width, Config.Height / 2f), Color.White);

      int btnSize = (int)(0.15 * Width);

      var closebtn = new Rectangle2D(new Vector2(0.1875f * Width, 0.093f * Config.Height), btnSize, btnSize, Color.OrangeRed, (s) => RenderFrame.Instance.Exit(), SideBarState.RightTab, null, exitTex);
      rightTab.Add(closebtn);
      GotInputClick += closebtn.HandleInput;
      var pausebtn = new Rectangle2D(new Vector2(0.5625f * Width, 0.093f * Config.Height), btnSize, btnSize, Color.White, (s) => { Config.Paused = !Config.Paused; }, SideBarState.RightTab, null, playTex);
      rightTab.Add(pausebtn);
      GotInputClick += pausebtn.HandleInput;
      var clearbtn = new Rectangle2D(new Vector2(0.375f * Width, 0.093f * Config.Height), btnSize, btnSize, Color.White, (s) => inputHandler.ClearWorld(), SideBarState.RightTab, null, clearTex);
      rightTab.Add(clearbtn);
      GotInputClick += clearbtn.HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(0.75f * Width, 0.093f * Config.Height), btnSize, btnSize, Color.White, (s) => SamplerStates.Instance.SwitchWrapMode(), SideBarState.RightTab, null, cornerTex));
      GotInputClick += rightTab.Last().HandleInput;


      //Farben
      rightTab.Add(new Rectangle2D(new Vector2(0.1875f * Width, 0.27f * Config.Height), btnSize, btnSize, Color.Red, (s) => inputHandler.ChangeColor(new Color4(1, 1, 0, 0)), SideBarState.RightTab));
      GotInputClick += rightTab.Last().HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(0.375f * Width, 0.27f * Config.Height), btnSize, btnSize, Color.Green, (s) => inputHandler.ChangeColor(new Color4(1, 0, 1, 0)), SideBarState.RightTab));
      GotInputClick += rightTab.Last().HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(0.5625f * Width, 0.27f * Config.Height), btnSize, btnSize, Color.Blue, (s) => inputHandler.ChangeColor(new Color4(1, 0, 0, 1)), SideBarState.RightTab));
      GotInputClick += rightTab.Last().HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(0.75f * Width, 0.27f * Config.Height), btnSize, btnSize, Color.Black, (s) => inputHandler.ChangeColor(new Color4(1, 0, 0, 0)), SideBarState.RightTab));
      GotInputClick += rightTab.Last().HandleInput;

      rightTab.Add(new Rectangle2D(new Vector2(0.1875f * Width, 0.18f * Config.Height), btnSize, btnSize, Color.White, (s) => MakeScreenshot(), SideBarState.RightTab, null, screenshotTex));
      GotInputClick += rightTab.Last().HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(0.375f * Width, 0.18f * Config.Height), btnSize, btnSize, Color.White, (s) => LoadFile(), SideBarState.RightTab, null, fileLoadTex));
      GotInputClick += rightTab.Last().HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(0.5625f * Width, 0.18f * Config.Height), btnSize, btnSize, Color.White, (s) => Config.LineThickness--, SideBarState.RightTab, null, minusTex));
      GotInputClick += rightTab.Last().HandleInput;
      rightTab.Add(new Rectangle2D(new Vector2(0.75f * Width, 0.18f * Config.Height), btnSize, btnSize, Color.White, (s) => Config.LineThickness++, SideBarState.RightTab, null, plusTex));
      GotInputClick += rightTab.Last().HandleInput;

      // Birth setting buttons
      for (int i = 1; i < 9; i++)
      {
        birth.Add(new Rectangle2D(new Vector2((float)0.175 * Width, (int)(-0.011 * Config.Height)) + (24 + i * 3) * offset, (int)(0.25 * Width), (int)(0.046 * Config.Height), (Config.BirthRule & 1 << i) > 0 ? Color.Green : Color.DimGray, OnBirthChanged, SideBarState.RightTab, i));
      }
      // Death setting buttons
      for (int i = 1; i < 9; i++)
      {
        death.Add(new Rectangle2D(new Vector2((float)0.625 * Width, (int)(-0.011 * Config.Height)) + (24 + i * 3) * offset, (int)(0.25 * Width), (int)(0.046 * Config.Height), (Config.DeathRule & 1 << i) > 0 ? Color.Green : Color.DimGray, OnDeathChanged, SideBarState.RightTab, i));
      }

      foreach (var r in birth.Concat(death))
      {
        rightTab.Add(r);
        GotInputClick += r.HandleInput;
      }
      rightTabStrings.Add(new DrawableString("Leben", new Vector2((float)0.235 * Width, 0) + 24 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("Tod", new Vector2((float)0.705 * Width, 0) + 24 * offset, Color.White));
      rightTabStrings.Add(new DrawableString("Farbe", new Vector2(10, 0.275f * Config.Height + btnSize / 2f - offset.Y), Color.White));
      for (int i = 1; i < 9; i++)
      {
        rightTabStrings.Add(new DrawableString(i.ToString(), new Vector2((float)0.10 * Width, 0) + (24 + 3 * i) * offset, Color.White));
      }
      rightTabStrings.Add(minimizeString);
      leftTabStrings.Add(minimizeString);

      rightTabStrings.Add(rightTabString);
      leftTabStrings.Add(rightTabString);

      rightTabStrings.Add(leftTabString);
      leftTabStrings.Add(leftTabString);

      sliderbackground = new Rectangle2D(new Vector2(0.1875f * Width, (int)(0.36f * Config.Height)), (int)(0.714f * Width), (int)(0.046 * Config.Height), Color.DarkGray, s => speed = location.X, SideBarState.RightTab);
      slider = new Rectangle2D(new Vector2(0.1875f * Width, (int)(0.36f * Config.Height)), (int)(0.02 * Width), (int)(0.046 * Config.Height), Color.White);
      GotInputClick += sliderbackground.HandleInput;

      rightTabStrings.Add(new DrawableString("Langsam", new Vector2(sliderbackground.Location.X + 10, sliderbackground.Location.Y + sliderbackground.Size.Y / 2 - DrawableString.Measure("a", 1).Y / 2), Color.White, 1));
      rightTabStrings.Add(new DrawableString("Schnell", new Vector2(sliderbackground.Location.X + sliderbackground.Size.X - 10 - DrawableString.Measure("Schnell", 1).X, sliderbackground.Location.Y + sliderbackground.Size.Y / 2 - DrawableString.Measure("a", 1).Y / 2), Color.White, 1));

      rightTab.Add(sliderbackground);
      rightTab.Add(slider);

      pm = new PatternManager(0, (int)(0.074 * Config.Height), Width, Config.Height - 2 * (int)(0.074 * Config.Height), this); //oberen und unteren button abziehen ....

      speed = 0.1875f * Width * 3f;
    }

    private void OnBirthChanged(object sender)
    {
      int index = (int)((Rectangle2D)sender).Data;
      Config.BirthRule ^= (uint)(1 << index);
      birth[index - 1].Color = (Config.BirthRule & 1 << index) > 0 ? Color.Green : Color.DimGray;

      if ((Config.BirthRule & 1 << index) > 0 && (Config.DeathRule & 1 << index) > 0)
      {
        Config.DeathRule ^= (uint)(1 << index);
        death[index - 1].Color = Color.DimGray;
      }
    }

    private void OnDeathChanged(object sender)
    {
      int index = (int)((Rectangle2D)sender).Data;
      Config.DeathRule ^= (uint)(1 << index);
      death[index - 1].Color = (Config.DeathRule & 1 << index) > 0 ? Color.Green : Color.DimGray;

      if ((Config.DeathRule & 1 << index) > 0 && (Config.BirthRule & 1 << index) > 0)
      {
        Config.BirthRule ^= (uint)(1 << index);
        birth[index - 1].Color = Color.DimGray;
      }
    }

    public void Draw(SpriteBatch sb)
    {
      float widthMin = 0.1875f * Width;
      float widthMax = 0.7f * Width;
      slider.SetPosition((int)(speed - slider.Size.X / 2f), (int)slider.Location.Y);
      int sleepMS = (int)((1 - (speed - widthMin) / widthMax) * 12);
      Config.Delay = (1 << sleepMS) >> 1;

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
        pm.Draw(sb);
      }
      else if (State == SideBarState.RightTab)
      {
        sb.Draw(rightTab);
        sb.DrawString(rightTabStrings);
      }
    }

    internal bool OnMouseDown(Point loc)
    {
      if (State == SideBarState.LeftTab)
      {
        pm.HandleMouseDown(loc);
        return true;
      }
      return false;
    }

    internal bool IsPointOnUI(Point loc)
    {
      return ((loc.X <= Width && State != SideBarState.Minimized) ||
        (loc.X <= MinimizedWidth && State == SideBarState.Minimized))
        || pm.Contains(loc);
    }

    Point location = new Point();
    public bool HandleMouseMove(Point loc)
    {
      location = loc;
      pm.HandleMouseMove(loc);
      return IsPointOnUI(loc);
    }

    public bool HandleMouseClick(Point loc)
    {
      location = loc;
      if (State == SideBarState.LeftTab && pm.Contains(loc))
      {
        pm.HandleMouseClick(loc, inputHandler.SelectedColor);
        return true;
      }
      if (IsPointOnUI(loc))
      {
        GotInputClick?.Invoke(loc, State);
        return true;
      }
      return false;
    }

    public void MakeScreenshot()
    {
      try
      {
        var folder = "Screenshots";
        Directory.CreateDirectory(folder);
        var filename = Path.Combine(folder, $@"Rule_{Config.GetRuleAsString().Replace('/', ',')}_{DateTime.Now:dd.MM.yy_HH.mm.ss}.png");
        RenderFrame.Instance.gol.MakeScreenshot(filename);
      }
      catch { }
    }

    string path = null;
    public void LoadFile()
    {
      try
      {
        var thread = new Thread(loadFile);
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (path == null) return;

        var d = RenderFrame.Instance.device;
        var tex = Texture2D.FromFile(d, path);
        RenderFrame.Instance.gol.LoadTexture(tex);
        tex.Dispose();
      }
      catch { }
    }

    private void loadFile()
    {
      try
      {
        var fod = new OpenFileDialog();
        var result = fod.ShowDialog();
        if (result != DialogResult.OK) { path = null; return; };
        path = fod.FileName;
      }
      catch { path = null; }
    }

    internal void Dispose()
    {
      maximize?.Dispose();
      maximizeString?.Dispose();
      pm.Dispose();
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
