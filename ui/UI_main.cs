// UI_main.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;
using idLib.Engine.Content.ui;
using idLib.Engine.Content.ui.Private;

namespace ui
{
    //
    // idUserInterfaceLocal
    //
    public class idUserInterfaceLocal : idUserInterface
    {
        private string hashname;
        private idUserInterfaceMenuDef menu;
        private idUserInterfaceCachedAssets assets;

        //
        // idUserInterfaceLocal
        //
        public idUserInterfaceLocal(idUserInterfaceCachedAssets assets, idUserInterfaceMenuDef menu)
        {
            hashname = menu.window.name;
            this.assets = assets;
            this.menu = menu;
        }

        //
        // LoadAssets
        //
        private void LoadAssets()
        {

        }

        //
        // GetName
        // 
        public override string GetName()
        {
            return hashname;
        }

        //
        // Draw
        //
        public override void Draw()
        {
            
        }
    }
}
