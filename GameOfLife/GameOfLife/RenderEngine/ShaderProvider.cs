using System;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using System.Windows.Forms;

namespace GameOfLife.RenderEngine
{
    static class ShaderProvider
    {
        public const ShaderFlags Flags = ShaderFlags.OptimizationLevel3;

        public static PixelShader CompilePS(string path)
        {
            PixelShader ps = null;
            try
            {
                string compilerMessages;
                using (var bytecode = ShaderBytecode.CompileFromFile(path, "PS", "ps_4_0", Flags, EffectFlags.None, null, null, out compilerMessages))
                {
                    ps = new PixelShader(RenderFrame.Instance.device, bytecode);
                }
                if (!string.IsNullOrWhiteSpace(compilerMessages))
                {
                    System.Diagnostics.Debug.WriteLine(compilerMessages);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
            }
            return ps;
        }

        public static VertexShader CompileVS(string path)
        {
            VertexShader vs = null;
            try
            {
                string compilerMessages;
                using (var bytecode = ShaderBytecode.CompileFromFile(path, "VS", "vs_4_0", Flags, EffectFlags.None, null, null, out compilerMessages))
                {
                    vs = new VertexShader(RenderFrame.Instance.device, bytecode);
                }
                if (!string.IsNullOrWhiteSpace(compilerMessages))
                {
                    System.Diagnostics.Debug.WriteLine(compilerMessages);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
            }
            return vs;
        }

        public static Effect CompileFX(string path)
        {
            Effect fx = null;
            try
            {
                string compilerMessages;
                using (var bytecode = ShaderBytecode.CompileFromFile(path, null, "fx_5_0", Flags, EffectFlags.None, null, null, out compilerMessages))
                {
                    fx = new Effect(RenderFrame.Instance.device, bytecode);
                }
                if (!string.IsNullOrWhiteSpace(compilerMessages))
                {
                    System.Diagnostics.Debug.WriteLine(compilerMessages);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
            }
            return fx;
        }

        public static ShaderSignature GetSignatureFromShader(string path)
        {
            try
            {
                string compilerMessages;
                using (var bytecode = ShaderBytecode.CompileFromFile(path, "VS", "vs_4_0", Flags, EffectFlags.None, null, null, out compilerMessages))
                {
                    if (!string.IsNullOrWhiteSpace(compilerMessages))
                    {
                        System.Diagnostics.Debug.WriteLine(compilerMessages);
                    }
                    return ShaderSignature.GetInputSignature(bytecode);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
            }
            return null;
        }
    }
}
