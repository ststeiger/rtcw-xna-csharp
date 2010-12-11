// ui_window.cs (c) 2010 JV Software
//

using System;
using idLib.Math;

namespace idLib.Engine.Content.ui.Private
{
    //
    // idUserInterfaceWindow
    //
    public class idUserInterfaceWindow
    {
        public idUserInterfaceWindow()
        {
            // Allocate the window for the menu and set the default values.
            borderSize = 1;
            foreColor[0] = foreColor[1] = foreColor[2] = foreColor[3] = 1.0f;
            cinematic = -1;
        }

        public void WriteBinaryFile(ref System.IO.BinaryWriter writer)
        {
            rect.WriteBinaryFile(ref writer);
            rectClient.WriteBinaryFile(ref writer);
            writer.Write(name);
            writer.Write(model);
            writer.Write(group);
            writer.Write(cinematicName);
            writer.Write(cinematic);
            writer.Write(style);
            writer.Write(border);
            writer.Write(ownerDraw);
            writer.Write(ownerDrawFlags);
            writer.Write(borderSize);
            writer.Write(flags);
            //rectEffects.WriteBinaryFile(ref writer);
           // rectEffects2.WriteBinaryFile(ref writer);
            writer.Write(offsetTime);
            writer.Write(nextTime);

            for (int i = 0; i < 4; i++)
            {
                writer.Write(foreColor[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write(backColor[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write(borderColor[i]);
            }

            for (int i = 0; i < 4; i++)
            {
                writer.Write(outlineColor[i]);
            }

            writer.Write(background);
        }

        public idUserInterfaceRectangle rect = new idUserInterfaceRectangle();                 // client coord rectangle
        public idUserInterfaceRectangle rectClient = new idUserInterfaceRectangle();           // screen coord rectangle
	    public string name = "";               //
        public string model = "";              //
        public string group = "";              // if it belongs to a group
        public string cinematicName = "";      // cinematic name
        public int cinematic = 0;                  // cinematic handle
        public int style = 0;                      //
        public int border = 0;                     //
        public int ownerDraw = 0;                  // ownerDraw style
        public int ownerDrawFlags = 0;             // show flags for ownerdraw items
        public float borderSize = 0;               //
        public int flags = 0;                      // visible, focus, mouseover, cursor
        public idUserInterfaceRectangle rectEffects;          // for various effects
        public idUserInterfaceRectangle rectEffects2;         // for various effects
        public int offsetTime = 0;                 // time based value for various effects
        public int nextTime = 0;                   // time next effect should cycle
	    public idVector4 foreColor = new idVector4();               // text color
        public idVector4 backColor = new idVector4();               // border color
        public idVector4 borderColor = new idVector4();             // border color
        public idVector4 outlineColor = new idVector4();            // border color
        public string background = "";
    }
}
