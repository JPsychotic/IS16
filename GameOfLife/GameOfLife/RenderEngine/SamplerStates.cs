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
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
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
    }
}
