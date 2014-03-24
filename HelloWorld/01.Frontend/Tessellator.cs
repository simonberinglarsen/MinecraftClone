﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.Windows;
using SlimDX.D3DCompiler;
using Device = SlimDX.Direct3D11.Device;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7._01.Frontend.Effects;

namespace WindowsFormsApplication7.Frontend
{
    class Tessellator
    {
        public static Tessellator Instance = new Tessellator();
        public int VertexCount = 0;
        public FXBase ActiveEffect;
        private ShaderResourceView activeTexture;
        public Device Device;
        public float ArrayIndex = -1;

        private DataStream stream;
        private int vertexAddCount = 0;
        private PrimitiveTopology mode = PrimitiveTopology.TriangleList;
        private Resources resources;
        private VS_IN vertex0;
        private VS_IN vertex2;
        private Vector2[] uvVertices = new Vector2[]{
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1),
        };
        private int uvIndex = 0;
        public struct VS_IN
        {
            public Vector4 Vertex;
            public Vector4 Color;
            public Vector2 Uv;
            public Vector3 Normal;
            public float TextureIndex;
        }

        public Tessellator()
        {
        }

        public void Initialize(int size, Device device)
        {
            this.Device = device;
            resources = new Resources(device);
            stream = new DataStream(size, true, true);
            resources.LoadAllTextures();
            BlockTextures.Instance.Initialize();
        }

        public void Draw()
        {
            if (VertexCount > 0)
            {
                SlimDX.Direct3D11.Buffer vertices = GetDrawBuffer();
                DrawBuffer(vertices);
                vertices.Dispose();
            }
            Reset();
        }

        public void Draw(SlimDX.Direct3D11.Buffer vertices, int count)
        {
            VertexCount = count;
            DrawBuffer(vertices);
            Reset();
        }

        public void Draw(VertexBuffer vertexBuffer)
        {
            VertexCount = vertexBuffer.VertexCount;
            DrawBuffer(vertexBuffer.Vertices);
            Reset();
        }

        public VertexBuffer GetVertexBuffer()
        {
            VertexBuffer vertexBuffer = new VertexBuffer();
            vertexBuffer.Vertices = GetDrawBuffer();
            vertexBuffer.VertexCount = VertexCount;
            return vertexBuffer;
        }

        public SlimDX.Direct3D11.Buffer GetDrawBuffer()
        {
            stream.Position = 0;
            if (VertexCount == 0)
                return null;
            SlimDX.Direct3D11.Buffer vertices = new SlimDX.Direct3D11.Buffer(Device, stream, new BufferDescription()
                {
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    SizeInBytes = VertexCount * ActiveEffect.GetStride(),
                    Usage = ResourceUsage.Default
                });
            return vertices;
        }


        private void DrawBuffer(SlimDX.Direct3D11.Buffer vertices)
        {
            if (VertexCount == 0)
                return;
            ActiveEffect.Apply(vertices);
            Device.ImmediateContext.InputAssembler.PrimitiveTopology = mode;
            Device.ImmediateContext.Draw(VertexCount, 0);

        }

        private void Reset()
        {
            // reset vertex info
            VertexCount = 0;
            vertexAddCount = 0;
            stream.Position = 0;
            uvIndex = 0;
        }

        internal void AddVertexWithColor(Vector4 vertex, Vector4 color)
        {
            AddVertexWithColor(vertex, color, new Vector3(0, 0, 0));
        }
        internal void AddVertexWithColor(Vector4 vertex, Vector4 color, Vector3 normal)
        {
            vertexAddCount++;
            VertexCount++;
            int stride = ActiveEffect.GetStride();
            VS_IN vsin = new VS_IN() { Vertex = vertex, Color = color, Uv = uvVertices[uvIndex++], TextureIndex = ArrayIndex, Normal = normal };
            uvIndex = uvIndex % 4;

            ActiveEffect.WriteVertex(stream, ref vsin);
            if (mode == PrimitiveTopology.TriangleList)
            {
                if (vertexAddCount % 4 == 1)
                {
                    vertex0 = vsin;
                }
                else if (vertexAddCount % 4 == 3)
                {
                    vertex2 = vsin;
                }
                else if (vertexAddCount % 4 == 0)
                {
                    VertexCount += 2;
                    ActiveEffect.WriteVertex(stream, ref vertex0);
                    ActiveEffect.WriteVertex(stream, ref vertex2);
                }
            }
            bool isStreamAlmostFull = (VertexCount * stride) >= stream.Length - 32 * stride;
            bool isQuadComplete = vertexAddCount % 4 == 0;
            bool timeToFlushVertices = isStreamAlmostFull && isQuadComplete;
            if (timeToFlushVertices)
            {
                Draw();
            }
        }

        internal void StartDrawingLines()
        {
            StartDrawing(null, FXSimple.Instance, 0, PrimitiveTopology.LineList);
        }

        public void StartDrawingTiledQuads()
        {
            SetTextureQuad(new Vector2(0, 0), 1f, 1f);
            StartDrawing(null, FXTiles.Instance, 0, PrimitiveTopology.TriangleList);
        }

        internal void StartDrawingColoredQuads()
        {
            StartDrawing(null, FXSimple.Instance, 0, PrimitiveTopology.TriangleList);
        }

        public void StartDrawingAlphaTexturedQuads(string textureName)
        {
            StartDrawing(textureName, FXTexture.Instance, 2, PrimitiveTopology.TriangleList);
        }

        private void StartDrawing(string textureName, FXBase effect, int technique, PrimitiveTopology mode)
        {
            activeTexture = resources.GetResource(textureName);
            ActiveEffect = effect;
            ActiveEffect.TechniqueIndex = technique;
            this.mode = mode;
            Reset();
        }

        public void Dispose()
        {
            BlockTextures.Instance.Dispose();
            FXSimple.Instance.Dispose();
            FXTexture.Instance.Dispose();
            resources.Dispose();
            stream.Dispose();
        }

        internal void SetTextureQuad(Vector2 corner, float width, float height)
        {
            uvIndex = 0;
            uvVertices[1].X = corner.X;
            uvVertices[1].Y = corner.Y;

            uvVertices[2].X = corner.X + width;
            uvVertices[2].Y = corner.Y;

            uvVertices[3].X = corner.X + width;
            uvVertices[3].Y = corner.Y + height;

            uvVertices[0].X = corner.X;
            uvVertices[0].Y = corner.Y + height;
        }

        public ShaderResourceView ActiveTexture
        {
            get
            {
                return activeTexture;
            }
        }
    }
}
