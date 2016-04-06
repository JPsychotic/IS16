using System;
using System.Drawing;
using System.Windows.Forms;
using GameOfLife.Storage;
using GameOfLife.UI;
using SlimDX;

namespace GameOfLife.RenderEngine.UI.Elements
{
    class Textbox2D
    {
        #region AlternativeConstructors

        public Textbox2D(Vector2 pos, Color col)
            : this((int)pos.X, (int)pos.Y, col)
        { }

        public Textbox2D(Vector2 pos, Vector2 size, Color col, string txt = "")
            : this((int)pos.X, (int)pos.Y, col, (int)size.X, (int)size.Y, txt)
        { }

        public Textbox2D(Vector2 pos, int sizeX, int sizeY, Color col, string txt = "")
            : this((int)pos.X, (int)pos.Y, col, sizeX, sizeY, txt)
        { }

        public Textbox2D(int x, int y, Vector2 size, Color col, string txt = "")
            : this(x, y, col, (int)size.X, (int)size.Y, txt)
        { }

        #endregion AlternativeConstructors

        string txt;
        public string Text
        {
            get { return txt; }
            set
            {
                txt = value;
                drawString = new DrawableString(value, new Vector2(Position.X + Spacer, Position.Y + Spacer), Color.White);
                if (TextChanged != null)
                {
                    TextChanged(this, new StringEventArgs(txt));
                }
            }
        }

        DrawableString drawString;
        Rectangle2D rect;
        public event EventHandler<StringEventArgs> TextChanged;
        bool Focused;
        private int MaxLength = 20;
        Vector2 Position, Size;
        public static int Spacer = 5;

        public Textbox2D(int x, int y, Color col, int sizeX = 180, int sizeY = 20, string txt = "")
        {
            rect = new Rectangle2D(x, y, sizeX, sizeY, col);
            Text = txt;
            Position = new Vector2(x, y);
            Size = new Vector2(sizeX, sizeY);
            MaxLength = (int)((sizeX - 2f * Spacer) / DrawableString.FontWidth);
            RenderFrame.Instance.RenderForm.KeyPress += KeyPressedHandler;
            RenderFrame.Instance.RenderForm.MouseClick += RenderForm_MouseClick;
        }

        void RenderForm_MouseClick(object sender, MouseEventArgs e)
        {
            Focused = rect.BoundingBox.Contains(e.Location);
        }

        void KeyPressedHandler(object sender, KeyPressEventArgs e)
        {
            if (!Focused) { return; }
            if (e.KeyChar == (char)Keys.Back)
            {
                if (Text.Length > 0)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
            }
            else if (Text.Length < MaxLength)
            {
                Text += e.KeyChar;
            }
        }
      
        public void Update()
        {
            rect.Update();
        }

        public void Draw(SpriteBatch sb)
        {
            sb.DrawString(drawString);
            rect.Draw(sb);
        }

        public void Dispose()
        {
            rect.Dispose();
        }
    }

    class StringEventArgs : EventArgs
    {
        public string Text;
        public StringEventArgs(string txt)
        {
            Text = txt;
        }
    }
}
