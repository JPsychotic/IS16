using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;
using Resource = SlimDX.Direct3D11.Resource;
using GameOfLife.RenderEngine;
using GameOfLife.Storage;
using System.Threading;
using System.Linq;
using GameOfLife.TouchInput;
using System.Collections.Generic;
using GameOfLife.RenderEngine.UI;

namespace GameOfLife
{
  public class RenderFrame
  {
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceCounter(out long lPerformanceCount);
    [DllImport("Kernel32.dll")]
    private static extern bool QueryPerformanceFrequency(out long lFrequency);

    long tickPrev, freq;
    public Device device { get; private set; }
    public DeviceContext deviceContext { get; private set; }
    public TouchForm RenderForm { get; set; }
    public SwapChain swapChain;
    public RenderTargetView MainRenderTarget;
    public DepthStencilView renderTargetDepthStencil;
    Texture2D DepthBuffer, swapchainresource;
    InputLayout layout;
    readonly GameOfLifeCalculator gol;
    readonly Userinterface UI;
    TextureInput inputHandler;

    Texture2DDescription ShaderInputTexDescription;
    Texture2D SzeneTexture;
    ShaderResourceView SzeneShaderRessource;
    SpriteBatch sb;

    public List<Finger> CurrentFingerCoordinates = new List<Finger>();

    public static RenderFrame Instance;

    public RenderFrame()
    {
      var bound = Screen.AllScreens[Config.DisplayScreen].Bounds;
      Config.Width = bound.Width;
      Config.Height = bound.Height;

      Point windowSize = new Point(Config.Width, Config.Height);

      Instance = this;
      RenderForm = new TouchForm("GameOfLife")
      {
        StartPosition = FormStartPosition.Manual,
        FormBorderStyle = FormBorderStyle.None,
        ClientSize = new Size(windowSize),
        Location = new Point(bound.X, bound.Y),
      };

      CreateDeviceSwapChainContext();
      Initialize();
      SetContextStates();
      OverrideEvents();

      sb = new SpriteBatch();
      inputHandler = new TextureInput(device);
      gol = new GameOfLifeCalculator();
      QueryPerformanceFrequency(out freq);
      QueryPerformanceCounter(out tickPrev);
      UI = new Userinterface(inputHandler);

      RenderForm.MouseMove += RenderForm_MouseMove;
      RenderForm.MouseWheel += RenderForm_MouseWheel;
      RenderForm.MouseDown += OnMouseDown;
      RenderForm.MouseUp += RenderForm_MouseUp;

      RenderForm.Touchdown += OnTouchDownHandler;
      RenderForm.Touchup += OnTouchUpHandler;
      RenderForm.TouchMove += OnTouchMoveHandler;
    }

    bool MouseOnSideBar = false;
    private void RenderForm_MouseUp(object sender, MouseEventArgs e)
    {
      if (MouseOnSideBar && CurrentFingerCoordinates.Count == 0) UI.OnMouseClick(sender, e);
      MouseOnSideBar = false;
    }

    private void OnMouseDown(object sender, MouseEventArgs e)
    {
      if (UI.IsPointInUI(e.Location)) MouseOnSideBar = true;
      else inputHandler.RegisterMouseDown(sender, e);
    }

    private void OnTouchDownHandler(object sender, TouchEventArgs e)
    {
      if (CurrentFingerCoordinates.Any((s) => s.ID == e.ID)) throw new Exception();
      CurrentFingerCoordinates.Add(new Finger(e.ID, e.Location, UI.IsPointInUI(e.Location)));
    }

    private void OnTouchUpHandler(object sender, TouchEventArgs e)
    {
      CurrentFingerCoordinates.RemoveAll((s) => s.ID == e.ID);
      if (UI.IsPointInUI(e.Location) && CurrentFingerCoordinates.Count > 0) UI.OnMouseClick(null, new MouseEventArgs(MouseButtons.Left, 1, e.Location.X, e.Location.Y, 0));
    }

