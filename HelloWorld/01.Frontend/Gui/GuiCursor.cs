﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using WindowsFormsApplication7.CrossCutting.Entities;
using WindowsFormsApplication7.Business;

namespace WindowsFormsApplication7.Frontend.Gui
{
    class GuiCursor
    {
        private Tessellator t = Tessellator.Instance;

        public void Render(float partialStep)
        {
            t.ResetTransformation();
            string cursorText = "+";
            FontRenderer f = FontRenderer.Instance;
            t.StartDrawingAlphaTexturedQuads("ascii");
            Vector2 textSize = f.TextSize(cursorText);
            Vector2 Location = Input.Instance.InterpolatedMouseLocation(partialStep);
            f.RenderTextShadow(cursorText, Location.X - textSize.X / 2f, Location.Y - textSize.Y / 2f);
            t.Draw();
        }
    }
}
