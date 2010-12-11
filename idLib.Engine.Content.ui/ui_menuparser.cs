

using System;
using idLib.Math;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{

    class keywordHash_t
    {
        public delegate void func(ref idUserInterfaceItem item, ref idUserInterfaceFile ui);

        public string name;
        public func function;

        public keywordHash_t(string keyname, func function, object notUsed)
        {
            name = keyname;
            this.function = function;
        }
    };

    static class MenuParser
    {
        /*
      =================
      PC_Color_Parse
      =================
      */
        public static void ParseColor(ref idUserInterfaceFile ui, ref idVector4 c)
        {
            int i;
            float f;

            for (i = 0; i < 4; i++)
            {
                c[i] = float.Parse(ui.GetNextTokenFromLineChecked());
            }
        }

        public static idVector4 ParseColor(ref idUserInterfaceFile ui)
        {
            int i;
            float f;
            idVector4 c = new idVector4();

            for (i = 0; i < 4; i++)
            {
                c[i] = float.Parse(ui.GetNextTokenFromLineChecked());
            }

            return c;
        }

        /*
        =================
        PC_Rect_Parse
        =================
        */
        public static void ParseRect(ref idUserInterfaceFile ui, ref idUserInterfaceRectangle r)
        {
            r.x = float.Parse(ui.GetNextTokenFromLineChecked());
            r.y = float.Parse(ui.GetNextTokenFromLineChecked());
            r.w = float.Parse(ui.GetNextTokenFromLineChecked());
            r.h = float.Parse(ui.GetNextTokenFromLineChecked());
        }

        //
        // ParseMenu
        //
        public static void ParseMenu(ref idUserInterfaceMenuDef menudef, ref idUserInterfaceCachedAssets assets, ref idUserInterfaceFile ui)
        {
            bool foundKeyFunction = false;

            // Set the default values for the menudef.
            menudef.cursorItem = -1;
            menudef.fadeAmount = assets.fadeAmount;
            menudef.fadeClamp = assets.fadeClamp;
            menudef.fadeCycle = assets.fadeCycle;

            if (ui.ReachedEndOfBuffer)
                throw new Exception("EOF at start of menu");

            if (ui.NextToken != "{")
                throw new Exception("UI menu expected open bracket");

            while (true)
            {
                string token;

                if (ui.ReachedEndOfBuffer)
                    throw new Exception("EOF inside menu item");

                token = ui.NextToken;

                if (token == "}")
                {
                    break;
                }

                token = token.ToLower();

                foreach (keywordHash_t key in menuParseKeywords)
                {
                    if (key.name.ToLower() == token)
                    {
                        idUserInterfaceItem item = menudef;
                        key.function(ref item, ref ui);
                        menudef = (idUserInterfaceMenuDef)item;
                        foundKeyFunction = true;
                        break;
                    }
                }

                if (foundKeyFunction == false)
                {
                    throw new Exception("unknown menu keyword " + token);
                }
                foundKeyFunction = false;
            }
        }


        /*
        ===============
        Menu Keyword Parse functions
        ===============
        */

        public static void MenuParse_font(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.font = ui.GetNextTokenFromLineChecked();
            item = menu;
        }

        public static void MenuParse_name(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.name = ui.GetNextTokenFromLineChecked();
        }

        public static void MenuParse_fullscreen(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.fullScreen = (int.Parse(ui.GetNextTokenFromLineChecked()) != 0);
            item = menu;
        }

        public static void MenuParse_rect(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ParseRect(ref ui, ref item.window.rect);
        }

        public static void MenuParse_style(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.style = int.Parse(ui.GetNextTokenFromLineChecked());
        }

        public static void MenuParse_visible(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int isVisible = int.Parse(ui.GetNextTokenFromLineChecked());

            if (isVisible != 0)
            {
                item.window.flags |= ui_globals.WINDOW_VISIBLE;
            }
        }

        public static void MenuParse_onOpen(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.onOpen = ui.GetNextBracketedSection();
            item = menu;
        }

        public static void MenuParse_onClose(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.onClose = ui.GetNextBracketedSection();
            item = menu;
        }

        public static void MenuParse_onESC(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.onESC = ui.GetNextBracketedSection();
            item = menu;
        }

        //----(SA)	added
        public static void MenuParse_onROQDone(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.onROQDone = ui.GetNextBracketedSection();
            item = menu;
        }

        //----(SA)	end

        public static void MenuParse_border(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.border = int.Parse(ui.GetNextTokenFromLineChecked());
        }

        public static void MenuParse_borderSize(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.borderSize = float.Parse(ui.GetNextTokenFromLineChecked());
        }

        public static void MenuParse_backcolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                item.window.backColor[i] = float.Parse(ui.GetNextTokenFromLineChecked());
            }
        }

        public static void MenuParse_forecolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                item.window.foreColor[i] = float.Parse(ui.GetNextTokenFromLineChecked());
                item.window.flags |= ui_globals.WINDOW_FORECOLORSET;
            }
        }

        public static void MenuParse_bordercolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                item.window.borderColor[i] = float.Parse(ui.GetNextTokenFromLineChecked());
            }
        }

        public static void MenuParse_focuscolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i;
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            for (i = 0; i < 4; i++)
            {
                menu.focusColor[i] = float.Parse(ui.GetNextTokenFromLineChecked());
            }
            item = menu;
        }

        public static void MenuParse_disablecolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i;
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            for (i = 0; i < 4; i++)
            {
                menu.disableColor[i] = float.Parse(ui.GetNextTokenFromLineChecked());
            }
            item = menu;
        }


        public static void MenuParse_outlinecolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ParseColor(ref ui, ref item.window.outlineColor);
        }

        public static void MenuParse_background(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.background = ui.GetNextTokenFromLineChecked();
        }

        public static void MenuParse_cinematic(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.cinematicName = ui.GetNextTokenFromLineChecked();
        }

        public static void MenuParse_ownerdrawFlag(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i = int.Parse(ui.GetNextTokenFromLineChecked());

            item.window.ownerDrawFlags |= i;
        }

        public static void MenuParse_ownerdraw(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.ownerDraw = int.Parse(ui.GetNextTokenFromLineChecked());
        }


        // decoration
        public static void MenuParse_popup(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.flags |= ui_globals.WINDOW_POPUP;
        }


        public static void MenuParse_outOfBounds(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.flags |= ui_globals.WINDOW_OOB_CLICK;
        }

        public static void MenuParse_soundLoop(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.soundName = ui.GetNextTokenFromLineChecked();
            item = menu;
        }

        public static void MenuParse_fadeClamp(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.fadeClamp = float.Parse(ui.GetNextTokenFromLineChecked());
            item = menu;
        }

        public static void MenuParse_fadeAmount(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.fadeAmount = float.Parse(ui.GetNextTokenFromLineChecked());
            item = menu;
        }


        public static void MenuParse_fadeCycle(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.fadeCycle = int.Parse(ui.GetNextTokenFromLineChecked());
            item = menu;
        }




        // NERVE - SMF
        public static void MenuParse_execKey(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            char keyname;
            short keyindex;

            keyname = ui.GetNextTokenFromLineChecked()[0];
            keyindex = (short)keyname;

            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.onKey[keyindex] = ui.GetNextTokenFromLineChecked();
            item = menu;
        }

        public static void MenuParse_execKeyInt(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int keyname;

            keyname = int.Parse(ui.GetNextTokenFromLineChecked());

            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            menu.onKey[keyname] = ui.GetNextTokenFromLineChecked();
            item = menu;
        }

        //
        // MenuParse_itemDef
        //
        public static void MenuParse_itemDef(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceMenuDef menu = (idUserInterfaceMenuDef)item;
            if (menu.itemCount < ui_globals.MAX_MENUITEMS)
            {
                menu.items[menu.itemCount] = new idUserInterfaceItem();
                ItemParser.Item_Parse(ref menu.items[menu.itemCount], ref ui);
                menu.items[menu.itemCount++].parent = menu;
            }
            item = menu;
        }

        

        //
        // menuParseKeywords
        //
        public static keywordHash_t[] menuParseKeywords = new keywordHash_t[]{
	        new keywordHash_t("font", MenuParse_font, null),
	        new keywordHash_t("name", MenuParse_name, null),
	        new keywordHash_t("fullscreen", MenuParse_fullscreen, null),
	        new keywordHash_t("rect", MenuParse_rect, null),
	        new keywordHash_t("style", MenuParse_style, null),
	        new keywordHash_t("visible", MenuParse_visible, null),
	        new keywordHash_t("onopen", MenuParse_onOpen, null),
	        new keywordHash_t("onclose", MenuParse_onClose, null),
	        new keywordHash_t("onesc", MenuParse_onESC, null),
	        new keywordHash_t("onroqdone", MenuParse_onROQDone, null),    //----(SA)	added
	        new keywordHash_t("border", MenuParse_border, null),
	        new keywordHash_t("borderSize", MenuParse_borderSize, null),
	        new keywordHash_t("backcolor", MenuParse_backcolor, null),
	        new keywordHash_t("forecolor", MenuParse_forecolor, null),
	        new keywordHash_t("bordercolor", MenuParse_bordercolor, null),
	        new keywordHash_t("focuscolor", MenuParse_focuscolor, null),
	        new keywordHash_t("disablecolor", MenuParse_disablecolor, null),
	        new keywordHash_t("outlinecolor", MenuParse_outlinecolor, null),
	        new keywordHash_t("background", MenuParse_background, null),
	        new keywordHash_t("ownerdraw", MenuParse_ownerdraw, null),
	        new keywordHash_t("ownerdrawFlag", MenuParse_ownerdrawFlag, null),
	        new keywordHash_t("outofboundsclick", MenuParse_outOfBounds, null),
	        new keywordHash_t("soundloop", MenuParse_soundLoop, null),
	        new keywordHash_t("itemdef", MenuParse_itemDef, null),
	        new keywordHash_t("cinematic", MenuParse_cinematic, null),
	        new keywordHash_t("popup", MenuParse_popup, null),
	        new keywordHash_t("fadeclamp", MenuParse_fadeClamp, null),
	        new keywordHash_t("fadecycle", MenuParse_fadeCycle, null),
	        new keywordHash_t("fadeamount", MenuParse_fadeAmount, null),
	        new keywordHash_t("execKey", MenuParse_execKey, null),                // NERVE - SMF
	        new keywordHash_t("execKeyInt", MenuParse_execKeyInt, null),          // NERVE - SMF
        };
    }
}
