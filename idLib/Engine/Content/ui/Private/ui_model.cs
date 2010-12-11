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
        public int angle = 0;
        public idVector3 origin = new idVector3();
        public float fov_x = 0;
        public float fov_y = 0;
        public int rotationSpeed = 0;

        public int animated = 0;
        public int startframe = 0;
        public int numframes = 0;
        public int loopframes = 0;
        public int fps = 0;

        public int frame = 0;
        public int oldframe = 0;
        public float backlerp = 0;
	    public int frameTime = 0;

        public override void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            writer.Write(angle);
            for (int i = 0; i < 3; i++)
            {
                writer.Write(origin[i]);
            }
            writer.Write(fov_x);
            writer.Write(fov_y);
            writer.Write(rotationSpeed);
            writer.Write(animated);
            writer.Write(startframe);
            writer.Write(numframes);
            writer.Write(loopframes);
            writer.Write(fps);
            writer.Write(frame);
            writer.Write(oldframe);
            writer.Write(backlerp);
            writer.Write(frameTime);
        }
    }
}