    private void OnTouchMoveHandler(object sender, TouchEventArgs e)
    {
      var stroke = CurrentFingerCoordinates.First(s => s.ID == e.ID);
      if (stroke.Disabled) return;
      inputHandler.RegisterInput(stroke.Location, e.Location);
      stroke.Location = e.Location;
    }

    private void RenderForm_MouseMove(object sender, MouseEventArgs e)
    {
      if (CurrentFingerCoordinates.Count == 0 && !MouseOnSideBar)
        inputHandler.RegisterInput(sender, e);
    }

    private void RenderForm_MouseWheel(object sender, MouseEventArgs e)
    {
      var incremet = 1;
      if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) incremet = 10;
      if (e.Delta < 0)
      {
        Config.LineThickness -= incremet;
        if (Config.LineThickness < 1) Config.LineThickness = 1;
      }
      else
      {
        Config.LineThickness += incremet;
      }
    }

    public void Update(float elapsed)
    {
      UI.Update(elapsed);
    }

    public void Render(float elapsed)
    {
      //deviceContext.ClearDepthStencilView(renderTargetDepthStencil, DepthStencilClearFlags.Depth, 1, 0);
      deviceContext.OutputMerger.DepthStencilState = States.Instance.depthDisabledStencilDisabledWriteDisabled;
      deviceContext.OutputMerger.BlendState = States.Instance.blendDisabled;
      deviceContext.InputAssembler.InputLayout = layout;

      inputHandler.RenderInput(device, deviceContext, gol.OffscreenRenderTarget);

      gol.Draw(MainRenderTarget);

      sb.Begin(MainRenderTarget);
      UI.Draw(sb);
      sb.End();

      swapChain.Present(Config.Vsync, PresentFlags.None);
      Thread.Sleep(Config.Delay);
    }

    public float ElapsedTime()
    {
      long tickNow;
      QueryPerformanceCounter(out tickNow);
      float elapsed = (float)(tickNow - tickPrev) / freq;
      tickPrev = tickNow;
      return elapsed;
    }

    private void CreateDeviceSwapChainContext()
    {
      var description = new SwapChainDescription
      {
        BufferCount = 1,
        Usage = Usage.RenderTargetOutput,
        OutputHandle = RenderForm.Handle,
        IsWindowed = true,
        ModeDescription = new ModeDescription(0, 0, new Rational(60, 1), Format.R8G8B8A8_UNorm),
        SampleDescription = new SampleDescription(1, 0),
        Flags = SwapChainFlags.AllowModeSwitch,
        SwapEffect = SwapEffect.Discard,

      };
      Device dev;
      Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, description, out dev, out swapChain);
      device = dev;

      deviceContext = device.ImmediateContext;

      swapchainresource = Resource.FromSwapChain<Texture2D>(swapChain, 0);
      MainRenderTarget = new RenderTargetView(device, swapchainresource);

      Texture2DDescription depthBufferDesc = new Texture2DDescription
      {
        ArraySize = 1,
        BindFlags = BindFlags.DepthStencil,
        CpuAccessFlags = CpuAccessFlags.None,
        Format = Format.R32_Typeless,
        Height = RenderForm.ClientSize.Height,
        Width = RenderForm.ClientSize.Width,
        MipLevels = 1,
        OptionFlags = ResourceOptionFlags.None,
        SampleDescription = new SampleDescription(1, 0),
        Usage = ResourceUsage.Default
      };

      DepthBuffer = new Texture2D(device, depthBufferDesc);

      DepthStencilViewDescription dsViewDesc = new DepthStencilViewDescription
      {
        ArraySize = 1,
        Format = Format.D32_Float,
        Dimension = DepthStencilViewDimension.Texture2D,
        MipSlice = 0,
        Flags = 0,
        FirstArraySlice = 0
      };

