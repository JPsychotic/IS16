using SlimDX.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GameOfLife.TouchInput
{
  public class TouchForm : RenderForm
  {
    private int touchInputSize;

    public event EventHandler<TouchEventArgs> Touchdown;
    public event EventHandler<TouchEventArgs> Touchup;
    public event EventHandler<TouchEventArgs> TouchMove;
    
    public TouchForm(string text)
    {
      Text = text;
      if (!RegisterTouchWindow(Handle, 0))
      {
        throw new Exception("Cant register touch input");
      }
      touchInputSize = Marshal.SizeOf(new TOUCHINPUT());
    }

    #region Touch Interop

    private const int WM_TOUCH = 0x0240;
    private const int TOUCHEVENTF_MOVE = 0x0001;
    private const int TOUCHEVENTF_DOWN = 0x0002;
    private const int TOUCHEVENTF_UP = 0x0004;

    [StructLayout(LayoutKind.Sequential)]
    private struct TOUCHINPUT
    {
      public int x;
      public int y;
      public IntPtr hSource;
      public int dwID;
      public int dwFlags;
      public int dwMask;
      public int dwTime;
      public IntPtr dwExtraInfo;
      public int cxContact;
      public int cyContact;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINTS
    {
      public short x;
      public short y;
    }

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool RegisterTouchWindow(IntPtr hWnd, ulong ulFlags);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [In, Out] TOUCHINPUT[] pInputs, int cbSize);

    [DllImport("user32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern void CloseTouchInputHandle(IntPtr lParam);

    protected override void WndProc(ref Message m)
    {
      bool handled;
      if (m.Msg == WM_TOUCH)
      {
        handled = DecodeTouch(ref m);
        if (handled) m.Result = new IntPtr(1);
      }
      base.WndProc(ref m);
    }

    private bool DecodeTouch(ref Message m)
    {
      int touchInputCount = m.WParam.ToInt32() & 0xffff;

      TOUCHINPUT[] inputs = new TOUCHINPUT[touchInputCount];
      if (!GetTouchInputInfo(m.LParam, touchInputCount, inputs, touchInputSize)) { return false; }

      bool handled = false;
      for (int i = 0; i < touchInputCount; i++)
      {
        var tochInput = inputs[i];
        EventHandler<TouchEventArgs> handler = null;
        if ((tochInput.dwFlags & TOUCHEVENTF_DOWN) != 0)
        {
          handler = Touchdown;
        }
        else if ((tochInput.dwFlags & TOUCHEVENTF_UP) != 0)
        {
          handler = Touchup;
        }
        else if ((tochInput.dwFlags & TOUCHEVENTF_MOVE) != 0)
        {
          handler = TouchMove;
        }

        if (handler != null)
        {
          // TOUCHINFO point coordinates size is in 1/100 of a pixel
          // convert screen to client coordinates.
          var te = new TouchEventArgs();
          te.Location = PointToClient(new Point(tochInput.x / 100, tochInput.y / 100));
          te.ID = tochInput.dwID;
          te.Flags = tochInput.dwFlags;
          handler(this, te);
          handled = true;
        }
      }
      CloseTouchInputHandle(m.LParam);
      return handled;
    }

    #endregion
  }
}
