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
        private idWorld world;
        private bool isOpen = false;
        private idUserInterfaceCommandHandler cmd = new idUserInterfaceCommandHandler();
        private int cursorPosX = 0;
        private int cursorPosY = 0;
        private idUserInterfaceLocal parentWindow;
        private idUserInterfaceLocal childWindow;

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
            idFont fnt = null;
            if (font == ui_menudef.UI_FONT_DEFAULT)
            {
                if (scale <= idUserInterfaceManagerLocal.ui_smallFont.GetValueFloat())
                {
                    fnt = assets.handles.smallFont;
                }
                else if (scale > idUserInterfaceManagerLocal.ui_bigFont.GetValueFloat())
                {
                    fnt = assets.handles.bigFont;
                }
            }
            else if (font == ui_menudef.UI_FONT_BIG)
            {
                fnt = assets.handles.bigFont;
            }
            else if (font == ui_menudef.UI_FONT_SMALL)
            {
                fnt = assets.handles.smallFont;
            }
            else if (font == ui_menudef.UI_FONT_HANDWRITING)
            {
                fnt = assets.handles.handwritingFont;
            }

            fnt = assets.handles.textFont;

            if (fnt == null)
            {
                fnt = Engine.RenderSystem.RegisterFont("default", 24);
            }

            return fnt;
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
#if false // disabled doesn't work on the phone
            if (AssetStringValid(name))
            {
                video = Engine.RenderSystem.LoadVideo(name);
                return;
            }
