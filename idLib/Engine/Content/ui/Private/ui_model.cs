// ui_model.cs (c) 2010 JV Software
//

using System;
using idLib.Math;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceModel
    //
    public class idUserInterfaceModel : idUserInterfaceDefBase
    {
        public int angle;
        public idVector3 origin;
        public float fov_x;
        public float fov_y;
        public int rotationSpeed;

        public int animated;
        public int startframe;
        public int numframes;
        public int loopframes;
        public int fps;

        public int frame;
        public int oldframe;
        public float backlerp;
	    public int frameTime;
    }
}
