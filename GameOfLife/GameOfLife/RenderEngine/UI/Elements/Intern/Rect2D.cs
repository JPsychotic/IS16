using System.Collections.Generic;
using System.Drawing;
using GameOfLife.Storage;
using SlimDX;
using SlimDX.Direct3D11;

namespace GameOfLife.RenderEngine.UI.Elements.Intern
{
  class Rect2D
  {
    #region AlternativeConstructors

    public Rect2D(Vector2 pos, Vector2 size, Color col)
        : this((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, col)
    { }

    public Rect2D(Vector2 pos, int sizeX, int sizeY, Color col)
        : this((int)pos.X, (int)pos.Y, sizeX, sizeY, col)
    { }

    public Rect2D(int x, int y, Vector2 size, Color col)
        : this(x, y, (int)size.X, (int)size.Y, col)
    { }

    public Rect2D(int x, int y, int sizeX, int sizeY, Texture2D tex) : this(x, y, sizeX, sizeY, new Color4(1, 1, 0, 0))
    {
      texture = tex;
      texView = new ShaderResourceView(RenderFrame.Instance.device, tex);

      Update();
    }

    #endregion AlternativeConstructors

    internal static float width = Config.Width;
    internal static float height = Config.Height;
    internal static List<short> indices = new List<short> { 1, 3, 2, 1, 0, 3 };

    static Mesh mesh = CreateMesh();
    private int X, Y, SizeY, SizeX;
    internal Color4 Color;
    internal RectangleF BoundingBox;
    internal Texture2D texture;
    internal ShaderResourceView texView;
    internal Buffer Cbuffer;

    public Rect2D(int x, int y, int sizeX, int sizeY, Color4 col)
    {
      X = x;
      Y = Config.Height - y; // nullpunkt muss korrigiert werden weil er bei DirectX unten links und nich oben links ist.
      SizeX = sizeX;
      SizeY = -sizeY;
      Color = col;

      var d = RenderFrame.Instance.device;
      Cbuffer = new Buffer(d, Vector4.SizeInBytes * 3, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

      Update();
    }

    static Mesh CreateMesh()
    {
      List<Vertex> vertices = new List<Vertex>
      {
        new Vertex(new Vector4(1, 1, 1, 0), new Vector3(1f / Config.Width, 1f / Config.Height, 0)),
        new Vertex(new Vector4(0, 1, 0, 0), new Vector3(1f / Config.Width, 1f / Config.Height, 0)),
        new Vertex(new Vector4(0, 0, 0, 1), new Vector3(1f / Config.Width, 1f / Config.Height, 0)),
        new Vertex(new Vector4(1, 0, 1, 1), new Vector3(1f / Config.Width, 1f / Config.Height, 0))
      };

      return new Mesh(vertices, indices);
    }

    public void Update()
    {
      var c = RenderFrame.Instance.deviceContext;
      BoundingBox = new RectangleF(X, Config.Height - Y, SizeX, -SizeY);

      float texAvailable = texView != null ? 2 : 0;
      DataBox databox = c.MapSubresource(Cbuffer, 0, MapMode.WriteDiscard, 0);
      databox.Data.Position = 0;
      databox.Data.Write(new Vector4(X / width * 2 - 1, Y / height * 2 - 1, SizeX / width, SizeY / height));
      databox.Data.Write(new Vector4(texAvailable, texAvailable, texAvailable, texAvailable));
      databox.Data.Write(Color.ToVector4());
      c.UnmapSubresource(Cbuffer, 0);
    }

    public void Draw()
    {
      var c = RenderFrame.Instance.deviceContext;
      if (texView != null) c.PixelShader.SetShaderResource(texView, 0);
      c.VertexShader.SetConstantBuffer(Cbuffer, 2);
      c.PixelShader.SetConstantBuffer(Cbuffer, 2);
      mesh.Render();
    }

    public void SetSize(int sizeX, int sizeY)
    {
      SizeX = sizeX;
      SizeY = -sizeY;
      Update();
    }

    public void SetPosition(int x, int y)
    {
      X = x;
      Y = Config.Height - y;
      Update();
    }

    public void SetPositionAndSize(int x, int y, int sizeX, int sizeY)
    {
      X = x;
      Y = Config.Height - y;
      SizeX = sizeX;
      SizeY = -sizeY;
      Update();
    }
    
    public void Dispose()
    {
      //mesh.Dispose();
    }
  }
}
