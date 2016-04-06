using System.Runtime.InteropServices;
using SlimDX;

namespace GameOfLife.RenderEngine
{
    [StructLayout(LayoutKind.Explicit)]
    struct PPSQVertex
    {
        [FieldOffset(0)]
        public Vector4 com;

        public PPSQVertex(Vector2 pos, Vector2 tex)
        {
            com.X = pos.X;
            com.Y = pos.Y;
            com.Z = tex.X;
            com.W = tex.Y;
        }
    }

}