#endif
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
                menu.items[i].parentUI = this;
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
            idUserInterfaceRectangle fillRect = new idUserInterfaceRectangle();

            if (window == null || (window.style == 0 && window.border == 0))
            {
                return;
            }

            fillRect.x = window.rect.x;
            fillRect.y = window.rect.y;
            fillRect.w = window.rect.w;
            fillRect.h = window.rect.h;

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
                if (window.cinematic != 1 && window.cinematicHandle != null)
                {
                    // jv - hack
                    fillRect.x = window.rectClient.x;
                    fillRect.y = window.rectClient.y;
                    fillRect.w = window.rectClient.w;
                    fillRect.h = window.rectClient.h;

                    AdjustFrom640(ref fillRect.v.X, ref fillRect.v.Y, ref fillRect.v.Z, ref fillRect.v.W);

                    window.cinematicHandle.SetLooping(true);
                    window.cinematicHandle.SetExtents((int)fillRect.x, (int)fillRect.y, (int)fillRect.w, (int)fillRect.h);
                    window.cinematic = 1;
                }

                if (window.cinematicHandle != null)
                {
                    window.cinematicHandle.DrawCinematic();
                }
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
        // Rect_ContainsPoint
        //
        private bool Rect_ContainsPoint(idUserInterfaceRectangle rect, float x, float y)
        {
            if (rect != null)
            {
                if (x > rect.x && x < rect.x + rect.w && y > rect.y && y < rect.y + rect.h)
                {
                    return true;
                }
            }
            return false;
        }

        //
        // Item_CorrectedTextRect
        //
        idUserInterfaceRectangle itemCorrectedRect = new idUserInterfaceRectangle();
        private idUserInterfaceRectangle Item_CorrectedTextRect( idUserInterfaceItem item ) {
	        if ( item != null ) {
		        itemCorrectedRect = item.textRect;
		        if ( itemCorrectedRect.w != 0 ) {
			        itemCorrectedRect.y -= itemCorrectedRect.h;
		        }
	        }
	        return itemCorrectedRect;
        }

        //
        // IsVisible
        //
        private bool IsVisible( int flags ) {
            return ((flags & ui_globals.WINDOW_VISIBLE) != 0 && (flags & ui_globals.WINDOW_FADINGOUT) == 0);
        }

        //
        // Item_MouseEnter
        //
        private void Item_MouseEnter(ref idUserInterfaceItem item, float x, float y)
        {
            idUserInterfaceRectangle r;
            if (item != null)
            {
                r = item.textRect;
                r.y -= r.h;
                // in the text rect?
                /*
                // items can be enabled and disabled based on cvars
                if (item->cvarFlags & (CVAR_ENABLE | CVAR_DISABLE) && !Item_EnableShowViaCvar(item, CVAR_ENABLE))
                {
                    return;
                }

                if (item->cvarFlags & (CVAR_SHOW | CVAR_HIDE) && !Item_EnableShowViaCvar(item, CVAR_SHOW))
                {
                    return;
                }
                */

                if (Rect_ContainsPoint(r, x, y))
                {
                    if ((item.window.flags & ui_globals.WINDOW_MOUSEOVERTEXT) == 0)
                    {
                        cmd.Execute(ref item, item.mouseEnterText);
                        item.window.flags |= ui_globals.WINDOW_MOUSEOVERTEXT;
                    }
                    if ((item.window.flags & ui_globals.WINDOW_MOUSEOVER) == 0)
                    {
                        cmd.Execute(ref item, item.mouseEnter);
                        item.window.flags |= ui_globals.WINDOW_MOUSEOVER;
                    }

                }
                else
                {
                    // not in the text rect
                    if ((item.window.flags & ui_globals.WINDOW_MOUSEOVERTEXT) != 0)
                    {
                        // if we were
                        cmd.Execute(ref item, item.mouseExitText);
                        item.window.flags &= ~ui_globals.WINDOW_MOUSEOVERTEXT;
                    }
                    if ((item.window.flags & ui_globals.WINDOW_MOUSEOVER) == 0)
                    {
                        cmd.Execute(ref item, item.mouseEnter);
                        item.window.flags |= ui_globals.WINDOW_MOUSEOVER;
                    }

                    if (item.type == ui_menudef.ITEM_TYPE_LISTBOX)
                    {
                       // Item_ListBox_MouseEnter(item, x, y);
                    }
                }
            }
        }

        //
        // Item_MouseLeave
        //
        private void Item_MouseLeave(ref idUserInterfaceItem item)
        {
            if (item != null)
            {
                if ((item.window.flags & ui_globals.WINDOW_MOUSEOVERTEXT) != 0)
                {
                    cmd.Execute(ref item, item.mouseExitText);
                    item.window.flags &= ~ui_globals.WINDOW_MOUSEOVERTEXT;
                }
                cmd.Execute(ref item, item.mouseExit);
                item.window.flags &= ~(ui_globals.WINDOW_LB_RIGHTARROW | ui_globals.WINDOW_LB_LEFTARROW);
            }
        }

        //
        // Item_SetMouseOver
        //
        private void Item_SetMouseOver(ref idUserInterfaceItem item, bool focus)
        {
            if (item != null)
            {
                if (focus)
                {
                    item.window.flags |= ui_globals.WINDOW_MOUSEOVER;
                }
                else
                {
                    item.window.flags &= ~ui_globals.WINDOW_MOUSEOVER;
                }
            }
        }

        //
        // ClearFocusFromUI
        //
        private idUserInterfaceItem ClearFocusFromUI()
        {
            int i;
            idUserInterfaceItem ret = null;


            for (i = 0; i < menu.itemCount; i++)
            {
                if ((menu.items[i].window.flags & ui_globals.WINDOW_HASFOCUS) != 0)
                {
                    ret = menu.items[i];
                }
                menu.items[i].window.flags &= ~ui_globals.WINDOW_HASFOCUS;
                if (menu.items[i].leaveFocus.Length > 0)
                {
                    cmd.Execute(ref menu.items[i], menu.items[i].leaveFocus);
                }
            }

            return ret;
        }

        //
        // Item_SetFocus
        //
        private bool Item_SetFocus(ref idUserInterfaceItem item, float x, float y)
        {
            int i;
            idUserInterfaceItem oldFocus;
            idSound sfx = assets.handles.itemFocusSound;
            bool playSound = false;
            idUserInterfaceMenuDef parent;
            // sanity check, non-null, not a decoration and does not already have the focus
            if (item == null || (item.window.flags & ui_globals.WINDOW_DECORATION) != 0 || (item.window.flags & ui_globals.WINDOW_HASFOCUS) != 0 || (item.window.flags & ui_globals.WINDOW_VISIBLE) == 0)
            {
                return false;
            }

            parent = item.parent;

            /*
            // items can be enabled and disabled based on cvars
            if (item->cvarFlags & (CVAR_ENABLE | CVAR_DISABLE) && !Item_EnableShowViaCvar(item, CVAR_ENABLE))
            {
                return qfalse;
            }

            if (item->cvarFlags & (CVAR_SHOW | CVAR_HIDE) && !Item_EnableShowViaCvar(item, CVAR_SHOW))
            {
                return qfalse;
            }
            */

            oldFocus = ClearFocusFromUI();

            if (item.type == ui_menudef.ITEM_TYPE_TEXT)
            {
                idUserInterfaceRectangle r;
                r = item.textRect;
                r.y -= r.h;
                if (Rect_ContainsPoint(r, x, y))
                {
                    item.window.flags |= ui_globals.WINDOW_HASFOCUS;
                    if (item.focusSnd != null)
                    {
                        sfx = item.focusSnd;
                    }
                    playSound = true;
                }
                else
                {
                    if (oldFocus != null)
                    {
                        oldFocus.window.flags |= ui_globals.WINDOW_HASFOCUS;
                        if (oldFocus.onFocus.Length > 0)
                        {
                            cmd.Execute(ref oldFocus, oldFocus.onFocus);
                        }
                    }
                }
            }
            else
            {
                item.window.flags |= ui_globals.WINDOW_HASFOCUS;
                if (item.onFocus.Length > 0)
                {
                    cmd.Execute(ref item, item.onFocus);                    
                }
                if (item.focusSnd != null)
                {
                    sfx = item.focusSnd;
                }
                playSound = true;
            }

            if (playSound && sfx != null)
            {
                sfx.Play();
            }

            for (i = 0; i < parent.itemCount; i++)
            {
                if (parent.items[i] == item)
                {
                    parent.cursorItem = i;
                    break;
                }
            }

            return true;
        }

        //
        // OpenUI
        //
        public void OpenUI(string name)
        {
            if (parentWindow != null && parentWindow.menu.window.name == name )
            {
                return;
            }

            if (parentWindow != null)
            {
                parentWindow.childWindow = this;
            }

            childWindow = (idUserInterfaceLocal)Engine.ui.FindUserInterface(name);
            if (childWindow == null)
            {
                Engine.common.Warning("UI_OpenUI: Failed to find UI " + name + "\n");
                return;
            }

            if (childWindow.assets.handles.cursor == null)
            {
                childWindow.assets.handles.cursor = assets.handles.cursor;
            }

            childWindow.parentWindow = this;
        }

        //
        // CloseUI
        //
        public void CloseUI()
        {
            if (parentWindow == null)
            {
                Engine.common.Warning("UI_OpenUI: Tried to close a parent window\n");
                return;
            }
            parentWindow.CloseChild();
           // parentWindow = null;
        }

        //
        // loseChild
        //
        private void CloseChild()
        {
            if (childWindow == null)
                return;

            childWindow.isOpen = false;
            childWindow = null;
        }

        //
        // HandleKeyEvent
        //
        public override void HandleKeyEvent(keyNum key, bool down)
        {
            idUserInterfaceItem item = null;

            if (childWindow != null)
            {
                childWindow.HandleKeyEvent(key, down);
                return;
            }

            if (isOpen == false)
                return;

            if (down == false)
                return;

            if (key == keyNum.K_MOUSE1 || key == keyNum.K_MOUSE2 || key == keyNum.K_MOUSE3)
            {
                // get the item with focus
                for (int i = 0; i < menu.itemCount; i++)
                {
                    if ((menu.items[i].window.flags & ui_globals.WINDOW_HASFOCUS) != 0)
                    {
                        item = menu.items[i];
                    }
                }
            }

            if (item != null)
            {
                cmd.Execute(ref item, item.action);
            }
        }

        //
        // HandleMouseEvent
        //
        public override void HandleMouseEvent(int dx, int dy)
        {
            bool focusSet = false;

            if (childWindow != null)
            {
                childWindow.HandleMouseEvent(dx, dy);
                return;
            }
#if WINDOWS_PHONE
            cursorPosX = dx;
            cursorPosY = dy;
#else
            cursorPosX += dx;
            cursorPosY += dy;
#endif
            if (cursorPosX < 0)
            {
                cursorPosX = 0;
            }
            else if (cursorPosX > 640)
            {
                cursorPosX = 640;
            }

            if (cursorPosY < 0)
            {
                cursorPosY = 0;
            }
            else if (cursorPosY > 480)
            {
                cursorPosY = 480;
            }

            // FIXME: this is the whole issue of focus vs. mouse over..
	        // need a better overall solution as i don't like going through everything twice
            for (int pass = 0; pass < 2; pass++)
            {
                for (int i = 0; i < menu.itemCount; i++)
                {
                    if (Rect_ContainsPoint(menu.items[i].window.rect, cursorPosX, cursorPosY))
                    {
                        if (pass == 1)
                        {
                            idUserInterfaceItem overItem = menu.items[i];
                            if (overItem.type == ui_menudef.ITEM_TYPE_TEXT && (overItem.text != null && overItem.text.Length > 0))
                            {
                                if (!Rect_ContainsPoint(Item_CorrectedTextRect(overItem), cursorPosX, cursorPosY))
                                {
                                    continue;
                                }
                            }
                            // if we are over an item
                            if (IsVisible(overItem.window.flags))
                            {
                                // different one
                                Item_MouseEnter(ref overItem, cursorPosX, cursorPosY);
                                // Item_SetMouseOver(overItem, qtrue);

                                // if item is not a decoration see if it can take focus
                                if (!focusSet)
                                {
                                    focusSet = Item_SetFocus(ref overItem, cursorPosX, cursorPosY);
                                }
                            }
                        }
                    }
                    else if ((menu.items[i].window.flags & ui_globals.WINDOW_MOUSEOVER) != 0)
                    {
                        Item_MouseLeave(ref menu.items[i]);
                        Item_SetMouseOver(ref menu.items[i], false);
                    }
                }
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

        /*
        ==============
        CG_HorizontalPercentBar
	        Generic routine for pretty much all status indicators that show a fractional
	        value to the palyer by virtue of how full a drawn box is.

        flags:
	        left		- 1
	        center		- 2		// direction is 'right' by default and orientation is 'horizontal'
	        vert		- 4
	        nohudalpha	- 8		// don't adjust bar's alpha value by the cg_hudalpha value
	        bg			- 16	// background contrast box (bg set with bgColor of 'NULL' means use default bg color (1,1,1,0.25)
	        spacing		- 32	// some bars use different sorts of spacing when drawing both an inner and outer box

	        lerp color	- 256	// use an average of the start and end colors to set the fill color
        ==============
        */


        // TODO: these flags will be shared, but it was easier to work on stuff if I wasn't changing header files a lot
        private const int BAR_LEFT       = 0x0001;
        private const int BAR_CENTER     = 0x0002;
        private const int BAR_VERT       = 0x0004;
        private const int BAR_NOHUDALPHA = 0x0008;
        private const int BAR_BG         = 0x0010;
        // different spacing modes for use w/ BAR_BG
        private const int BAR_BGSPACING_X0Y5 = 0x0020;
        private const int BAR_BGSPACING_X0Y0 = 0x0040;

        private const int  BAR_LERP_COLOR = 0x0100;

        private const int BAR_BORDERSIZE = 2;

        private idVector4 backgroundcolor = new idVector4(1, 1, 1, 0.25f);  // colorAtPos is the lerped color if necessary
        private void FilledBar(float x, float y, float w, float h, idVector4 startColorIn, idVector4 endColor, idVector4 bgColor, float frac, int flags)
        {
            idVector4 colorAtPos;
            idVector4 startColor;
            idVector4 backColor;

	        int indent = BAR_BORDERSIZE;

            AdjustFrom640(ref x, ref y, ref w, ref h);

            startColor = startColorIn;

            if ((flags & BAR_BG) != 0)
            { // BAR_BG set, and color specified, use specified bg color
                backColor = bgColor;
            }
            else
            {
                backColor = backgroundcolor;
            }

	        // hud alpha
	        if ( ( flags & BAR_NOHUDALPHA ) != 0 ) {
                /*
		        startColor[3] *= cg_hudAlpha.value;
		        if ( endColor ) {
			        endColor[3] *= cg_hudAlpha.value;
		        }
		        if ( backgroundcolor ) {
			        backgroundcolor[3] *= cg_hudAlpha.value;
		        }
                */
	        }

	        if ( (flags & BAR_LERP_COLOR) != 0 ) {
		      //  Vector4Average( startColor, endColor, frac, colorAtPos );
	        }

	        // background
	        if ( ( flags & BAR_BG ) != 0 ) {
		        // draw background at full size and shrink the remaining box to fit inside with a border.  (alternate border may be specified by a BAR_BGSPACING_xx)
		        FillRect(      (int)x,
                               (int)y,
                               (int)w,
                               (int)h,
					           backgroundcolor );

		        if ( (flags & BAR_BGSPACING_X0Y0) != 0 ) {          // fill the whole box (no border)

		        } else if ( (flags & BAR_BGSPACING_X0Y5) != 0 ) {   // spacing created for weapon heat
			        indent *= 3;
			        y += indent;
			        h -= ( 2 * indent );

		        } else {                                // default spacing of 2 units on each side
			        x += indent;
			        y += indent;
			        w -= ( 2 * indent );
			        h -= ( 2 * indent );
		        }
	        }


	        // adjust for horiz/vertical and draw the fractional box
	        if ( (flags & BAR_VERT) != 0 ) {
		        if ( (flags & BAR_LEFT) != 0 ) {    // TODO: remember to swap colors on the ends here
			        y += ( h * ( 1 - frac ) );
		        } else if ( (flags & BAR_CENTER) != 0 ) {
			        y += ( h * ( 1 - frac ) / 2 );
		        }

		        if ( (flags & BAR_LERP_COLOR) != 0 ) {
                    FillRect((int)x, (int)y, (int)w, (int)(h * frac), startColor);
		        } else {
        //			CG_FillRectGradient ( x, y, w, h * frac, startColor, endColor, 0 );
                    FillRect((int)x, (int)y, (int)w, (int)(h * frac), startColor);
		        }

	        } else {

		        if ( (flags & BAR_LEFT) != 0 ) {    // TODO: remember to swap colors on the ends here
			        x += ( w * ( 1 - frac ) );
		        } else if ( (flags & BAR_CENTER) != 0 ) {
			        x += ( w * ( 1 - frac ) / 2 );
		        }

		        if ( (flags & BAR_LERP_COLOR) != 0 ) {
                    FillRect((int)x, (int)y, (int)(w * frac), (int)h, startColor);
		        } else {
        //			CG_FillRectGradient ( x, y, w * frac, h, startColor, endColor, 0 );
                    FillRect((int)x, (int)y, (int)(w * frac), (int)h, startColor);
		        }
	        }

        }


        /*
        =================
        CG_HorizontalPercentBar
        =================
        */
        private idVector4 percentBarBgColor = new idVector4(0.5f, 0.5f, 0.5f, 0.3f);
        private idVector4 percentBarColor = new idVector4(1.0f, 1.0f, 1.0f, 0.3f);
        public override void HorizontalPercentBar( float x, float y, float width, float height, float percent ) {
            FilledBar(x, y, width, height, percentBarBgColor, percentBarBgColor, percentBarColor, percent, BAR_BG | BAR_NOHUDALPHA);
        }

        //
        // SetItemVisible
        //
        public override void SetItemVisible(string name, bool visible)
        {
            for (int i = 0; i < menu.itemCount; i++)
            {
                if (menu.items[i].window.name == name)
                {
                    if (visible)
                    {
                        if (IsVisible(menu.items[i].window.flags) == false)
                        {
                            menu.items[i].window.flags |= ui_globals.WINDOW_VISIBLE;
                        }
                    }
                    else
                    {
                        if (IsVisible(menu.items[i].window.flags) == true)
                        {
                            menu.items[i].window.flags &= ~ui_globals.WINDOW_VISIBLE;
                        }
                    }
                    return;
                }
            }

            Engine.common.Warning("UI_SetItemVisible: Failed to find item " + name + " to toggle visibility\n");
        }

        //
        // Draw
        //
        public override void Draw()
        {
            // If the child window is fullscreen than don't draw this ui.
            if (childWindow != null && childWindow.menu.fullScreen == true)
            {
                childWindow.Draw();
                isOpen = false;
                return;
            }

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
                if (IsVisible(menu.items[i].window.flags))
                {
                    PaintItem(ref menu.items[i]);
                }
            }

            if (childWindow != null)
            {
                childWindow.Draw();
                return;
            }

            // Draw the cursor over everything else.
            UI_DrawHandlePic(cursorPosX - 16, cursorPosY - 16, 32, 32, assets.handles.cursor);
        }
    }
}
