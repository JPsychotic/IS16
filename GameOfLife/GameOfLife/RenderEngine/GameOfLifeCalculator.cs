using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.Remoting.Contexts;
using GameOfLife.RenderEngine.UI.Elements;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using GameOfLife.Storage;
using SlimDX;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace GameOfLife.RenderEngine
{
  class GameOfLifeCalculator
  {
    readonly Texture2D ScreenBufferShaderResourceTexture, OffscreenRenderTargetTexture, randomTex;
    public readonly ShaderResourceView ScreenBufferShaderResource, randomTexView;
    public readonly RenderTargetView OffscreenRenderTarget;
    readonly VertexShader GoLVS;
    readonly PixelShader GoLPS;
    readonly Mesh quad;
    readonly Buffer CBuffer;
    Random rnd = new Random();

    public GameOfLifeCalculator()
    {
      var d = RenderFrame.Instance.device;

      var ShaderInputTexDescription = new Texture2DDescription
      {
        ArraySize = 1,
        Width = Config.Width,
        Height = Config.Height,
        BindFlags = BindFlags.ShaderResource,
        CpuAccessFlags = CpuAccessFlags.None,
        Format = Format.R8G8B8A8_UNorm,
        Usage = ResourceUsage.Default,
        MipLevels = 1,
        SampleDescription = new SampleDescription(1, 0)
      };

      ScreenBufferShaderResourceTexture = new Texture2D(d, ShaderInputTexDescription);
      ScreenBufferShaderResource = new ShaderResourceView(d, ScreenBufferShaderResourceTexture);

      randomTex = Texture2D.FromFile(d, @".\Content\Noise.png");
      randomTexView = new ShaderResourceView(d, randomTex);

      ShaderInputTexDescription.BindFlags = BindFlags.RenderTarget;
      OffscreenRenderTargetTexture = new Texture2D(d, ShaderInputTexDescription);
      OffscreenRenderTarget = new RenderTargetView(d, OffscreenRenderTargetTexture);
      d.ImmediateContext.ClearRenderTargetView(OffscreenRenderTarget, Color.Black);

      GoLPS = ShaderProvider.CompilePS("./Shader/GoL.fx");
      GoLVS = ShaderProvider.CompileVS("./Shader/GoL.fx");

      CBuffer = new Buffer(d, Vector4.SizeInBytes * 2, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

      quad = Mesh.ScreenQuad();
    }

    internal void Draw(RenderTargetView mainRendertarget)
    {
      var c = RenderFrame.Instance.deviceContext;
      if (!Config.Paused)
      {
        DataBox databoxPS = c.MapSubresource(CBuffer, 0, MapMode.WriteDiscard, 0);
        databoxPS.Data.Position = 0;
        databoxPS.Data.Write(new Vector4(1 / (float)Config.Width, 1 / (float)Config.Height, (float)rnd.NextDouble(), (float)rnd.NextDouble()));
        databoxPS.Data.Write(Config.BirthRule);
        databoxPS.Data.Write(Config.DeathRule);
        databoxPS.Data.Write<uint>(0);
        databoxPS.Data.Write<uint>(0);
        c.UnmapSubresource(CBuffer, 0);

        c.CopyResource(OffscreenRenderTarget.Resource, ScreenBufferShaderResource.Resource);

        c.VertexShader.Set(GoLVS);
        c.PixelShader.Set(GoLPS);
        c.PixelShader.SetSampler(SamplerStates.Instance.LinSampler, 0);
        c.PixelShader.SetSampler(SamplerStates.Instance.PointSampler, 1);
        c.OutputMerger.SetTargets(OffscreenRenderTarget);
        c.PixelShader.SetShaderResource(ScreenBufferShaderResource, 0);
        c.PixelShader.SetShaderResource(randomTexView, 1);
        c.PixelShader.SetConstantBuffer(CBuffer, 2);

        quad.Render();
      }
      c.CopyResource(OffscreenRenderTarget.Resource, mainRendertarget.Resource);
    }

    public void Dispose()
    {
      ScreenBufferShaderResource.Dispose();
      ScreenBufferShaderResourceTexture.Dispose();
      OffscreenRenderTargetTexture.Dispose();
      OffscreenRenderTarget.Dispose();
      CBuffer.Dispose();
      GoLVS.Dispose();
      GoLPS.Dispose();
      quad.Dispose();
    }

    internal void MakeScreenshot(string filename)
    {
      try
      {
        Texture2D.ToFile(RenderFrame.Instance.deviceContext, OffscreenRenderTargetTexture, ImageFileFormat.Png, filename);
      }
      catch { }
    }

    public void LoadTexture(Texture2D tex)
    {
      var sb = RenderFrame.Instance.spriteBatch;
      var rect = new Rectangle2D(new Vector2(), Config.Width, Config.Height, tex);
      sb.Begin(OffscreenRenderTarget);
      sb.Draw(rect);
      sb.End();
      rect.Dispose();
    }
  }
}
