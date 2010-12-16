// UI_text.cs (c) 2010 JV Software
//

using System;
using idLib.Math;
using idLib.Engine.Public;
using idLib.Engine.Content.ui;
using idLib.Engine.Content.ui.Private;

namespace ui
{
    //
    // idText
    //
    public static class idText
    {
        //
        // SetColor
        //
        private static void SetColor(idVector4 color)
        {
            Engine.RenderSystem.SetColor(color.X, color.Y, color.Z, color.W);
        }

        //
        // Text_PaintChar
        //
        private static void Text_PaintChar(float x, float y, float width, float height, int font, float scale, float s, float t, float s2, float t2, idMaterial hShader)
        {
            float w, h;
            w = width * scale;
            h = height * scale;
            Engine.RenderSystem.DrawStretchPic(x, y, w, h, s, t, s2, t2, hShader);
        }

        //
        // ToWindowCoords
        //
        private static void ToWindowCoords(ref float x, ref float y, idUserInterfaceWindow window)
        {
            if (window.border != 0)
            {
                x += window.borderSize;
                y += window.borderSize;
            }

            x += window.rect.x;
            y += window.rect.y;
        }

        //
        // Text_Width
        //
        private static int Text_Width(string text, idFont fnt, float scale, int limit)
        {
	        int count,len;
	        float outlen;
	        glyphInfo_t glyph;
	        float useScale;

            useScale = scale * fnt.glyphScale;
	        outlen = 0;
	        if ( text != null && text.Length > 0 ) {
		        len = text.Length;
		        if ( limit > 0 && len > limit ) {
			        len = limit;
		        }
		        count = 0;
		        while ( count < len ) {
			        if ( idColor.IsColorString( text, count ) ) {
				        count += 2;
				        continue;
			        } else {
				        glyph = fnt.glyphs[text[count]];
				        outlen += glyph.xSkip;
				        count++;
			        }
		        }
	        }
	        return (int)(outlen * useScale);
        }

        //
        // SetTextExtents
        //
        public static void SetTextExtents( ref idUserInterfaceItem item, idFont font, ref int width, ref int height, string text ) {
	        string textPtr = ( text.Length > 0 ) ? text : item.text;

	        if ( textPtr == null || textPtr.Length <= 0 ) {
		        return;
	        }

	        width = (int)item.textRect.w;
	        height = (int)item.textRect.h;

	        // keeps us from computing the widths and heights more than once
	        if ( width == 0 || ( item.type == ui_menudef.ITEM_TYPE_OWNERDRAW && item.textalignment == ui_menudef.ITEM_ALIGN_CENTER ) ) {
                int originalWidth = Text_Width(item.text, font, item.textscale, 0);

		        if ( item.type == ui_menudef.ITEM_TYPE_OWNERDRAW && ( item.textalignment == ui_menudef.ITEM_ALIGN_CENTER || item.textalignment == ui_menudef.ITEM_ALIGN_RIGHT ) ) {
                    originalWidth += Text_Width(item.text, font, item.textscale, 0); //DC->ownerDrawWidth(item->window.ownerDraw, item->font, item->textscale);

		        } 
                /*
                else if ( ( item.type == ui_menudef.ITEM_TYPE_EDITFIELD || item.type == ui_menudef.ITEM_TYPE_VALIDFILEFIELD ) && item.textalignment == ui_menudef.ITEM_ALIGN_CENTER && item->cvar ) {
			        char buff[256];
			        DC->getCVarString( item->cvar, buff, 256 );
			        originalWidth += DC->textWidth( buff, item->font, item->textscale, 0 );
		        }
                */

                width = Text_Width(textPtr, font, item.textscale, 0);
                height = Text_Width(textPtr, font, item.textscale, 0);
		        item.textRect.w = width;
		        item.textRect.h = height;
		        item.textRect.x = item.textalignx;
		        item.textRect.y = item.textaligny;
                if (item.textalignment == ui_menudef.ITEM_ALIGN_RIGHT)
                {
			        item.textRect.x = item.textalignx - originalWidth;
                }
                else if (item.textalignment == ui_menudef.ITEM_ALIGN_CENTER)
                {
			        item.textRect.x = item.textalignx - originalWidth / 2;
		        }

                ToWindowCoords(ref item.textRect.v.X, ref item.textRect.v.Y, item.window);
	        }
        }

        //
        // PaintText
        //
        public static void PaintText(idUserInterfaceLocal ui, float x, float y, int font, float scale, idVector4 color, string text, float adjust, int limit, int style)
        {
            idFont fnt;
            float useScale;
            glyphInfo_t glyph;

            fnt = ui.GetFontForSize(font, scale);

            useScale = scale * fnt.glyphScale;
            if (text != null && text.Length > 0)
            {
                SetColor(color);
                int len = (int)text.Length;
                int count = 0;
		        if ( limit > 0 && len > limit ) {
			        len = limit;
		        }
		        while ( count < len ) {
			        glyph = fnt.glyphs[text[count]];
			        //int yadj = Assets.textFont.glyphs[text[i]].bottom + Assets.textFont.glyphs[text[i]].top;
			        //float yadj = scale * (Assets.textFont.glyphs[text[i]].imageHeight - Assets.textFont.glyphs[text[i]].height);
			        if ( idColor.IsColorString( text, count ) ) {
                        SetColor( idColor.g_color_table[ idColor.ColorIndex( (char)(text[count] + 1) )]);
				        count += 2;
				        continue;
			        } else {
				        float yadj = useScale * glyph.top;
                        if (style == ui_menudef.ITEM_TEXTSTYLE_SHADOWED || style == ui_menudef.ITEM_TEXTSTYLE_SHADOWEDMORE)
                        {
                            /*
                            int ofs = style == ui_menudef.ITEM_TEXTSTYLE_SHADOWED ? 1 : 2;
					        colorBlack[3] = newColor[3];
					        trap_R_SetColor( colorBlack );
					        Text_PaintChar( x + ofs, y - yadj + ofs,
									        glyph->imageWidth,
									        glyph->imageHeight,
									        font,
									        useScale,
									        glyph->s,
									        glyph->t,
									        glyph->s2,
									        glyph->t2,
									        glyph->glyph );
					        trap_R_SetColor( newColor );
					        colorBlack[3] = 1.0;
                            */
				        }
				        Text_PaintChar( x, y - yadj,
								        glyph.imageWidth,
								        glyph.imageHeight,
								        font,
								        useScale,
								        glyph.s,
								        glyph.t,
								        glyph.s2,
								        glyph.t2,
								        glyph.glyph );

				        x += ( glyph.xSkip * useScale ) + adjust;
				        count++;
			        }
		        }
                Engine.RenderSystem.SetColor(1, 1, 1, 1);
            }
        }
    }
}
