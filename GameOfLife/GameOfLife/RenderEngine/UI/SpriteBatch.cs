using System;
using System.Collections.Generic;
using GameOfLife.RenderEngine.UI.Elements;
using SlimDX.Direct3D11;
using SlimDX.DXGI;

namespace GameOfLife.RenderEngine.UI
{
  class SpriteBatch
  {
    readonly Texture2D tex;
    readonly ShaderResourceView res;
    VertexShader vs, textVS;
    PixelShader ps, textPS;
    Queue<IDrawable2DElement> spriteQueue = new Queue<IDrawable2DElement>();
    Queue<DrawableString> textQueue = new Queue<DrawableString>();
    private bool active = false;
    RenderTargetView target;
    InputLayout layoutSprite, layoutFont;

    public SpriteBatch()
    {
      ps = ShaderProvider.CompilePS("Shader/Sprite2D.fx");
      vs = ShaderProvider.CompileVS("Shader/Sprite2D.fx");

      InputElement[] inputElements =
      {
           new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
           new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
      };
      layoutSprite = new InputLayout(RenderFrame.Instance.device, ShaderProvider.GetSignatureFromShader("Shader/Sprite2D.fx"), inputElements);

      tex = Texture2D.FromFile(RenderFrame.Instance.device, @".\Content\Font.png");
      res = new ShaderResourceView(RenderFrame.Instance.device, tex);
      textVS = ShaderProvider.CompileVS("Shader/Font.fx");
      textPS = ShaderProvider.CompilePS("Shader/Font.fx");
      layoutFont = new InputLayout(RenderFrame.Instance.device, ShaderProvider.GetSignatureFromShader("Shader/Font.fx"), inputElements);

    }

    public void Draw(IDrawable2DElement e)
    {
      if (!active) throw new InvalidOperationException("You should call Begin() before ending a batch.");
      spriteQueue.Enqueue(e);
    }

    public void Draw(List<IDrawable2DElement> e)
    {
      if (!active) throw new InvalidOperationException("You should call Begin() before ending a batch.");
      foreach(var s in e) spriteQueue.Enqueue(s);
    }

    public void DrawString(DrawableString s)
    {
      if (!active) throw new InvalidOperationException("You should call Begin() before ending a batch.");
      textQueue.Enqueue(s);
    }

    internal void DrawString(List<DrawableString> strings)
    {
      if (!active) throw new InvalidOperationException("You should call Begin() before ending a batch.");
      foreach(var s in strings) textQueue.Enqueue(s);
    }

    public void Begin(RenderTargetView rt)
    {
      target = rt;
      active = true;
    }

    public void End()
    {
      if (!active) throw new InvalidOperationException("You should call Begin() before ending a batch.");
      active = false;

      var c = RenderFrame.Instance.deviceContext;

      c.PixelShader.Set(ps);
      c.VertexShader.Set(vs);
      c.OutputMerger.SetTargets(target);
      c.InputAssembler.InputLayout = layoutSprite;
      c.OutputMerger.BlendState = States.Instance.blendEnabledAlphaBlending;
      c.PixelShader.SetSampler(SamplerStates.Instance.PointSampler, 1);

      while (spriteQueue.Count > 0)
      {
        var sprite = spriteQueue.Dequeue();
        sprite.Draw();
      }

      c.VertexShader.Set(textVS);
      c.PixelShader.Set(textPS);
      c.InputAssembler.InputLayout = layoutFont;
      c.PixelShader.SetShaderResource(res, 0);

      while (textQueue.Count > 0)
      {
        var text = textQueue.Dequeue();
        text.Draw();
      }
    }

    public void Dispose()
    {
      res.Dispose();
      tex.Dispose();
      textPS.Dispose();
      textVS.Dispose();
      spriteQueue.Clear();
      textQueue.Clear();
      ps.Dispose();
      vs.Dispose();
      layoutFont.Dispose();
      layoutSprite.Dispose();
    }
  }
}
