using System;
using SlimDX.D3DCompiler;
using System.IO;

namespace GameOfLife.RenderEngine
{
    class IncludeHandler : Include
    {
        private const string includeDirectory = ".\\Shader\\";

        public void Close(Stream stream)
        {
            stream.Close();
            stream.Dispose();
        }

        public void Open(IncludeType type, string fileName, Stream parentStream, out Stream stream)
        {
            if (type == IncludeType.Local)
            {
                stream = new FileStream(includeDirectory + fileName, FileMode.Open);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
