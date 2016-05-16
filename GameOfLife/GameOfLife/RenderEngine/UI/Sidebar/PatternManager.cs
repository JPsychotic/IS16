using System.Collections.Generic;
using System.Drawing;
using System.IO;
using GameOfLife.RenderEngine.UI.Elements;
using GameOfLife.Storage;
using SlimDX;

namespace GameOfLife.RenderEngine.UI.Sidebar
{
  class PatternManager
  {
    string BasePath = @".\Content\Muster\";
    List<Pattern> patterns = new List<Pattern>();
    List<Pattern> currentPatterns = new List<Pattern>();
    Vector2 Position, Size;
    DrawableString PageText, leftText, rightText;
    Rectangle2D left, right;
    Vector2 PatternSize;
    int PatternMarginX = 20;
    int PatternMarginY = 50;
    int Page = 0;
    string currentRules;
    List<Rectangle2D> patternToPlace = new List<Rectangle2D>();
    List<Rectangle2D> patternToDraw = new List<Rectangle2D>();

    public PatternManager(int x, int y, int width, int heigth, SideBar sb)
    {
      Position = new Vector2(x, y);
      Size = new Vector2(width, heigth);
      PatternSize = new Vector2(width / 2f - PatternMarginX * 2, heigth / 4.5f - PatternMarginY);

      foreach (var folder in Directory.GetDirectories(BasePath))
      {
        var world = Path.GetFileName(folder).Replace(',', '/');
        foreach (var file in Directory.GetFiles(folder))
        {
          var name = Path.GetFileNameWithoutExtension(file);
          patterns.Add(new Pattern(new Vector2(0, Position.Y * 1.5f), (int)PatternSize.X, (int)PatternSize.Y, file, world, name));
        }
      }
      PageText = new DrawableString("Welt: " + currentRules, new Vector2(Size.X * 0.1f, Position.Y + Size.Y - Size.Y * 0.075f), Color.White);

      Config.RulesChanged += Config_RulesChanged;

      var buttonsHeight = (int)(PageText.Location.Y - Size.X / 16f + DrawableString.FontHeight);
      left = new Rectangle2D(new Vector2(width * 0.5f, buttonsHeight), (int)(Size.X / 4f - 20), (int)(Size.X / 8f), Color.LightGray, (s) => { UpdatePatterns(-1); }, SideBarState.LeftTab);
      right = new Rectangle2D(new Vector2(width * 0.75f + 0, buttonsHeight), (int)(Size.X / 4f - 20), (int)(Size.X / 8f), Color.LightGray, (s) => { UpdatePatterns(1); }, SideBarState.LeftTab);

      sb.GotInputClick += left.HandleInput;
      sb.GotInputClick += right.HandleInput;

      var strSize = DrawableString.Measure("<", 4);
      leftText = new DrawableString("<", new Vector2(left.Location.X + left.Size.X / 2, left.Location.Y + left.Size.Y / 2) - strSize / 2, Color.Black, 4);
      rightText = new DrawableString(">", new Vector2(right.Location.X + right.Size.X / 2, right.Location.Y + right.Size.Y / 2) - strSize / 2, Color.Black, 4);

      Config_RulesChanged();
    }

    private void Config_RulesChanged()
    {
      Page = 0;
      currentRules = Config.GetRuleAsString();
      UpdatePatterns();
    }

    private void UpdatePatterns(int pageDiff = 0)
    {
      var possiblePatterns = patterns.FindAll(s => s.World == currentRules);

      Page += pageDiff;
      Page = Page < 0 ? 0 : Page > (int)(possiblePatterns.Count / 8f) ? (int)(possiblePatterns.Count / 8f) : Page;

      currentPatterns.Clear();
      for (int i = 0; i + Page * 8 < possiblePatterns.Count && i < 8; i++)
      {
        var p = possiblePatterns[i + Page * 8];
        int x = (i % 2) * (int)(PatternSize.X + PatternMarginX * 2) + PatternMarginX;
        int y = (int)(i / 2) * (int)(PatternSize.Y + PatternMarginY) + (int)Position.Y + PatternMarginY;
        p.SetLocation(x, y);
        currentPatterns.Add(p);
      }
      PageText.Dispose();

      var pageStrSize = DrawableString.Measure(Page.ToString(), 4);
      PageText = new DrawableString((1 + Page) + "/" + (int)(possiblePatterns.Count / 8f + 1), new Vector2(Size.X * 0.2f - pageStrSize.X / 2, left.Location.Y + left.Size.Y / 2 - pageStrSize.Y * 0.5f), Color.White, 4);
    }

    public void Draw(SpriteBatch sb)
    {
      foreach (var p in currentPatterns) p.Draw(sb);
      foreach (var p in patternToPlace) sb.Draw(p);

      sb.Draw(left);
      sb.Draw(right);
      sb.DrawString(PageText);
      sb.DrawString(leftText);
      sb.DrawString(rightText);
    }

    public void Dispose()
    {
      foreach (var p in patterns) p.Dispose();
      left.Dispose();
      right.Dispose();
      PageText.Dispose();
      leftText.Dispose();
      rightText.Dispose();
    }

    public bool Contains(Point loc)
    {
      foreach (var p in patternToPlace) if (p.BoundingBox.Contains(loc)) return true;
      return false;
    }

    Point oldmouseLoc = new Point();
    Point startMouseLoc = new Point();
    public void HandleMouseMove(Point loc)
    {
      var old = oldmouseLoc;
      oldmouseLoc = loc;
      foreach (var p in patternToPlace)
      {
        if (p.BoundingBox.Contains(old))
        {
          p.SetPosition((int)p.Location.X + loc.X - old.X, (int)p.Location.Y + loc.Y - old.Y);
          return;
        }
      }
      foreach (var p in currentPatterns)
      {
        if (p.rect.BoundingBox.Contains(old))
        {
          patternToPlace.Add(new Rectangle2D(loc.X, loc.Y, p.Texture.Description.Width, p.Texture.Description.Height, p.Texture));
        }
      }
    }

    public void HandleMouseDown(Point loc)
    {
      oldmouseLoc = loc;
      startMouseLoc = loc;
    }

    public void HandleMouseClick(Point loc, Color4 col)
    {
      if (loc != startMouseLoc) return;
      foreach (var p in patternToPlace)
      {
        if (p.BoundingBox.Contains(loc))
        {
          var sb = RenderFrame.Instance.spriteBatch;
          var rt = RenderFrame.Instance.gol.OffscreenRenderTarget;
          p.Color = col;

          sb.Begin(rt);
          sb.Draw(p);
          sb.End();

          patternToPlace.Remove(p);
          return;
        }
      }
    }
  }
}
