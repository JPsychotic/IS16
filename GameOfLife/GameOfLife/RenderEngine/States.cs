using SlimDX.Direct3D11;

namespace GameOfLife.RenderEngine
{
    class States
    {
        public DepthStencilState depthEnabledStencilDisabledWriteEnabled { get; }
        public DepthStencilState depthEnabledStencilDisabledWriteDisabled { get; }
        public DepthStencilState depthDisabledStencilDisabledWriteEnabled { get; }
        public DepthStencilState depthDisabledStencilDisabledWriteDisabled { get; private set; }
        public RasterizerState cullNoneFillSolid { get; }
        public RasterizerState cullNoneFillWireframe { get; }
        public RasterizerState cullBackFillSolid { get; }
        public RasterizerState cullBackFillWireframe { get; }
        public RasterizerState cullFrontFillSolid { get; }
        public RasterizerState cullFrontFillWireframe { get; }
        public BlendState blendDisabled { get; }
        public BlendState blendEnabledSourceAlphaInverseSourceAlpha { get; }
        public BlendState blendEnabledSourceAlphaDestinationAlpha { get; }
        public BlendState blendEnabledOneOne { get; }
        public BlendState blendEnabledAlphaBlending { get; }

        public static readonly States Instance = new States();

        public States()
        {
            var device = RenderFrame.Instance.device;
            DepthStencilStateDescription dsStateDesc = new DepthStencilStateDescription
            {
                IsDepthEnabled = false,
                IsStencilEnabled = false,
                DepthWriteMask = DepthWriteMask.Zero,
                DepthComparison = Comparison.Less,
            };

            depthDisabledStencilDisabledWriteDisabled = DepthStencilState.FromDescription(device, dsStateDesc);
            dsStateDesc.DepthWriteMask = DepthWriteMask.All;
            depthDisabledStencilDisabledWriteEnabled = DepthStencilState.FromDescription(device, dsStateDesc);
            dsStateDesc.IsDepthEnabled = true;
            depthEnabledStencilDisabledWriteEnabled = DepthStencilState.FromDescription(device, dsStateDesc);
            dsStateDesc.DepthWriteMask = DepthWriteMask.Zero;
            depthEnabledStencilDisabledWriteDisabled = DepthStencilState.FromDescription(device, dsStateDesc);

            RasterizerStateDescription rasStateDesc = new RasterizerStateDescription
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0.0f,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = false,
                IsFrontCounterclockwise = true,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false
            };
            cullNoneFillSolid = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.FillMode = FillMode.Wireframe;
            cullNoneFillWireframe = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.CullMode = CullMode.Back;
            cullBackFillWireframe = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.FillMode = FillMode.Solid;
            cullBackFillSolid = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.CullMode = CullMode.Front;
            cullFrontFillSolid = RasterizerState.FromDescription(device, rasStateDesc);
            rasStateDesc.FillMode = FillMode.Wireframe;
            cullFrontFillWireframe = RasterizerState.FromDescription(device, rasStateDesc);

            BlendStateDescription blendStateDesc = new BlendStateDescription
            {
                IndependentBlendEnable = false,
                AlphaToCoverageEnable = false,
            };

            blendStateDesc.RenderTargets[0] = new RenderTargetBlendDescription
            {
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            blendDisabled = BlendState.FromDescription(device, blendStateDesc);
            blendStateDesc.RenderTargets[0].BlendEnable = true;
            blendStateDesc.RenderTargets[0].BlendOperation = BlendOperation.Add;
            blendStateDesc.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            blendStateDesc.RenderTargets[0].DestinationBlend = BlendOption.DestinationAlpha;
            blendStateDesc.RenderTargets[0].DestinationBlendAlpha = BlendOption.DestinationAlpha;
            blendStateDesc.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            blendStateDesc.RenderTargets[0].SourceBlendAlpha = BlendOption.SourceAlpha;
            blendEnabledSourceAlphaDestinationAlpha = BlendState.FromDescription(device, blendStateDesc);

            blendStateDesc.RenderTargets[0].BlendOperationAlpha = BlendOperation.Maximum;
            blendStateDesc.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            blendStateDesc.RenderTargets[0].DestinationBlendAlpha = BlendOption.InverseSourceAlpha;
            blendEnabledSourceAlphaInverseSourceAlpha = BlendState.FromDescription(device, blendStateDesc);

            blendStateDesc.RenderTargets[0].DestinationBlend = BlendOption.One;
            blendStateDesc.RenderTargets[0].DestinationBlendAlpha = BlendOption.One;
            blendStateDesc.RenderTargets[0].SourceBlend = BlendOption.One;
            blendStateDesc.RenderTargets[0].SourceBlendAlpha = BlendOption.One;
            blendEnabledOneOne = BlendState.FromDescription(device, blendStateDesc);

            var desc = new BlendStateDescription
            {
                AlphaToCoverageEnable = false, 
                IndependentBlendEnable = false
            };

            desc.RenderTargets[0].BlendEnable = true;
            desc.RenderTargets[0].BlendOperation = BlendOperation.Add;
            desc.RenderTargets[0].SourceBlend = BlendOption.SourceAlpha;
            desc.RenderTargets[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            desc.RenderTargets[0].SourceBlendAlpha = BlendOption.Zero;
            desc.RenderTargets[0].DestinationBlendAlpha = BlendOption.Zero;
            desc.RenderTargets[0].BlendOperationAlpha = BlendOperation.Add;
            desc.RenderTargets[0].RenderTargetWriteMask = ColorWriteMaskFlags.All; 
            blendEnabledAlphaBlending = BlendState.FromDescription(device, desc);
        }
        
        public void Dispose()
        {
            if (!depthEnabledStencilDisabledWriteEnabled.Disposed) { depthEnabledStencilDisabledWriteEnabled.Dispose(); }
            if (!depthEnabledStencilDisabledWriteDisabled.Disposed) { depthEnabledStencilDisabledWriteDisabled.Dispose(); }
            if (!depthDisabledStencilDisabledWriteEnabled.Disposed) { depthDisabledStencilDisabledWriteEnabled.Dispose(); }
            if (!cullNoneFillSolid.Disposed) { cullNoneFillSolid.Dispose(); }
            if (!cullNoneFillWireframe.Disposed) { cullNoneFillWireframe.Dispose(); }
            if (!cullBackFillSolid.Disposed) { cullBackFillSolid.Dispose(); }
            if (!cullBackFillWireframe.Disposed) { cullBackFillWireframe.Dispose(); }
            if (!cullFrontFillSolid.Disposed) { cullFrontFillSolid.Dispose(); }
            if (!cullFrontFillWireframe.Disposed) { cullFrontFillWireframe.Dispose(); }
            if (!blendDisabled.Disposed) { blendDisabled.Dispose(); }
            if (!blendEnabledSourceAlphaInverseSourceAlpha.Disposed) { blendEnabledSourceAlphaInverseSourceAlpha.Dispose(); }
            if (!blendEnabledSourceAlphaDestinationAlpha.Disposed) { blendEnabledSourceAlphaDestinationAlpha.Dispose(); }
            if (!blendEnabledOneOne.Disposed) { blendEnabledOneOne.Dispose(); }
            if (!blendEnabledAlphaBlending.Disposed) { blendEnabledAlphaBlending.Dispose(); }
        }
    }
}
