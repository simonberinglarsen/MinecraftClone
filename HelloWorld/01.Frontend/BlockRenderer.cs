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
using System.Drawing;
using WindowsFormsApplication7.Business;
using WindowsFormsApplication7.CrossCutting.Entities;

namespace WindowsFormsApplication7.Frontend
{
    class BlockRenderer
    {
        private World world = World.Instance;

        public BlockRenderer()
        {
        }

        internal void RenderBlock(PositionBlock positionBlock)
        {
            int blockId = world.GetBlock(positionBlock.X, positionBlock.Y, positionBlock.Z);
            if (blockId == 0)
                return;

            Block block = BlockRepository.Blocks[blockId];
            int x = positionBlock.X;
            int y = positionBlock.Y;
            int z = positionBlock.Z;

            Vector4 c1, c2, c3, c4, c5, c6;
            Vector4[] blockColors = BlockColorMap.BlockColors[blockId];
            if (blockColors == null)
            {
                c1 = new Vector4(1f, 1f, 1f, 1f);
                c2 = c1;
                c3 = c1;
                c4 = c1;
                c5 = c1;
                c6 = c1;
            }
            else
            {
                c1 = blockColors[0];
                c2 = blockColors[1];
                c3 = blockColors[2];
                c4 = blockColors[3];
                c5 = blockColors[4];
                c6 = blockColors[5];
            }
            float factor = (float)(1d - MathLibrary.Random.NextDouble() * 0.1);
            c1 = c1 * factor;
            c2 = c2 * factor;
            c3 = c3 * factor;
            c4 = c4 * factor;
            c5 = c5 * factor;
            c6 = c6 * factor;
            const float reduction = 0.5f;
            float sideShadow = 1f;

            Tessellator tessellator = Tessellator.Instance;
            if (world.GetBlock(x, y, z + 1) == 0)
            {
                // front
                sideShadow = world.GetBlock(x, y-1, z + 1)!=0 ? reduction : 1f;
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 0f, z + 1f, 1.0f), c1 * sideShadow);
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 1f, z + 1f, 1.0f), c1);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 1f, z + 1f, 1.0f), c1);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 1f, 1.0f), c1 * sideShadow);
            }

            if (world.GetBlock(x, y, z - 1) == 0)
            {
                // back
                sideShadow = world.GetBlock(x, y-1, z - 1) != 0 ? reduction : 1f;
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 0f, 1.0f), c2 * sideShadow);
                tessellator.AddVertexWithColor(new Vector4(x + 1, y + 1f, z + 0f, 1.0f), c2);
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 1f, z + 0f, 1.0f), c2);
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 0f, z + 0f, 1.0f), c2 * sideShadow);
            }

            if (world.GetBlock(x - 1, y, z) == 0)
            {
                //left
                sideShadow = world.GetBlock(x - 1, y-1, z) != 0 ? reduction : 1f;
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 0f, z + 0f, 1.0f), c3 * sideShadow);
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 1f, z + 0f, 1.0f), c3);
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 1f, z + 1f, 1.0f), c3);
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 0f, z + 1f, 1.0f), c3 * sideShadow);
            }

            if (world.GetBlock(x + 1, y, z) == 0)
            {
                //right
                sideShadow = world.GetBlock(x + 1, y-1, z) != 0 ? reduction : 1f;
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 1f, 1.0f), c4 * sideShadow);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 1f, z + 1f, 1.0f), c4);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 1f, z + 0f, 1.0f), c4);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 0f, 1.0f), c4 * sideShadow);
            }

            if (world.GetBlock(x, y + 1, z) == 0)
            {
                //top
                float a, b, c, d, e, f, g, h;
                a = world.GetBlock(x - 1, y + 1, z) == 0 ? 1f : reduction;
                b = world.GetBlock(x, y + 1, z - 1) == 0 ? 1f : reduction;
                c = world.GetBlock(x + 1, y + 1, z) == 0 ? 1f : reduction;
                d = world.GetBlock(x, y + 1, z + 1) == 0 ? 1f : reduction;

                e = world.GetBlock(x - 1, y + 1, z + 1) == 0 ? 1f : reduction;
                f = world.GetBlock(x - 1, y + 1, z - 1) == 0 ? 1f : reduction;
                g = world.GetBlock(x + 1, y + 1, z - 1) == 0 ? 1f : reduction;
                h = world.GetBlock(x + 1, y + 1, z + 1) == 0 ? 1f : reduction;

                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 1f, z + 1f, 1.0f), c5 * a * d * e);
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 1f, z + 0f, 1.0f), c5 * a * b * f);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 1f, z + 0f, 1.0f), c5 * b * c * g);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 1f, z + 1f, 1.0f), c5 * c * d * h);
            }

            if (world.GetBlock(x, y - 1, z) == 0)
            {
                //bottom
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 0f, z + 0f, 1.0f), c6);
                tessellator.AddVertexWithColor(new Vector4(x + 0f, y + 0f, z + 1f, 1.0f), c6);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 1f, 1.0f), c6);
                tessellator.AddVertexWithColor(new Vector4(x + 1f, y + 0f, z + 0f, 1.0f), c6);
            }



        }
    }
}