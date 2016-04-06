using System.Runtime.InteropServices;
using SlimDX;

namespace GameOfLife.RenderEngine
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Vertex
    {
        public static int SizeInBytes = 32;

        [FieldOffset(0)]
        public Vector4 VertexPosition;
        [FieldOffset(16)]
        public Vector4 Normal;

        public Vertex(Vector4 vec, Vector3 nor)
        {
            VertexPosition = vec;
            Normal = new Vector4(nor, 1);
        }

        public Vertex(Vector4 vec, Vector4 nor)
        {
            VertexPosition = vec;
            Normal = nor;
        }
    }
}
