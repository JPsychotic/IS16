using System.Drawing;

namespace GameOfLife
{
  public class Finger
  {
    public Finger(int id, Point loc, bool disabled = false)
    {
      Disabled = disabled;
      ID = id;
      Location = loc;
    }
    public bool Disabled;
    public int ID;
    public Point Location;
  }
}
