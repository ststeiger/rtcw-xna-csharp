// ui_rect.cs (c) 2010 JV Software
//

using System;
using idLib.Math;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceRectangle
    //
    public class idUserInterfaceRectangle
    {
        idVector4 v;
        public idUserInterfaceRectangle()
        {
            v = new idVector4();
        }

        // horiz position
        public float x
        {
            get
            {
                return v.X;
            }
            set
            {
                v.X = value;
            }
        }

        // vert position
        public float y
        {
            get
            {
                return v.Y;
            }
            set
            {
                v.Y = value;
            }
        }

        // width
        public float w
        {
            get
            {
                return v.Z;
            }
            set
            {
                v.Z = value;
            }
        }

        // height;
        public float h
        {
            get
            {
                return v.W;
            }
            set
            {
                v.W = value;
            }
        }
    }
}
