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

            LoadAssets();
        }

        //
        // AssetStringValid
        //
        private bool AssetStringValid(string str)
        {
            if (str == null)
                return false;

            if (str.Length <= 0)
                return false;

            return true;
        }

        //
        // UI_RegisterFont
        //
        private void UI_RegisterFont( string name, out idFont font )
        {
            // Load all the UI specified assets.
            if (AssetStringValid(name))
            {
                font = Engine.RenderSystem.RegisterFont(name);
                return;
            }

            font = null;
        }

        //
        // UI_RegisterMaterial
        //
        private void UI_RegisterMaterial( string name, out idMaterial mtr )
        {
            if(AssetStringValid(name))
            {
                mtr = Engine.materialManager.FindMaterial( name, -1 );
                return;
            }

            mtr = null;
        }

        //
        // UI_RegisterSound
        //
        private void UI_RegisterSound(string name, out idSound snd)
        {
            if (AssetStringValid(name))
            {
                snd = Engine.soundManager.LoadSound(name);
                return;
            }

            snd = null;
        }

        //
        // UI_RegisterModel
        //
        private void UI_RegisterModel(string name, out idModel model)
        {
            if (AssetStringValid(name))
            {
                model = Engine.modelManager.LoadModel(name);
                return;
            }

            model = null;
        }

        //
        // UI_RegisterVideo
        //
        private void UI_RegisterVideo(string name, out idVideo video)
        {
            if (AssetStringValid(name))
            {
                video = Engine.RenderSystem.LoadVideo(name);
                return;
            }

            video = null;
        }

        //
        // LoadAssets
        //
        private void LoadAssets()
        {
            UI_RegisterFont(assets.bigFont, out assets.handles.bigFont );
            UI_RegisterFont(assets.smallFont, out assets.handles.smallFont);
            UI_RegisterFont(assets.textFont, out assets.handles.textFont);
            UI_RegisterFont(assets.handwritingFont, out assets.handles.handwritingFont);

            UI_RegisterMaterial(assets.cursor, out assets.handles.cursor );
            UI_RegisterMaterial(assets.gradientBar, out assets.handles.gradientBar );
            UI_RegisterMaterial(assets.scrollBarArrowUp, out assets.handles.scrollBarArrowUp );
            UI_RegisterMaterial(assets.scrollBarArrowDown, out assets.handles.scrollBarArrowDown );
            UI_RegisterMaterial(assets.scrollBarArrowLeft, out assets.handles.scrollBarArrowLeft );
            UI_RegisterMaterial(assets.scrollBarArrowRight, out assets.handles.scrollBarArrowRight );
            UI_RegisterMaterial(assets.scrollBar, out assets.handles.scrollBar );
            UI_RegisterMaterial(assets.scrollBarThumb, out assets.handles.scrollBarThumb );
            UI_RegisterMaterial(assets.buttonMiddle, out assets.handles.buttonMiddle );
            UI_RegisterMaterial(assets.buttonInside, out assets.handles.buttonInside );
            UI_RegisterMaterial(assets.solidBox, out assets.handles.solidBox );
            UI_RegisterMaterial(assets.sliderBar, out assets.handles.sliderBar );
            UI_RegisterMaterial(assets.sliderThumb, out assets.handles.sliderThumb );

            UI_RegisterSound(assets.menuEnterSound, out assets.handles.menuEnterSound);
            UI_RegisterSound(assets.menuExitSound, out assets.handles.menuExitSound);
            UI_RegisterSound(assets.menuBuzzSound, out assets.handles.menuBuzzSound);
            UI_RegisterSound(assets.itemFocusSound , out assets.handles.itemFocusSound);

            UI_RegisterModel(menu.window.model, out menu.window.modelHandle);
            UI_RegisterVideo(menu.window.cinematicName, out menu.window.cinematicHandle);

            for (int i = 0; i < menu.itemCount; i++)
            {
                UI_RegisterModel(menu.items[i].asset_model, out menu.items[i].model);
                UI_RegisterModel(menu.items[i].window.model, out menu.items[i].window.modelHandle);

                UI_RegisterMaterial(menu.items[i].asset_shader, out menu.items[i].material);
                UI_RegisterModel(menu.items[i].asset_model, out menu.items[i].model);

                UI_RegisterVideo(menu.items[i].window.cinematicName, out menu.items[i].window.cinematicHandle);
                UI_RegisterSound(menu.items[i].focusSound, out menu.items[i].focusSnd);
            }
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
