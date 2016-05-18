using SlimDX.Direct3D11;

namespace GameOfLife.RenderEngine
{
  class SamplerStates
  {
    public SamplerState LinSampler;
    public SamplerState PointSampler;

    public static SamplerStates Instance = new SamplerStates();

    public SamplerStates()
    {
      var samplerDesc = new SamplerDescription
      {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = Filter.MinMagMipLinear,
        MinimumLod = 0,
        MaximumLod = 0
      };

      LinSampler = SamplerState.FromDescription(RenderFrame.Instance.device, samplerDesc);

      samplerDesc.Filter = Filter.MinMagPointMipLinear;
      PointSampler = SamplerState.FromDescription(RenderFrame.Instance.device, samplerDesc);
    }

    public void Dispose()
    {
      LinSampler.Dispose();
      PointSampler.Dispose();
    }

    bool wrap = true;
    public void SwitchWrapMode()
    {
      wrap = !wrap;
      var samplerDesc = new SamplerDescription
      {
        AddressU = wrap ? TextureAddressMode.Wrap : TextureAddressMode.Clamp,
        AddressV = wrap ? TextureAddressMode.Wrap : TextureAddressMode.Clamp,
        AddressW = wrap ? TextureAddressMode.Wrap : TextureAddressMode.Clamp,
        Filter = Filter.MinMagMipLinear,
        MinimumLod = 0,
        MaximumLod = 0
      };

      LinSampler = SamplerState.FromDescription(RenderFrame.Instance.device, samplerDesc);

      samplerDesc.Filter = Filter.MinMagPointMipLinear;
      PointSampler = SamplerState.FromDescription(RenderFrame.Instance.device, samplerDesc);

    }
  }
}
