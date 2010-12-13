// UI_main.cs (c) 2010 JV Software
//

using System;
using idLib.Math;
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
        private idImage whiteImage;

        //
        // idUserInterfaceLocal
        //
        public idUserInterfaceLocal(idUserInterfaceCachedAssets assets, idUserInterfaceMenuDef menu)
        {
            hashname = menu.window.name;
            this.assets = assets;
            this.menu = menu;

            LoadAssets();

            whiteImage = Engine.imageManager.FindImage("*white");
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
        private void UI_RegisterFont( string name, int pointSize, out idFont font )
        {
            // Load all the UI specified assets.
            if (AssetStringValid(name))
            {
                font = Engine.RenderSystem.RegisterFont(name, pointSize);
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
            UI_RegisterFont(assets.bigFont, assets.bigFontSize, out assets.handles.bigFont );
            UI_RegisterFont(assets.smallFont, assets.smallFontSize, out assets.handles.smallFont);
            UI_RegisterFont(assets.textFont, assets.textFontSize,  out assets.handles.textFont);
            UI_RegisterFont(assets.handwritingFont, assets.handwritingFontSize, out assets.handles.handwritingFont);

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

            UI_RegisterMaterial(menu.window.background, out menu.window.backgroundHandle);
            UI_RegisterModel(menu.window.model, out menu.window.modelHandle);
            UI_RegisterVideo(menu.window.cinematicName, out menu.window.cinematicHandle);

            for (int i = 0; i < menu.itemCount; i++)
            {
                menu.items[i].parent = menu;
                UI_RegisterModel(menu.items[i].asset_model, out menu.items[i].model);
                UI_RegisterModel(menu.items[i].window.model, out menu.items[i].window.modelHandle);

                UI_RegisterMaterial(menu.items[i].window.background, out menu.items[i].window.backgroundHandle);
                UI_RegisterModel(menu.items[i].window.model, out menu.items[i].window.modelHandle);
                UI_RegisterVideo(menu.items[i].window.cinematicName, out menu.items[i].window.cinematicHandle);

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
        // FillRect
        //
        private void FillRect(int x, int y, int w, int h, idVector4 color)
        {
            Engine.RenderSystem.SetColor(color.X, color.Y, color.Z, color.W);
            UI_DrawHandlePic(x, y, w, h, whiteImage);
            Engine.RenderSystem.SetColor(1, 1, 1, 1);
        }

        //
        // PaintWindow
        //
        private void PaintWindow(ref idUserInterfaceWindow window)
        {
            idUserInterfaceRectangle fillRect;

            if (window == null || (window.style == 0 && window.border == 0))
            {
                return;
            }

            fillRect = window.rect;

            if (window.border != 0)
            {
                fillRect.x += window.borderSize;
                fillRect.y += window.borderSize;
                fillRect.w -= window.borderSize + 1;
                fillRect.h -= window.borderSize + 1;
            }

            if (window.style == ui_menudef.WINDOW_STYLE_FILLED)
            {
                // box, but possible a shader that needs filled
                if (window.backgroundHandle != null)
                {
                    //Fade(&w->flags, &w->backColor[3], fadeClamp, &w->nextTime, fadeCycle, qtrue, fadeAmount);
                    Engine.RenderSystem.SetColor(window.backColor[0], window.backColor[1], window.backColor[2], window.backColor[3]);
                    UI_DrawHandlePic(fillRect.x, fillRect.y, fillRect.w, fillRect.h, window.backgroundHandle);
                    Engine.RenderSystem.SetColor(1, 1, 1, 1);
                }
                else
                {
                    FillRect((int)fillRect.x, (int)fillRect.y, (int)fillRect.w, (int)fillRect.h, window.backColor);
                }
            }
            else if (window.style == ui_menudef.WINDOW_STYLE_GRADIENT)
            {
                //GradientBar_Paint(&fillRect, w->backColor);
                // gradient bar
            }
            else if (window.style == ui_menudef.WINDOW_STYLE_SHADER)
            {
                if ((window.flags & ui_globals.WINDOW_FORECOLORSET) != 0)
                {
                    Engine.RenderSystem.SetColor(window.foreColor[0], window.foreColor[1], window.foreColor[2], window.foreColor[3]);
                }
                UI_DrawHandlePic(fillRect.x, fillRect.y, fillRect.w, fillRect.h, window.backgroundHandle);
                Engine.RenderSystem.SetColor(1, 1, 1, 1);
            }
            else if (window.style == ui_menudef.WINDOW_STYLE_TEAMCOLOR)
            {
                //if (DC->getTeamColor)
                //{
                //    DC->getTeamColor(&color);
                //    DC->fillRect(fillRect.x, fillRect.y, fillRect.w, fillRect.h, color);
                //}
            }
            else if (window.style == ui_menudef.WINDOW_STYLE_CINEMATIC)
            {
#if false
                if (w->cinematic == -1)
                {
                    w->cinematic = DC->playCinematic(w->cinematicName, fillRect.x, fillRect.y, fillRect.w, fillRect.h);
                    if (w->cinematic == -1)
                    {
                        w->cinematic = -2;
                    }
                }
                if (w->cinematic >= 0)
                {
                    DC->runCinematicFrame(w->cinematic);
                    DC->drawCinematic(w->cinematic, fillRect.x, fillRect.y, fillRect.w, fillRect.h);
                }
#endif
            }

            if (window.style == ui_menudef.WINDOW_BORDER_FULL)
            {

                // full
                // HACK HACK HACK
                if (window.style == ui_menudef.WINDOW_STYLE_TEAMCOLOR)
                {
#if false
                    if (color[0] > 0)
                    {
                        // red
                        color[0] = 1;
                        color[1] = color[2] = .5;

                    }
                    else
                    {
                        color[2] = 1;
                        color[0] = color[1] = .5;
                    }
                    color[3] = 1;
                    DC->drawRect(w->rect.x, w->rect.y, w->rect.w, w->rect.h, w->borderSize, color);
#endif
                }
                else
                {
                    _UI_DrawRect(window.rect.x, window.rect.y, window.rect.w, window.rect.h, window.borderSize, window.borderColor);
                }

            }
            else if (window.style == ui_menudef.WINDOW_BORDER_HORZ)
            {
                // top/bottom
                Engine.RenderSystem.SetColor(window.borderColor.X, window.borderColor.Y, window.borderColor.Z, window.borderColor.W);
                _UI_DrawTopBottom(window.rect.x, window.rect.y, window.rect.w, window.rect.h, window.borderSize);
                Engine.RenderSystem.SetColor(1, 1, 1, 1);
            }
            else if (window.style == ui_menudef.WINDOW_BORDER_VERT)
            {
                // left right
                _UI_DrawRect(window.rect.x, window.rect.y, window.rect.w, window.rect.h, window.borderSize, window.borderColor);
            }
            else if (window.style == ui_menudef.WINDOW_BORDER_KCGRADIENT)
            {
                /*
                // this is just two gradient bars along each horz edge
                rectDef_t r = w->rect;
                r.h = w->borderSize;
                GradientBar_Paint(&r, w->borderColor);
                r.y = w->rect.y + w->rect.h - 1;
                GradientBar_Paint(&r, w->borderColor);
                */
            }

        }

        //
        // _UI_DrawSides
        //
        void _UI_DrawSides( float x, float y, float w, float h, float size ) {
            Engine.RenderSystem.DrawStrechPic((int)x, (int)y, (int)size, (int)h, whiteImage);
            Engine.RenderSystem.DrawStrechPic((int)(x + w - size), (int)y, (int)size, (int)h, whiteImage);
        }

        void _UI_DrawTopBottom( float x, float y, float w, float h, float size ) {
            Engine.RenderSystem.DrawStrechPic((int)x, (int)y, (int)w, (int)size, whiteImage);
            Engine.RenderSystem.DrawStrechPic((int)x, (int)(y + h - size), (int)w, (int)size, whiteImage);
        }
        /*
        ================
        UI_DrawRect

        Coordinates are 640*480 virtual values
        =================
        */
        void _UI_DrawRect( float x, float y, float width, float height, float size, idVector4 color ) {
            if (width == 0 && height == 0)
            {
                return;
            }
	        Engine.RenderSystem.SetColor(color.X, color.Y, color.Z, color.W);

	        _UI_DrawTopBottom( x, y, width, height, size );
	        _UI_DrawSides( x, y, width, height, size );

            Engine.RenderSystem.SetColor(1, 1, 1, 1);
        }

        //
        // Item_SetScreenCoords
        //
        private void Item_SetScreenCoords(ref idUserInterfaceItem item, float x, float y)
        {
            if (item == null)
            {
                return;
            }

            if (item.window.border != 0)
            {
                x += item.window.borderSize;
                y += item.window.borderSize;
            }

            item.window.rect.x = x + item.window.rectClient.x;
            item.window.rect.y = y + item.window.rectClient.y;
            item.window.rect.w = item.window.rectClient.w;
            item.window.rect.h = item.window.rectClient.h;

            // force the text rects to recompute
            item.textRect.w = 0;
            item.textRect.h = 0;
        }

        //
        // PaintItem
        //
        private void PaintItem(ref idUserInterfaceItem item)
        {
            if (item.window.backgroundHandle != null)
            {
                UI_DrawHandlePic(item.window.rectClient.x, item.window.rectClient.y, item.window.rectClient.w, item.window.rectClient.h, item.window.backgroundHandle);
            }
            PaintWindow(ref item.window);

        }

        //
        // UI_DrawHandlePic
        //
        private void UI_DrawHandlePic(float x, float y, float w, float h, idMaterial shader)
        {
            float s0;
            float s1;
            float t0;
            float t1;

            if (w == 0 && h == 0)
            {
                return;
            }

            if (w < 0)
            {   // flip about vertical
                w = -w;
                s0 = 1;
                s1 = 0;
            }
            else
            {
                s0 = 0;
                s1 = 1;
            }

            if (h < 0)
            {   // flip about horizontal
                h = -h;
                t0 = 1;
                t1 = 0;
            }
            else
            {
                t0 = 0;
                t1 = 1;
            }

            Engine.RenderSystem.DrawStretchPic(x, y, w, h, s0, t0, s1, t1, shader);
        }

        //
        // UI_DrawHandlePic
        //
        private void UI_DrawHandlePic(float x, float y, float w, float h, idImage image)
        {
            if (w == 0 && h == 0)
            {
                return;
            }
            Engine.RenderSystem.DrawStrechPic((int)x, (int)y, (int)w, (int)h, image);
        }

        //
        // Draw
        //
        public override void Draw()
        {
            // draw the background if necessary
            if (menu.fullScreen == true && menu.window.backgroundHandle != null)
            {
                UI_DrawHandlePic(0, 0, Engine.RenderSystem.GetViewportWidth(), Engine.RenderSystem.GetViewportHeight(), menu.window.backgroundHandle);
            }

            // paint the background and or border
            PaintWindow(ref menu.window);

            for (int i = 0; i < menu.itemCount; i++)
            {
                PaintItem(ref menu.items[i]);
            }
        }
    }
}