      renderTargetDepthStencil = new DepthStencilView(device, DepthBuffer, dsViewDesc);
    }

    private void Initialize()
    {
      ShaderInputTexDescription = new Texture2DDescription
      {
        ArraySize = 1,
        Width = Config.Width,
        Height = Config.Height,
        BindFlags = BindFlags.RenderTarget,
        CpuAccessFlags = CpuAccessFlags.None,
        Format = Format.R8G8B8A8_UNorm,
        Usage = ResourceUsage.Default,
        MipLevels = 1,
        SampleDescription = new SampleDescription(Config.MSAA_SampleCount, Config.MSAA_Quality)
      };

      ShaderInputTexDescription.SampleDescription = new SampleDescription(1, 0);
      ShaderInputTexDescription.BindFlags = BindFlags.ShaderResource;

      SzeneTexture = new Texture2D(device, ShaderInputTexDescription);
      SzeneShaderRessource = new ShaderResourceView(device, SzeneTexture);

      deviceContext.Rasterizer.State = States.Instance.cullBackFillSolid;
    }

    private void SetContextStates()
    {
      InputElement[] inputElements =
      {
           new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
           new InputElement("NORMAL", 0, Format.R32G32B32A32_Float, 16, 0)
        };

      layout = new InputLayout(device, ShaderProvider.GetSignatureFromShader("Shader/GoL.fx"), inputElements);

      deviceContext.InputAssembler.InputLayout = layout;
      deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
      deviceContext.OutputMerger.BlendState = States.Instance.blendDisabled;
      deviceContext.OutputMerger.DepthStencilState = States.Instance.depthEnabledStencilDisabledWriteEnabled;
      deviceContext.Rasterizer.SetViewports(new Viewport(0, 0, RenderForm.ClientSize.Width, RenderForm.ClientSize.Height));
    }

    private void OverrideEvents()
    {
      RenderForm.KeyUp += (o, e) =>
      {
        var incremet = 1;
        if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) incremet = 10;
        if (e.KeyCode == Keys.Escape)
        {
          Exit();
        }
        else if (e.KeyCode == Keys.Down)
        {
          Config.Delay = Config.Delay << 1;
          if (Config.Delay == 0) Config.Delay = 1;
        }
        else if (e.KeyCode == Keys.Up)
        {
          Config.Delay = Config.Delay >> 1;
        }
        else if (e.KeyCode == Keys.Left)
        {
          Config.LineThickness -= incremet;
          if (Config.LineThickness < 1) Config.LineThickness = 1;
        }
        else if (e.KeyCode == Keys.Right)
        {
          Config.LineThickness += incremet;
        }
        else if (e.KeyCode == Keys.P)
        {
          Config.Paused = !Config.Paused;
        }
        else if (e.KeyCode == Keys.F)
        {
          Config.ShowFPS = !Config.ShowFPS;
        }
        else if (e.KeyCode == Keys.H)
        {
          Config.DisplayHelp = !Config.DisplayHelp;
        }
        else if (e.KeyCode == Keys.V)
        {
          Config.Vsync = Config.Vsync == 0 ? 1 : 0;
        }
        else if (e.KeyCode == Keys.C)
        {
          inputHandler.ClearWorld();
        }
      };
    }

    public void Exit()
    {
      swapchainresource.Dispose();
      sb?.Dispose();
      SzeneTexture?.Dispose();
      SzeneShaderRessource?.Dispose();
      gol?.Dispose();
      layout?.Dispose();
      inputHandler?.Dispose();
      UI?.Dispose();
      DepthBuffer?.Dispose();
      SamplerStates.Instance.Dispose();
      States.Instance.Dispose();
      swapChain?.Dispose();
      device?.Dispose();
      renderTargetDepthStencil?.Dispose();
      MainRenderTarget?.Dispose();
      RenderForm.Close();
      Application.Exit();
    }
  }
}
