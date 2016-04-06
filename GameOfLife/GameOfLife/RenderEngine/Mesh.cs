using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using SlimDX;
using DX11 = SlimDX.Direct3D11;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using GameOfLife.Storage;

namespace GameOfLife.RenderEngine
{
    public class Mesh
    {
        private readonly DX11.Buffer vertexBuffer;
        private readonly DX11.Buffer indexBuffer;
        private readonly int indexcount;
        private readonly VertexBufferBinding binding;
        public bool Disposed = false;

        public Mesh(ICollection<Vertex> vertices, ICollection<short> indices)
        {
            DataStream outStream = new DataStream(vertices.Count * Marshal.SizeOf(typeof(Vertex)), true, true);
            DataStream outStreamIndex = new DataStream(indices.Count * Marshal.SizeOf(typeof(short)), true, true);
            indexcount = indices.Count;

            foreach (Vertex v in vertices)
            {
              outStream.Write(v);
            }

            foreach (short i in indices)
            {
              outStreamIndex.Write(i);
            }

          outStream.Position = 0;
            outStreamIndex.Position = 0;

            BufferDescription bufferDescription = new BufferDescription
            {
              BindFlags = BindFlags.VertexBuffer, 
              CpuAccessFlags = CpuAccessFlags.None, 
              OptionFlags = ResourceOptionFlags.None,
              SizeInBytes = vertices.Count * Marshal.SizeOf(typeof(Vertex)), 
              Usage = ResourceUsage.Immutable
            };

            BufferDescription bufferDescriptionIndex = new BufferDescription
            {
                BindFlags = BindFlags.IndexBuffer, 
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None, 
                SizeInBytes = indices.Count * Marshal.SizeOf(typeof(short)),
                Usage = ResourceUsage.Immutable
            };

            try // just in case were shutting down the application, dont die here.
            {
                vertexBuffer = new DX11.Buffer(RenderFrame.Instance.device, outStream, bufferDescription);
                indexBuffer = new DX11.Buffer(RenderFrame.Instance.device, outStreamIndex, bufferDescriptionIndex);
                binding = new VertexBufferBinding(vertexBuffer, Marshal.SizeOf(typeof(Vertex)), 0);
            }
            catch (ObjectDisposedException)
            {

            }

            outStream.Close();
            outStreamIndex.Close();
            outStream.Dispose();
            outStreamIndex.Dispose();
        }

        public void Render()
        {
            var c = RenderFrame.Instance.deviceContext;
            c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);
            c.InputAssembler.SetVertexBuffers(0, binding);

            c.DrawIndexed(indexcount, 0, 0);
        }

        internal void Dispose()
        {
            Disposed = true;
            if (!indexBuffer.Disposed) { indexBuffer.Dispose(); }
            if (!vertexBuffer.Disposed) { vertexBuffer.Dispose(); }
        }

        public static Mesh OversizedScreenPolygon()
        {
            List<Vertex> vertices = new List<Vertex>();
            List<short> indices = new List<short> {1, 0, 2};

            vertices.Add(new Vertex(new Vector4(2, 2, 0, 1), new Vector3(2, 0, 0)));
            vertices.Add(new Vertex(new Vector4(-2, 2, 0, 1), new Vector3(0, 0, 0)));
            vertices.Add(new Vertex(new Vector4(2, -2, 0, 1), new Vector3(2, 2, 0)));

            return new Mesh(vertices, indices);
        }

        public static Mesh ScreenQuad()
        {
            List<Vertex> vertices = new List<Vertex>();
            List<short> indices = new List<short> {3, 0, 1, 2, 3, 1};

            vertices.Add(new Vertex(new Vector4(1, 1, 1, 0), new Vector3(1f / Config.Width, 1f / Config.Height, 0)));
            vertices.Add(new Vertex(new Vector4(-1, 1, 0, 0), new Vector3(1f / Config.Width, 1f / Config.Height, 0)));
            vertices.Add(new Vertex(new Vector4(-1, -1, 0, 1), new Vector3(1f / Config.Width, 1f / Config.Height, 0)));
            vertices.Add(new Vertex(new Vector4(1, -1, 1, 1), new Vector3(1f / Config.Width, 1f / Config.Height, 0)));

            return new Mesh(vertices, indices);
        }
    }
}