using SlimDX.Direct3D11;
using SlimDX.DXGI;
using GameOfLife.Storage;
using SlimDX;

namespace GameOfLife.RenderEngine
{
  class GameOfLifeCalculator
  {
    readonly Texture2D ScreenBufferShaderResourceTexture, OffscreenRenderTargetTexture;
    public readonly ShaderResourceView ScreenBufferShaderResource;
    public readonly RenderTargetView OffscreenRenderTarget;
    readonly VertexShader GoLVS;
    readonly PixelShader GoLPS;
    readonly Mesh quad;

    readonly Buffer CBuffer;
    internal ShaderResourceView InputShaderRessourceView;

    public GameOfLifeCalculator()
    {
      var d = RenderFrame.Instance.device;
      var c = d.ImmediateContext;

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

      ShaderInputTexDescription.BindFlags = BindFlags.RenderTarget;
      OffscreenRenderTargetTexture = new Texture2D(d, ShaderInputTexDescription);
      OffscreenRenderTarget = new RenderTargetView(d, OffscreenRenderTargetTexture);

      GoLPS = ShaderProvider.CompilePS("./Shader/GoL.fx");
      GoLVS = ShaderProvider.CompileVS("./Shader/GoL.fx");

      var ShaderOutputTexDescription = new Texture2DDescription
      {
        ArraySize = 1,
        Width = Config.Width,
        Height = Config.Height,
        BindFlags = BindFlags.RenderTarget,
        CpuAccessFlags = CpuAccessFlags.None,
        Format = Format.R8G8B8A8_UNorm,
        Usage = ResourceUsage.Default,
        MipLevels = 1,
        SampleDescription = new SampleDescription(1, 0)
      };

      CBuffer = new Buffer(d, Vector4.SizeInBytes * 2, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);


      DataBox databoxPS = c.MapSubresource(CBuffer, 0, MapMode.WriteDiscard, 0);
      databoxPS.Data.Position = 0;
      databoxPS.Data.Write(new Vector4(1 / (float)Config.Width, 1 / (float)Config.Height, 0, 0));
      databoxPS.Data.Write(new Vector4(0, 0, 0, 0));
      c.UnmapSubresource(CBuffer, 0);

      quad = Mesh.ScreenQuad();

    }

    internal void Draw(RenderTargetView mainRendertarget)
    {
      var c = RenderFrame.Instance.deviceContext;
      if (!Config.Paused)
      {
        c.CopyResource(OffscreenRenderTarget.Resource, ScreenBufferShaderResource.Resource);

        c.VertexShader.Set(GoLVS);
        c.PixelShader.Set(GoLPS);
        
        c.OutputMerger.SetTargets(OffscreenRenderTarget);
        c.PixelShader.SetShaderResource(ScreenBufferShaderResource, 0);
        c.PixelShader.SetShaderResource(InputShaderRessourceView, 1);
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
  }
}