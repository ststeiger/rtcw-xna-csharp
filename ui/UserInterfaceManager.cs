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
            Engine.common.Printf("UI Module Loaded...\n");
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
                    Engine.common.Printf("...ui/" + fileList[i] + "\n");
                    Engine.fileSystem.ReadContent<idUserInterfaceLocal>("ui/" + fileList[i]);
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

        //
        // FindUserInterface
        //
        public override idUserInterface FindUserInterface(string uiname)
        {
            // Try to locate the UI in the ui pool.
            for (int i = 0; i < uipool.Count; i++)
            {
                if (uiname == uipool[i].GetName())
                {
                    return uipool[i];
                }
            }

            Engine.common.Warning("UI_Load: Failed to find " + uiname + "\n");
            return null;
        }
    }
}
