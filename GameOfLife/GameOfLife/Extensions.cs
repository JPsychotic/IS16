using SlimDX;

namespace GameOfLife
{
    public static class Extensions
    {
        public static Vector4 ToVector4(this Color3 col)
        {
            return new Vector4(col.Red, col.Green, col.Blue, 1);
        }

        public static Vector4 ToVector4(this Color4 col)
        {
            return new Vector4(col.Red, col.Green, col.Blue, col.Alpha);
        }

        public static Vector3 ToVector3(this Color3 col)
        {
            return new Vector3(col.Red, col.Green, col.Blue);
        }

        public static Vector3 ToVector3(this Color4 col)
        {
            return new Vector3(col.Red, col.Green, col.Blue);
        }
    }
}
