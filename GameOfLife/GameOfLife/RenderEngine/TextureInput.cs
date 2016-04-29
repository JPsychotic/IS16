using GameOfLife.RenderEngine;
using GameOfLife.Storage;
using SlimDX;
using SlimDX.Direct3D11;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Buffer = SlimDX.Direct3D11.Buffer;
using System;

namespace GameOfLife
{
  class TextureInput
  {
    Queue<StrokeInfo> LinesTodo = new Queue<StrokeInfo>();
    Bitmap bitmap = new Bitmap(Config.Width, Config.Height);
    VertexShader inputVS;
    PixelShader inputPS;
    Buffer strokeInfoBuffer, miscInfoBuffer;
    Mesh quad;
    int MaxStrokesPerFrame = 10;
    public Color4 SelectedColor = new Color4(1, 1, 1, 1);
    Color4 DeleteColor = new Color4(1, 0, 0, 0);

    public TextureInput(Device d)
    {
      inputVS = ShaderProvider.CompileVS("./Shader/TextureInput.fx");
      inputPS = ShaderProvider.CompilePS("./Shader/TextureInput.fx");
      strokeInfoBuffer = new Buffer(d, Vector4.SizeInBytes * MaxStrokesPerFrame * 2, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
      miscInfoBuffer = new Buffer(d, Vector4.SizeInBytes, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
      quad = Mesh.ScreenQuad();

      DataBox databoxPS = d.ImmediateContext.MapSubresource(miscInfoBuffer, 0, MapMode.WriteDiscard, 0);
      databoxPS.Data.Position = 0;
      databoxPS.Data.Write(new Vector4(Config.Width, Config.Height, 0, 0));
      d.ImmediateContext.UnmapSubresource(miscInfoBuffer, 0);
    }

    public void RenderInput(Device device, DeviceContext context, RenderTargetView renderTarget)
    {
      if (LinesTodo.Count == 0) return;

      context.VertexShader.Set(inputVS);
      context.PixelShader.Set(inputPS);

      DataBox databoxPS2 = context.MapSubresource(strokeInfoBuffer, 0, MapMode.WriteDiscard, 0);
      databoxPS2.Data.Position = 0;
      List<Vector4> Colors = new List<Vector4>();
      for (int i = 0; i < MaxStrokesPerFrame; i++)
      {
        var vec4 = new Vector4(-1, -1, -1, -1);
        Colors.Add(SelectedColor.ToVector4());
        if (LinesTodo.Count > 0)
        {
          var line = LinesTodo.Dequeue();
          vec4 = new Vector4(line.oldLocation.X, line.oldLocation.Y, line.newLocation.X, line.newLocation.Y);
          if (line.DeleteMode) { Colors[i] = DeleteColor.ToVector4(); }
          var col = Colors[i];
          col.W = line.Width;
          Colors[i] = col;
        }
        databoxPS2.Data.Write(vec4);
      }
      foreach (var c in Colors) databoxPS2.Data.Write(c);
      context.UnmapSubresource(strokeInfoBuffer, 0);

      context.PixelShader.SetConstantBuffer(miscInfoBuffer, 2);
      context.PixelShader.SetConstantBuffer(strokeInfoBuffer, 3);
      context.OutputMerger.SetTargets(renderTarget);
      quad.Render();
    }

    Point oldMousePos = new Point();
    internal void RegisterInput(object sender, MouseEventArgs a)
    {
      if (a.Button == MouseButtons.None) return;
      LinesTodo.Enqueue(new StrokeInfo(oldMousePos, a.Location, a.Button.HasFlag(MouseButtons.Right), Config.LineThickness));
      oldMousePos = a.Location;
    }

    internal void RegisterMouseDown(object sender, MouseEventArgs e)
    {
      LinesTodo.Enqueue(new StrokeInfo(e.Location, e.Location, false, Config.LineThickness));
      oldMousePos = e.Location;
    }

    internal void RegisterInput(Point start, Point end)
    {
      LinesTodo.Enqueue(new StrokeInfo(start, end, false, Config.LineThickness));
      oldMousePos = end;
    }

    internal void ClearWorld()
    {
      LinesTodo.Enqueue(new StrokeInfo(new Point(0, 0), new Point(Config.Width, 0), true, Config.Height));
      LinesTodo.Enqueue(new StrokeInfo(new Point(Config.Width, 0), new Point(Config.Width, Config.Height), true, Config.Width));
      LinesTodo.Enqueue(new StrokeInfo(new Point(Config.Width, Config.Height), new Point(0, Config.Height), true, Config.Width));
      LinesTodo.Enqueue(new StrokeInfo(new Point(0, Config.Height), new Point(0, 0), true, Config.Height));
    }

    public void Dispose()
    {
      inputPS.Dispose();
      inputVS.Dispose();
      miscInfoBuffer.Dispose();
      strokeInfoBuffer.Dispose();
      quad.Dispose();
      bitmap.Dispose();
    }
  }

  class StrokeInfo
  {
    public Point oldLocation, newLocation;
    public bool DeleteMode;
    public int Width;
    public StrokeInfo(Point loc, Point delta, bool delete, int width)
    {
      Width = width;
      DeleteMode = delete;
      oldLocation = loc;
      newLocation = delta;
    }
  }
}