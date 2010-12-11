// ui_item.cs (c) 2010 JV Software
//

using System;
using System.IO;
using idLib.Math;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{
    //
    // idUserInterfaceMenuDef
    //
     public class idUserInterfaceMenuDef : idUserInterfaceItem
    {
         public override void WriteBinaryFile(ref BinaryWriter writer)
       {
           writer.Write(fullScreen);
           writer.Write(itemCount);
           writer.Write(fontIndex);
           writer.Write(cursorItem);
           writer.Write(fadeCycle);
           writer.Write(fadeClamp);
           writer.Write(fadeAmount);
           writer.Write(onOpen);
           writer.Write(onClose);
           writer.Write(onESC);

           foreach (string s in onKey)
           {
               if (s == null)
               {
                   writer.Write("NULL");
                   break;
               }
               writer.Write(s);
           }
           writer.Write(soundName);
           writer.Write(onROQDone);

           for (int i = 0; i < 4; i++)
           {
               writer.Write(focusColor[i]);
           }
           for (int i = 0; i < 4; i++)
           {
               writer.Write(disableColor[i]);
           }

           for (int i = 0; i < itemCount; i++)
           {
               items[i].WriteBinaryFile(ref writer);
           }
       }

         public bool fullScreen = false;            // covers entire screen
        public int itemCount = 0;                  // number of items;
        public int fontIndex = 0;                  //
        public int cursorItem = 0;                 // which item as the cursor
        public int fadeCycle = 0;                  //
        public float fadeClamp = 0;                //
        public float fadeAmount = 0;               //
        public string onOpen = "";             // run when the menu is first opened
        public string onClose = "";            // run when the menu is closed
        public string onESC = "";              // run when the menu is closed
        public string[] onKey = new string[255];         // NERVE - SMF - execs commands when a key is pressed
        public string soundName = "";          // background loop sound for menu
        public string onROQDone = "";          //----(SA)	added.  callback for roqs played from menus

        public idVector4 focusColor = new idVector4();              // focus color for items
        public idVector4 disableColor = new idVector4();            // focus color for items
        public idUserInterfaceItem[] items = new idUserInterfaceItem[ui_globals.MAX_MENUITEMS]; // items this menu contains
    }
}
