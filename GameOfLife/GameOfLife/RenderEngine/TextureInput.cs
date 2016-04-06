using GameOfLife.RenderEngine;
using GameOfLife.Storage;
using SlimDX;
using SlimDX.Direct3D11;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
    }

    public void RenderInput(Device device, DeviceContext context, RenderTargetView renderTarget)
    {
      if (LinesTodo.Count == 0) return;

      context.VertexShader.Set(inputVS);
      context.PixelShader.Set(inputPS);

      DataBox databoxPS = context.MapSubresource(miscInfoBuffer, 0, MapMode.WriteDiscard, 0);
      databoxPS.Data.Position = 0;
      databoxPS.Data.Write(new Vector4(Config.Width, Config.Height, Config.LineThickness, 0));
      context.UnmapSubresource(miscInfoBuffer, 0);

      DataBox databoxPS2 = context.MapSubresource(strokeInfoBuffer, 0, MapMode.WriteDiscard, 0);
      databoxPS2.Data.Position = 0;
      List<Vector4> Colors = new List<Vector4>();
      for (int i = 0; i < MaxStrokesPerFrame; i++)
      {
        var vec4 = new Vector4(-1,-1,-1,-1);
        Colors.Add(SelectedColor.ToVector4());
        if (LinesTodo.Count > 0)
        {
          var line = LinesTodo.Dequeue();
          vec4 = new Vector4(line.oldLocation.X, line.oldLocation.Y, line.newLocation.X, line.newLocation.Y);
          if(line.DeleteMode)
            Colors[i] = DeleteColor.ToVector4();
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
      if (a.Button.HasFlag(MouseButtons.Left))
      {
        LinesTodo.Enqueue(new StrokeInfo(oldMousePos, a.Location, false));
      }
      else if (a.Button.HasFlag(MouseButtons.Right))
      {
        LinesTodo.Enqueue(new StrokeInfo(oldMousePos, a.Location, true));
      }
      oldMousePos = a.Location;
    }
  }

  class StrokeInfo
  {
    public Point oldLocation, newLocation;
    public bool DeleteMode;
    public StrokeInfo(Point loc, Point delta, bool delete)
    {
      DeleteMode = delete;
      oldLocation = loc;
      newLocation = delta;
    }
  }
}