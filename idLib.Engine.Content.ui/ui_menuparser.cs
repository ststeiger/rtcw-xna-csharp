

using System;
using idLib.Math;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{



    static class MenuParser
    {
        class keywordHash_t
        {
            public delegate void func(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui);

            public string name;
            public func function;

            public keywordHash_t(string keyname, func function, object notUsed)
            {
                name = keyname;
                this.function = function;
            }
        };

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
                c[i] = float.Parse(ui.NextToken);
            }
        }

        public static idVector4 ParseColor(ref idUserInterfaceFile ui)
        {
            int i;
            float f;
            idVector4 c = new idVector4();

            for (i = 0; i < 4; i++)
            {
                c[i] = float.Parse(ui.NextToken);
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
            r.x = float.Parse(ui.NextToken);
            r.y = float.Parse(ui.NextToken);
            r.w = float.Parse(ui.NextToken);
            r.h = float.Parse(ui.NextToken);
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
                        key.function(ref menudef, ref ui);
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

        public static void MenuParse_font(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.font = ui.NextToken;
        }

        public static void MenuParse_name(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.name = ui.NextToken;
        }

        public static void MenuParse_fullscreen(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.fullScreen = (int.Parse(ui.NextToken) != 0);
        }

        public static void MenuParse_rect(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            ParseRect(ref ui, ref menu.window.rect);
        }

        public static void MenuParse_style(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.style = int.Parse(ui.NextToken);
        }

        public static void MenuParse_visible(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            int isVisible = int.Parse(ui.NextToken);

            if (isVisible != 0)
            {
                menu.window.flags |= ui_globals.WINDOW_VISIBLE;
            }
        }

        public static void MenuParse_onOpen(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.onOpen = ui.GetNextBracketedSection();
        }

        public static void MenuParse_onClose(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.onClose = ui.GetNextBracketedSection();
        }

        public static void MenuParse_onESC(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.onESC = ui.GetNextBracketedSection();
        }

        //----(SA)	added
        public static void MenuParse_onROQDone(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.onROQDone = ui.GetNextBracketedSection();
        }

        //----(SA)	end

        public static void MenuParse_border(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.border = int.Parse(ui.NextToken);
        }

        public static void MenuParse_borderSize(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.borderSize = float.Parse(ui.NextToken);
        }

        public static void MenuParse_backcolor(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                menu.window.backColor[i] = float.Parse(ui.NextToken);
            }
        }

        public static void MenuParse_forecolor(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                menu.window.foreColor[i] = float.Parse(ui.NextToken);
                menu.window.flags |= ui_globals.WINDOW_FORECOLORSET;
            }
        }

        public static void MenuParse_bordercolor(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                menu.window.borderColor[i] = float.Parse(ui.NextToken);
            }
        }

        public static void MenuParse_focuscolor(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            int i;
            for (i = 0; i < 4; i++)
            {
                menu.focusColor[i] = float.Parse(ui.NextToken);
            }
        }

        public static void MenuParse_disablecolor(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            int i;
            for (i = 0; i < 4; i++)
            {
                menu.disableColor[i] = float.Parse(ui.NextToken);
            }
        }


        public static void MenuParse_outlinecolor(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            ParseColor(ref ui, ref menu.window.outlineColor);
        }

        public static void MenuParse_background(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.background = ui.NextToken;
        }

        public static void MenuParse_cinematic(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.cinematicName = ui.NextToken;
        }

        public static void MenuParse_ownerdrawFlag(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            int i = int.Parse(ui.NextToken);

            menu.window.ownerDrawFlags |= i;
        }

        public static void MenuParse_ownerdraw(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.ownerDraw = int.Parse(ui.NextToken);
        }


        // decoration
        public static void MenuParse_popup(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.flags |= ui_globals.WINDOW_POPUP;
        }


        public static void MenuParse_outOfBounds(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.window.flags |= ui_globals.WINDOW_OOB_CLICK;
        }

        public static void MenuParse_soundLoop(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.soundName = ui.NextToken;
        }

        public static void MenuParse_fadeClamp(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.fadeClamp = float.Parse(ui.NextToken);
        }

        public static void MenuParse_fadeAmount(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.fadeAmount = float.Parse(ui.NextToken);
        }


        public static void MenuParse_fadeCycle(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            menu.fadeCycle = int.Parse(ui.NextToken);
        }




        // NERVE - SMF
        public static void MenuParse_execKey(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            char keyname;
            short keyindex;

            keyname = ui.NextToken[0];
            keyindex = (short)keyname;

            menu.onKey[keyindex] = ui.NextToken;
        }

        public static void MenuParse_execKeyInt(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            int keyname;

            keyname = int.Parse(ui.NextToken);
            menu.onKey[keyname] = ui.NextToken;
        }

        //
        // MenuParse_itemDef
        //
        public static void MenuParse_itemDef(ref idUserInterfaceMenuDef menu, ref idUserInterfaceFile ui)
        {
            if (menu.itemCount < ui_globals.MAX_MENUITEMS)
            {
                menu.items[menu.itemCount] = new idUserInterfaceItem();
                ItemParser.Item_Parse(ref menu.items[menu.itemCount], ref ui);
                menu.items[menu.itemCount++].parent = menu;
            }
        }

        

        //
        // menuParseKeywords
        //
        static keywordHash_t[] menuParseKeywords = new keywordHash_t[]{
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
