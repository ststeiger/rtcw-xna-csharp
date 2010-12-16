﻿// UI_main.cs (c) 2010 JV Software
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
        private idWorld world;
        private bool isOpen = false;
        private idUserInterfaceCommandHandler cmd = new idUserInterfaceCommandHandler();

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
            world = Engine.RenderSystem.AllocWorld();
        }

        //
        // GetFontForSize
        //
        public idFont GetFontForSize(int font, float scale)
        {
            if (font == ui_menudef.UI_FONT_DEFAULT)
            {
                if (scale <= idUserInterfaceManagerLocal.ui_smallFont.GetValueFloat())
                {
                    return assets.handles.smallFont;
                }
                else if (scale > idUserInterfaceManagerLocal.ui_bigFont.GetValueFloat())
                {
                    return assets.handles.bigFont;
                }
            }
            else if (font == ui_menudef.UI_FONT_BIG)
            {
                return assets.handles.bigFont;
            }
            else if (font == ui_menudef.UI_FONT_SMALL)
            {
                return assets.handles.smallFont;
            }
            else if (font == ui_menudef.UI_FONT_HANDWRITING)
            {
                return assets.handles.handwritingFont;
            }

            return assets.handles.textFont;
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
        // Item_UpdatePosition
        //
        private void Item_UpdatePosition(ref idUserInterfaceItem item)
        {
            float x, y;

            x = menu.window.rect.x;
            y = menu.window.rect.y;

            if (menu.window.border != 0)
            {
                x += menu.window.borderSize;
                y += menu.window.borderSize;
            }

            Item_SetScreenCoords(ref item, x, y);

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
        // AdjustFrom640
        //
        public static void AdjustFrom640(ref float x, ref float y, ref float w, ref float h)
        {
            float yscale = Engine.RenderSystem.GetViewportHeight() * (1.0f / 480.0f);
            float xscale = Engine.RenderSystem.GetViewportWidth() * (1.0f / 640.0f);

            //*x = *x * DC->scale + DC->bias;
            x *= xscale;
            y *= yscale;
            w *= xscale;
            h *= yscale;
        }

        //
        // Proejct2DCoordinates
        //
        private void Project2DCoordinates( ref idVector3 org, int x, int y, int width, int height )
        {
            int posx, posy;
            idVector3 normCoords;

            normCoords.X = org.X;
            normCoords.Y = org.Z;
            normCoords.Z = 0;

            normCoords.Normalize();

            posx = (int)((normCoords.X) * (width / 2) + x);
            posy = (int)((normCoords.Y) * (height / 2) + y);

          //  org.X += posx;
          //  org.Z += posy;
        }

        //
        // Item_TextColor
        //
        private void Item_TextColor(ref idUserInterfaceItem item, out idVector4 newColor)
        {
            newColor = item.window.foreColor;
#if false
            vec4_t lowLight;
            menuDef_t* parent = (menuDef_t*)item->parent;

            Fade(&item->window.flags, &item->window.foreColor[3], parent->fadeClamp, &item->window.nextTime, parent->fadeCycle, qtrue, parent->fadeAmount);

            if (item->window.flags & WINDOW_HASFOCUS)
            {
                lowLight[0] = 0.8 * parent->focusColor[0];
                lowLight[1] = 0.8 * parent->focusColor[1];
                lowLight[2] = 0.8 * parent->focusColor[2];
                lowLight[3] = 0.8 * parent->focusColor[3];
                LerpColor(parent->focusColor, lowLight, *newColor, 0.5 + 0.5 * sin(DC->realTime / PULSE_DIVISOR));
            }
            else if (item->textStyle == ITEM_TEXTSTYLE_BLINK && !((DC->realTime / BLINK_DIVISOR) & 1))
            {
                lowLight[0] = 0.8 * item->window.foreColor[0];
                lowLight[1] = 0.8 * item->window.foreColor[1];
                lowLight[2] = 0.8 * item->window.foreColor[2];
                lowLight[3] = 0.8 * item->window.foreColor[3];
                LerpColor(item->window.foreColor, lowLight, *newColor, 0.5 + 0.5 * sin(DC->realTime / PULSE_DIVISOR));
            }
            else
            {
                newColor = item.window.foreColor;
                // items can be enabled and disabled based on cvars
            }

            if (item->enableCvar && *item->enableCvar && item->cvarTest && *item->cvarTest)
            {
                if (item->cvarFlags & (CVAR_ENABLE | CVAR_DISABLE) && !Item_EnableShowViaCvar(item, CVAR_ENABLE))
                {
                    memcpy(newColor, &parent->disableColor, sizeof(vec4_t));
                }
            }
#endif
        }

        //
        // PaintItemText
        //
        private void PaintItemText(ref idUserInterfaceItem item)
        {
            idVector4 color;
            int width = 0;
            int height = 0;

            if (item.text.Length <= 0)
            {
                return;
            }
            if (item.font.Length <= 0)
            {
                item.font = "0"; // default.
            }


            idText.SetTextExtents(ref item, GetFontForSize(int.Parse(item.font), item.textscale), ref width, ref height, item.text);
            Item_TextColor(ref item, out color);

            idText.PaintText(this, item.textRect.x, item.textRect.y, int.Parse(item.font), item.textscale, color, item.text, 0, 0, item.textStyle);
        }

        //
        // PaintItemModel
        //
        float posTest = 0;
        private void PaintItemModel(ref idUserInterfaceItem item)
        {
            float x, y, w, h;   //,xx;
	        idRefdef refdef;
	        idRenderEntity ent;
	        idVector3 mins, maxs, origin;
            idVector3 angles;
	        idUserInterfaceModel modelPtr = (idUserInterfaceModel)item.typeData;
	        int backLerpWhole;

	        if ( modelPtr == null ) {
		        return;
	        }

	        if ( item.model == null ) {
		        return;
	        }

	        // setup the refdef
	        //memset( &refdef, 0, sizeof( refdef ) );
            refdef = world.AllocRefdef();

	        refdef.rdflags = idRenderType.RDF_NOWORLDMODEL;
            refdef.viewaxis = idVector3.vector_origin.ToAxis();
            
	        //AxisClear( refdef.viewaxis );
	        x = item.window.rect.x + 1;
	        y = item.window.rect.y + 1;
	        w = item.window.rect.w - 2;
	        h = item.window.rect.h - 2;

           // refdef.vieworg[0] = x;
           

	        AdjustFrom640( ref x, ref y, ref w, ref h );

            refdef.vieworg.Y = (x / 6);
            refdef.vieworg.Z = (y / 10);

            posTest++;
            if (posTest > 100)
            {
                posTest = 0;
            }
            
          //  refdef.vieworg[1] = y;
          //  refdef.vieworg[2] = -50;

	        refdef.x = (int)x;
	        refdef.y = (int)y;
	        refdef.width = (int)w;
	        refdef.height = (int)h;

            item.model.GetModelBounds( out mins, out maxs );


            ent = world.AllocRenderEntity(ref refdef);
            ent.hModel = item.model;
	        ent.origin[2] = -0.5f * ( mins[2] + maxs[2] );
            ent.origin[1] = 0.5f * (mins[1] + maxs[1]);

	        // calculate distance so the model nearly fills the box
	        if ( true ) {
		        float len = 0.5f * ( maxs[2] - mins[2] );
                ent.origin[0] = len / 0.268f;    // len / tan( fov/2 )
		        //origin[0] = len / tan(w/2);
	        } else {
                ent.origin[0] = item.textscale;
	        }

            //Project2DCoordinates(ref ent.origin, refdef.x, refdef.y, refdef.width, refdef.height);

        #if true
	        refdef.fov_x = ( modelPtr.fov_x != 0 ) ? modelPtr.fov_x : w;
            refdef.fov_y = (modelPtr.fov_y != 0) ? modelPtr.fov_y : h;
        #else
	        refdef.fov_x = (int)( (float)refdef.width / 640.0f * 90.0f );
	        xx = refdef.width / tan( refdef.fov_x / 360 * M_PI );
	        refdef.fov_y = atan2( refdef.height, xx );
	        refdef.fov_y *= ( 360 / M_PI );
        #endif
	       // DC->clearScene();

	        //refdef.time = DC->realTime;

	        // add the model

	        //memset( &ent, 0, sizeof( ent ) );
            

	        //adjust = 5.0 * sin( (float)uis.realtime / 500 );
	        //adjust = 360 % (int)((float)uis.realtime / 1000);
	        //VectorSet( angles, 0, 0, 1 );

	        // use item storage to track
	       // if ( modelPtr->rotationSpeed ) {
		  //      if ( DC->realTime > item->window.nextTime ) {
			//        item->window.nextTime = DC->realTime + modelPtr->rotationSpeed;
			//        modelPtr.angle = (int)( modelPtr.angle + 1 ) % 360;
		   //     }
	      //  }
            angles = new idVector3(0, modelPtr.angle, 0);
            ent.axis = angles.ToAxis();

            if (modelPtr.frame < item.model.GetNumFrames()-1)
            {
                if (modelPtr.backlerp < 1)
                {
                    modelPtr.backlerp += 0.5f;
                }
                else
                {
                    modelPtr.frame++;
                    modelPtr.backlerp = 0;
                }
            }
            else
            {
                modelPtr.frame = 0;
            }

            /*
	        if ( modelPtr->frameTime ) { // don't advance on the first frame
		        modelPtr->backlerp += ( ( ( DC->realTime - modelPtr->frameTime ) / 1000.0f ) * (float)modelPtr->fps );
	        }

	        if ( modelPtr->backlerp > 1 ) {
		        backLerpWhole = floor( modelPtr->backlerp );

		        modelPtr->frame += ( backLerpWhole );
		        if ( ( modelPtr->frame - modelPtr->startframe ) > modelPtr->numframes ) {
			        modelPtr->frame = modelPtr->startframe + modelPtr->frame % modelPtr->numframes; // todo: ignoring loopframes

		        }
		        modelPtr->oldframe += ( backLerpWhole );
		        if ( ( modelPtr->oldframe - modelPtr->startframe ) > modelPtr->numframes ) {
			        modelPtr->oldframe = modelPtr->startframe + modelPtr->oldframe % modelPtr->numframes;   // todo: ignoring loopframes

		        }
		        modelPtr->backlerp = modelPtr->backlerp - backLerpWhole;
	        }

	        modelPtr->frameTime = DC->realTime;
            */
	        ent.frame       = modelPtr.frame;
	        ent.oldframe    = modelPtr.oldframe;
	        ent.backlerp    = 1.0f - modelPtr.backlerp;

            ent.lightingOrigin = ent.origin;
            ent.oldorigin = ent.origin;

            world.RenderScene(refdef);
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

            // Paint the window first.
            PaintWindow(ref item.window);


            switch (item.type)
            {
                case ui_menudef.ITEM_TYPE_MENUMODEL:
                    Item_UpdatePosition(ref item);
                    PaintItemModel(ref item);
                    break;
                case ui_menudef.ITEM_TYPE_MODEL:
                    Item_UpdatePosition(ref item);
                    PaintItemModel(ref item);
                    break;

                case ui_menudef.ITEM_TYPE_TEXT:
                  //  Item_UpdatePosition(ref item);
                    PaintItemText(ref item);
                    break;
                case ui_menudef.ITEM_TYPE_BUTTON:
                    Item_UpdatePosition(ref item);
                    PaintItemText(ref item);
                    break;
            }
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
        // OpenMenuEvents
        //
        private void OpenMenuEvents()
        {
            cmd.Execute(ref menu, menu.onOpen);

            for (int i = 0; i < menu.itemCount; i++)
            {
                //cmd.Execute(ref menu.items[i], menu.items[i].on
            }
        }

        //
        // Draw
        //
        public override void Draw()
        {
            if (isOpen == false)
            {
                OpenMenuEvents();
                isOpen = true;
            }
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
