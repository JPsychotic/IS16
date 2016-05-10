using System;
using System.Drawing;

namespace GameOfLife.TouchInput
{
  public class TouchEventArgs : EventArgs
  {
    private const int TOUCHEVENTF_PRIMARY = 0x0010;
    public Point Location;
    public int ID;
    public int Flags;

    public bool IsPrimaryContact => (Flags & TOUCHEVENTF_PRIMARY) != 0;
  }

}
