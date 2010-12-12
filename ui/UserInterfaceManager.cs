// UserInterfaceManager.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using idLib.Engine.Content;
using idLib.Engine.Public;
using idLib.Engine.Content.ui;
using idLib.Engine.Content.ui.Private;

namespace ui
{
    //
    // idUserInterfaceManagerLocal
    //
    public class idUserInterfaceManagerLocal : idUserInterfaceManager
    {
        List<idUserInterfaceLocal> uipool = new List<idUserInterfaceLocal>();

        //
        // idUserInterfaceManagerLocal
        //
        public idUserInterfaceManagerLocal( int version )
        {
            Engine.common.Printf("UserInterface Module Loaded...\n");
        }

        //
        // Init
        //
        public override void Init()
        {
            idFileList fileList;
            
            // Get all the menus in the current directories.
            fileList = Engine.fileSystem.ListFiles("ui", ".menu");

            // Load all the ui's into memory, this assumes the menus are there, fixme?
            Engine.common.Printf( "Parsing UserInterfaces...\n");
            for( int i = 0; i < fileList.Count; i++ ) {
                if (fileList[i].Contains("menudef") == false)
                {
                    Engine.fileSystem.ReadContent<idUserInterfaceLocal>("ui/" + fileList[i]);
                    Engine.common.Printf("...ui/" + fileList[i] + "\n");
                }
            }

            // Free the file list.
            Engine.fileSystem.FreeFileList(ref fileList);
        }

        //
        // LoadUIFromMemory
        //
        public override idUserInterface LoadUIFromMemory(idUserInterfaceCachedAssets assets, idUserInterfaceMenuDef menu)
        {
            idUserInterfaceLocal ui = new idUserInterfaceLocal(assets, menu);

            // Add the UI to the pool.
            uipool.Add(ui);

            return uipool[uipool.Count - 1];
        }
    }
}
