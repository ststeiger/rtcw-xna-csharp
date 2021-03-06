﻿// cgame_view.cs (c) 2010 JV Software
//

using idLib.Math;
using idLib.Engine.Public;

namespace cgame
{
    //
    // idView
    //
    public class idView
    {
        idVector3 viewxyz;
        idMatrix viewangle;
        public idRefdef refdef;

        //
        // SetViewOrigin
        //
        public void SetViewOrigin(idVector3 origin)
        {
            viewxyz.X = origin.X;
            viewxyz.Y = origin.Y;
            viewxyz.Z = origin.Z;
        }

        //
        // SetViewAngle
        //
        public void SetViewAngle(idVector3 angle)
        {
            viewangle = angle.ToAxis();
        }

        /*
        ====================
        CalcFov

        Fixed fov at intermissions, otherwise account for fov variable and zooms.
        ====================
        */

        private void CalcFov( ref float fov_x, ref float fov_y ) {
            float x = 0;

            fov_x = 90.0f;

            x = Engine.RenderSystem.GetViewportWidth() / (float)System.Math.Tan(fov_x / 360 * System.Math.PI);
            fov_y = (float)System.Math.Atan2(Engine.RenderSystem.GetViewportHeight(), x);
            fov_y = fov_y * 360.0f / (float)System.Math.PI;
        }

        //
        // GenerateRefdef
        //
        public void GenerateRefdef()
        {
            if (refdef == null && Globals.world != null)
            {
                refdef = Globals.world.AllocRefdef();
            }
        }

        //
        // ClearRefdef
        //
        private void ClearRefdef()
        {
            refdef = null;
        }

        //
        // DrawView
        //
        public void DrawView()
        {
            CalcFov(ref refdef.fov_x, ref refdef.fov_y);


            refdef.viewaxis = viewangle;
            refdef.vieworg = viewxyz;
            if (Globals.inCinematic == false)
            {
                refdef.vieworg.Z += 35.0f;

                refdef.x = 0;
                refdef.y = 0;
                refdef.width = Engine.RenderSystem.GetViewportWidth();
                refdef.height = Engine.RenderSystem.GetViewportHeight();
            }
            else
            {
                int vidWidth = Engine.RenderSystem.GetViewportWidth();
                int vidHeight = Engine.RenderSystem.GetViewportHeight();
                int xsize = 100;
                int ysize = 80;

                refdef.width = vidWidth * xsize / 100;
                refdef.width &= ~1;

                refdef.height = vidHeight * ysize / 100;
                refdef.height &= ~1;

                refdef.x = (vidWidth - refdef.width) / 2;
                refdef.y = (vidHeight - refdef.height) / 2;
            }
            

            refdef.rdflags = idRenderType.RDF_DRAWSKYBOX;

            Globals.world.RenderScene(refdef);

            ClearRefdef();
        }
    }
}
