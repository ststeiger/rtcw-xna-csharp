// cgame_view.cs (c) 2010 JV Software
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

        //
        // DrawView
        //
        public void DrawView()
        {
            idRefdef refdef = Globals.world.AllocRefdef();

            refdef.fov_x = 90.0f;
            refdef.fov_y = 90.0f;

            refdef.viewaxis = viewangle;
            refdef.vieworg = viewxyz;
            refdef.x = 0;
            refdef.y = 0;
            refdef.width = Engine.RenderSystem.GetViewportWidth();
            refdef.height = Engine.RenderSystem.GetViewportHeight();

            refdef.rdflags = 0;

            Globals.world.RenderScene(refdef);
        }
    }
}
