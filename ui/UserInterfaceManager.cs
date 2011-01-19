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

        public static idCVar ui_smallFont;
        public static idCVar ui_bigFont;

        //
        // idUserInterfaceManagerLocal
        //
        public idUserInterfaceManagerLocal( int version )
        {
            Engine.common.Printf("UI Module Loaded...\n");

            ui_smallFont = Engine.cvarManager.Cvar_Get("ui_smallFont", "0.25", idCVar.CVAR_ARCHIVE);
            ui_bigFont = Engine.cvarManager.Cvar_Get("ui_bigFont", "0.4", idCVar.CVAR_ARCHIVE);
        }

        //
        // LoadMenusFromUI
        //
        private void LoadMenusFromUI(string uipath)
        {
            idUserInterfaceCachedAssets assets = new idUserInterfaceCachedAssets();
            idFile file = Engine.fileSystem.OpenFileRead(uipath + ".xnb", true);

            assets.ReadBinaryFile(ref file);

            int numguis = file.ReadInt();
            for (int i = 0; i < numguis; i++)
            {
                idUserInterfaceMenuDef menu = new idUserInterfaceMenuDef();
                menu.ReadBinaryFile(ref file);
                Engine.ui.LoadUIFromMemory(assets, menu);
            }

            Engine.fileSystem.CloseFile(ref file);
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
                    LoadMenusFromUI("ui/" + fileList[i]);
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
