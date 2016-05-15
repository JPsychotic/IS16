using System.Drawing;
using GameOfLife.RenderEngine.UI.Sidebar;

namespace GameOfLife.RenderEngine.UI.Elements
{
  interface IDrawable2DElement
  {
    void Update();
    void Draw();
    void Dispose();
    void HandleInput(Point loc, SideBarState state);
    event ChangedEventHandler GotInput;
  }
  public delegate void ChangedEventHandler(SideBarState state);
}
