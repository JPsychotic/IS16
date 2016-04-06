using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.RenderEngine.UI.Elements;
using GameOfLife.Storage;
using System.Drawing;
using GameOfLife.UI;
using SlimDX;

namespace GameOfLife.RenderEngine.UI
{
    class ConfigUI
    {
        int Spacer = 10, Width = 200;
        Vector2 Position;

        DrawableString title;
        Rectangle2D background;

        public ConfigUI()
        {
            Position = new Vector2(Config.Width - (Width + Spacer), Spacer);
            background = new Rectangle2D(Position, 200, Config.Height - 2 * Spacer, Color.FromArgb(200, 20, 20, 20));
            title = new DrawableString("Config Menu", Position + new Vector2(Width / 2 - DrawableString.Measure("Config Menu").X, Spacer), Color.White);
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch sb)
        {
            background.Draw(sb);
            sb.DrawString(title);
        }

        public void Dispose()
        {
            title.Dispose();
            background.Dispose();
        }
    }
}
