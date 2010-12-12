using System;
using idLib.Engine.Content.ui.Private;

namespace idLib.Engine.Content.ui
{
    class ItemParser
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

        //
        // ValidateTypeData
        //
        private static void ValidateTypeData(ref idUserInterfaceItem item)
        {
            if (item.typeData != null)
            {
                return;
            }

            if (item.type == ui_menudef.ITEM_TYPE_LISTBOX)
            {
                item.typeData = new idUserInterfaceListBox();
            }
            else if (item.type == ui_menudef.ITEM_TYPE_EDITFIELD || item.type == ui_menudef.ITEM_TYPE_NUMERICFIELD || item.type == ui_menudef.ITEM_TYPE_VALIDFILEFIELD || item.type == ui_menudef.ITEM_TYPE_YESNO || item.type == ui_menudef.ITEM_TYPE_BIND || item.type == ui_menudef.ITEM_TYPE_SLIDER || item.type == ui_menudef.ITEM_TYPE_TEXT)
            {
                item.typeData = new idUserInterfaceEditField();
                if (item.type == ui_menudef.ITEM_TYPE_EDITFIELD || item.type == ui_menudef.ITEM_TYPE_VALIDFILEFIELD)
                {
                    if (((idUserInterfaceEditField)item.typeData).maxPaintChars == 0)
                    {
                        ((idUserInterfaceEditField)item.typeData).maxPaintChars = ui_globals.MAX_EDITFIELD;
                    }
                }
            }
            else if (item.type == ui_menudef.ITEM_TYPE_MULTI)
            {
                item.typeData = new idUserInterfaceMultiDef();
            }
            else if (item.type == ui_menudef.ITEM_TYPE_MODEL)
            {
                item.typeData = new idUserInterfaceModel();
            }
            else if (item.type == ui_menudef.ITEM_TYPE_MENUMODEL)
            {
                item.typeData = new idUserInterfaceModel();
            }
        }

        /*
        ===============
        Item Keyword Parse functions
        ===============
        */

        // name <string>
        private static void ItemParse_name(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.name = ui.NextToken;
            
        }

        // name <string>
        private static void ItemParse_focusSound(ref idUserInterfaceItem item, ref idUserInterfaceFile ui) {
            item.focusSound = ui.NextToken;
        }


        // text <string>
        private static void ItemParse_text(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.text = ui.NextToken;
        }

        //----(SA)	added

        // textfile <string>
        // read an external textfile into item->text
        private static void ItemParse_textfile(ref idUserInterfaceItem item, ref idUserInterfaceFile ui) {
            item.text = "filepath_" + ui.NextToken;
        }
        //----(SA)


        //----(SA)	added
        private static void ItemParse_textsavegame(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {

            // this'll get picked up when the savegames are parsed
            item.text = "savegameinfo";
            item.textSavegameInfo = true;
        }
        //----(SA)	end



        // group <string>
        private static void ItemParse_group(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.group = ui.NextToken;
        }


        // asset_model <string>
        private static void ItemParse_asset_model(ref idUserInterfaceItem item, ref idUserInterfaceFile ui) {
            item.asset_model = ui.NextToken;
        }

        // asset_shader <string>
        private static void ItemParse_asset_shader(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.asset_shader = ui.NextToken;
        }
        // model_origin <number> <number> <number>
        private static void ItemParse_model_origin(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceModel model = (idUserInterfaceModel)item.typeData;

            model.origin[0] = float.Parse(ui.NextToken);
            model.origin[1] = float.Parse(ui.NextToken);
            model.origin[2] = float.Parse(ui.NextToken);

            item.typeData = model;
        }

        // model_fovx <number>
        private static void ItemParse_model_fovx(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceModel model = (idUserInterfaceModel)item.typeData;

            model.fov_x = float.Parse(ui.NextToken);
            
            item.typeData = model;
        }

        // model_fovy <number>
        private static void ItemParse_model_fovy(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceModel model = (idUserInterfaceModel)item.typeData;

            model.fov_y = float.Parse(ui.NextToken);

            item.typeData = model;
        }

        // model_rotation <integer>
        private static void ItemParse_model_rotation(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceModel model = (idUserInterfaceModel)item.typeData;

            model.rotationSpeed = int.Parse(ui.NextToken);

            item.typeData = model;
        }

        // model_angle <integer>
        private static void ItemParse_model_angle(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceModel model = (idUserInterfaceModel)item.typeData;

            model.angle = int.Parse(ui.NextToken);

            item.typeData = model;
        }

        // model_animplay <int(startframe)> <int(numframes)> <int(loopframes)> <int(fps)>
        private static void ItemParse_model_animplay(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            idUserInterfaceModel modelPtr = (idUserInterfaceModel)item.typeData;


            modelPtr.animated = 1;
            modelPtr.startframe = int.Parse(ui.NextToken);
            modelPtr.numframes = int.Parse(ui.NextToken);
            modelPtr.loopframes = int.Parse(ui.NextToken);
            modelPtr.fps = int.Parse(ui.NextToken);

            modelPtr.frame = modelPtr.startframe + 1;
            modelPtr.oldframe = modelPtr.startframe;
            modelPtr.backlerp = 0.0f;
           // modelPtr->frameTime = DC->realTime;

            item.typeData = modelPtr;
        }


        // rect <rectangle>
        private static void ItemParse_rect(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            MenuParser.ParseRect( ref ui, ref item.window.rectClient );
        }

        // style <integer>
        private static void ItemParse_style(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.style = int.Parse(ui.NextToken);
        }

        // decoration
        private static void ItemParse_decoration(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.flags |= ui_globals.WINDOW_DECORATION;
        }

        // notselectable
        private static void ItemParse_notselectable(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            if (item.type == ui_menudef.ITEM_TYPE_LISTBOX)
            {
                ((idUserInterfaceListBox)item.typeData).notselectable = true;
            }
        }

        // manually wrapped
        private static void ItemParse_wrapped(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.flags |= ui_globals.WINDOW_WRAPPED;
        }

        // auto wrapped
        private static void ItemParse_autowrapped(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.flags |= ui_globals.WINDOW_AUTOWRAPPED;
        }


        // horizontalscroll
        private static void ItemParse_horizontalscroll(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.flags |= ui_globals.WINDOW_HORIZONTAL;
        }

        // type <integer>
        private static void ItemParse_type(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.type = int.Parse(ui.NextToken);
            ValidateTypeData(ref item);
        }

        // elementwidth, used for listbox image elements
        // uses textalignx for storage
        private static void ItemParse_elementwidth(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ValidateTypeData(ref item);
            ((idUserInterfaceListBox)item.typeData).elementWidth = float.Parse(ui.NextToken);
        }

        // elementheight, used for listbox image elements
        // uses textaligny for storage
        private static void ItemParse_elementheight(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ValidateTypeData(ref item);
            ((idUserInterfaceListBox)item.typeData).elementHeight = float.Parse(ui.NextToken);
        }

        // feeder <float>
        private static void ItemParse_feeder(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.special = float.Parse(ui.NextToken);
        }

        // elementtype, used to specify what type of elements a listbox contains
        // uses textstyle for storage
        private static void ItemParse_elementtype(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ValidateTypeData(ref item);
            ((idUserInterfaceListBox)item.typeData).elementStyle = int.Parse(ui.NextToken);
        }

        // columns sets a number of columns and an x pos and width per..
        private static void ItemParse_columns(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int num, i;
            ValidateTypeData(ref item);

            num = int.Parse(ui.NextToken);
            if (num > ui_globals.MAX_LB_COLUMNS)
            {
                num = ui_globals.MAX_LB_COLUMNS;
            }
            ((idUserInterfaceListBox)item.typeData).numColumns = num;
            for (i = 0; i < num; i++)
            {
                ((idUserInterfaceListBox)item.typeData).columnInfo[i] = new idUserInterfaceColumnInfo();
                ((idUserInterfaceListBox)item.typeData).columnInfo[i].pos = int.Parse(ui.NextToken);
                ((idUserInterfaceListBox)item.typeData).columnInfo[i].width = int.Parse(ui.NextToken);
                ((idUserInterfaceListBox)item.typeData).columnInfo[i].maxChars = int.Parse(ui.NextToken);
            }
        }

        private static void ItemParse_border(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.border = int.Parse(ui.NextToken);
        }

        private static void ItemParse_bordersize(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.borderSize = int.Parse(ui.NextToken);
        }

        private static void ItemParse_visible(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i = int.Parse(ui.NextToken);

            if (i !=0)
            {
                item.window.flags |= ui_globals.WINDOW_VISIBLE;
            }
        }

        private static void ItemParse_ownerdraw(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.ownerDraw = int.Parse(ui.NextToken);
            item.type = ui_menudef.ITEM_TYPE_OWNERDRAW;
        }

        private static void ItemParse_align(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            string s = ui.NextToken;
            try
            {
                item.alignment = int.Parse(s);
            }
            catch (FormatException)
            {
                throw new Exception("Invalid format for token " + s);
            }
        }

        private static void ItemParse_textalign(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.textalignment = int.Parse(ui.NextToken);
        }

        private static void ItemParse_textalignx(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.textalignx = float.Parse(ui.NextToken);
        }

        private static void ItemParse_textaligny(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.textaligny = float.Parse(ui.NextToken);
        }

        private static void ItemParse_textscale(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.textscale = float.Parse(ui.NextToken);
        }

        private static void ItemParse_textstyle(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.textStyle = int.Parse(ui.NextToken);
        }

        //----(SA)	added for forcing a font for a given item
        private static void ItemParse_textfont(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.fontSize = int.Parse(ui.NextToken);
        }
        //----(SA)	end

        private static void ItemParse_backcolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                item.window.backColor[i] = float.Parse(ui.NextToken);
            }
        }

        private static void ItemParse_forecolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                item.window.foreColor[i] = float.Parse(ui.NextToken);
                item.window.flags |= ui_globals.WINDOW_FORECOLORSET;
            }
        }

        private static void ItemParse_bordercolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i;

            for (i = 0; i < 4; i++)
            {
                item.window.borderColor[i] = float.Parse(ui.NextToken);
            }
        }

        private static void ItemParse_outlinecolor(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            MenuParser.ParseColor(ref ui, ref item.window.outlineColor);
        }

        private static void ItemParse_background(ref idUserInterfaceItem item, ref idUserInterfaceFile ui) {
	        item.window.background = ui.NextToken;
        }

        private static void ItemParse_cinematic(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.window.cinematicName = ui.NextToken;
        }

        private static void ItemParse_doubleClick(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ((idUserInterfaceListBox)item.typeData).doubleClick = ui.GetNextBracketedSection();
        }

        private static void ItemParse_onFocus(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.onFocus = ui.GetNextBracketedSection();
        }

        private static void ItemParse_leaveFocus(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.leaveFocus = ui.GetNextBracketedSection();
        }

        private static void ItemParse_mouseEnter(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.mouseEnter = ui.GetNextBracketedSection();
        }

        private static void ItemParse_mouseExit(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.mouseExit = ui.GetNextBracketedSection();
        }

        private static void ItemParse_mouseEnterText(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.mouseEnterText = ui.GetNextBracketedSection();
        }

        private static void ItemParse_mouseExitText(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.mouseExitText = ui.GetNextBracketedSection();
        }

        private static void ItemParse_action(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.action = ui.GetNextBracketedSection();
        }

        // NERVE - SMF
        private static void ItemParse_accept(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.onAccept = ui.GetNextBracketedSection();
        }
        // -NERVE - SMF

        private static void ItemParse_special(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.special = float.Parse(ui.NextToken);
        }

        private static void ItemParse_cvarTest(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ValidateTypeData(ref item);
            item.cvarTest = ui.NextToken;
        }

        private static void ItemParse_cvar(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ValidateTypeData(ref item);
            item.cvar = ui.NextToken;
           // ((idUserInterfaceEditField)item.typeData).minVal = -1;
           // ((idUserInterfaceEditField)item.typeData).maxVal = -1;
           // ((idUserInterfaceEditField)item.typeData).defVal = -1;
        }

        private static void ItemParse_maxChars(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ValidateTypeData(ref item);
            ((idUserInterfaceEditField)item.typeData).maxChars = int.Parse(ui.NextToken);
        }

        private static void ItemParse_maxPaintChars(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ((idUserInterfaceEditField)item.typeData).maxPaintChars = int.Parse(ui.NextToken);
        }



        private static void ItemParse_cvarFloat(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
           //editPtr = (editFieldDef_t*)item->typeData;
            item.cvar = ui.NextToken;
            ((idUserInterfaceEditField)item.typeData).defVal = float.Parse(ui.NextToken);
            ((idUserInterfaceEditField)item.typeData).minVal = float.Parse(ui.NextToken);
            ((idUserInterfaceEditField)item.typeData).maxVal = float.Parse(ui.NextToken);
        }

        private static void ItemParse_cvarStrList(ref idUserInterfaceItem item, ref idUserInterfaceFile ui) 
        {
	        int pass = 0;
            string token;
   
            ((idUserInterfaceMultiDef)item.typeData).count = 0;
            ((idUserInterfaceMultiDef)item.typeData).strDef = true;

	        if(ui.NextToken != "{")
                throw new Exception("CvarStrList expected open bracket");

	        pass = 0;
	        while ( true ) {
		        if(ui.ReachedEndOfBuffer)
                {
                    throw new Exception("end of file inside menu item\n");
                }
                token = ui.NextToken;

		        if ( token == "}" ) {
			        break;
		        }

		        if ( token == "," || token == ";" ) {
			        continue;
		        }

		        if ( pass == 0 ) {
			        ((idUserInterfaceMultiDef)item.typeData).cvarList[((idUserInterfaceMultiDef)item.typeData).count] = token;
			        pass = 1;
		        } else {
                    ((idUserInterfaceMultiDef)item.typeData).cvarStr[((idUserInterfaceMultiDef)item.typeData).count] = token;
			        pass = 0;
			        ((idUserInterfaceMultiDef)item.typeData).count++;
			        if ( ((idUserInterfaceMultiDef)item.typeData).count >= ui_globals.MAX_MULTI_CVARS ) {
                        throw new Exception("((idUserInterfaceMultiDef)item.typeData).count >= ui_globals.MAX_MULTI_CVARS");
			        }
		        }

	        }

	        //return qfalse;
        }

        private static void ItemParse_cvarFloatList(ref idUserInterfaceItem item, ref idUserInterfaceFile ui) {
            string token;
   
            ((idUserInterfaceMultiDef)item.typeData).count = 0;
            ((idUserInterfaceMultiDef)item.typeData).strDef = true;

	        if(ui.NextToken != "{")
                throw new Exception("CvarStrList expected open bracket");

	        while ( true ) {
		       if(ui.ReachedEndOfBuffer)
                {
                    throw new Exception("end of file inside menu item\n");
                }
                token = ui.NextToken;

		        if ( token == "}" ) {
			        break;
		        }

		        if ( token == "," || token == ";" ) {
			        continue;
		        }

                ((idUserInterfaceMultiDef)item.typeData).cvarList[((idUserInterfaceMultiDef)item.typeData).count] = token;
                ((idUserInterfaceMultiDef)item.typeData).cvarValue[((idUserInterfaceMultiDef)item.typeData).count] = float.Parse(ui.NextToken);

		        ((idUserInterfaceMultiDef)item.typeData).count++;
		         if ( ((idUserInterfaceMultiDef)item.typeData).count >= ui_globals.MAX_MULTI_CVARS ) {
                        throw new Exception("((idUserInterfaceMultiDef)item.typeData).count >= ui_globals.MAX_MULTI_CVARS");
			        }

	        }

	        //return qfalse;
        }


        private static void ParseColorRange(ref idUserInterfaceItem item, ref idUserInterfaceFile ui, int type)
        {
            if (item.numColors != 0 && type != item.colorRangeType)
            {
                throw new Exception( "both addColorRange and addColorRangeRel - set within same itemdef\n");
            }

            item.colorRangeType = type;
            item.colorRanges[item.numColors] = new idUserInterfaceColorRangeDef();
            item.colorRanges[item.numColors].low = float.Parse(ui.NextToken);
            item.colorRanges[item.numColors].high = float.Parse(ui.NextToken);
            MenuParser.ParseColor(ref ui, ref item.colorRanges[item.numColors].color);
        }

        private static void ItemParse_addColorRangeRel(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ParseColorRange(ref item, ref ui, ui_menudef.RANGETYPE_RELATIVE);
        }

        private static void ItemParse_addColorRange(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            ParseColorRange(ref item, ref ui, ui_menudef.RANGETYPE_ABSOLUTE);
        }



        private static void ItemParse_ownerdrawFlag(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            int i = int.Parse(ui.NextToken);
            item.window.ownerDrawFlags |= i;
        }

        private static void ItemParse_enableCvar(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.enableCvar = ui.NextToken;
            item.cvarFlags = ui_globals.CVAR_ENABLE;
        }

        private static void ItemParse_disableCvar(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.enableCvar = ui.NextToken;
            item.cvarFlags = ui_globals.CVAR_DISABLE;
        }

        private static void ItemParse_showCvar(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.enableCvar = ui.GetNextBracketedSection();
            item.cvarFlags = ui_globals.CVAR_SHOW;
        }

        private static void ItemParse_hideCvar(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            item.enableCvar = ui.GetNextBracketedSection() ;
            item.cvarFlags = ui_globals.CVAR_HIDE;
        }

        private static keywordHash_t[] itemParseKeywords = new keywordHash_t[]{
	        new keywordHash_t("name", ItemParse_name, null),
	        new keywordHash_t("text", ItemParse_text, null),
	        new keywordHash_t("textfile", ItemParse_textfile, null),  //----(SA)	added
	        new keywordHash_t("textsavegame", ItemParse_textsavegame, null),  //----(SA)	added
	        new keywordHash_t("group", ItemParse_group, null),
	        new keywordHash_t("asset_model", ItemParse_asset_model, null),
	        new keywordHash_t("asset_shader", ItemParse_asset_shader, null),
	        new keywordHash_t("model_origin", ItemParse_model_origin, null),
	        new keywordHash_t("model_fovx", ItemParse_model_fovx, null),
	        new keywordHash_t("model_fovy", ItemParse_model_fovy, null),
	        new keywordHash_t("model_rotation", ItemParse_model_rotation, null),
	        new keywordHash_t("model_angle", ItemParse_model_angle, null),
	        new keywordHash_t("model_animplay", ItemParse_model_animplay, null),
	        new keywordHash_t("rect", ItemParse_rect, null),
	        new keywordHash_t("style", ItemParse_style, null),
	        new keywordHash_t("decoration", ItemParse_decoration, null),
	        new keywordHash_t("notselectable", ItemParse_notselectable, null),
	        new keywordHash_t("wrapped", ItemParse_wrapped, null),
	        new keywordHash_t("autowrapped", ItemParse_autowrapped, null),
	        new keywordHash_t("horizontalscroll", ItemParse_horizontalscroll, null),
	        new keywordHash_t("type", ItemParse_type, null),
	        new keywordHash_t("elementwidth", ItemParse_elementwidth, null),
	        new keywordHash_t("elementheight", ItemParse_elementheight, null),
	        new keywordHash_t("feeder", ItemParse_feeder, null),
	        new keywordHash_t("elementtype", ItemParse_elementtype, null),
	        new keywordHash_t("columns", ItemParse_columns, null),
	        new keywordHash_t("border", ItemParse_border, null),
	        new keywordHash_t("bordersize", ItemParse_bordersize, null),
	        new keywordHash_t("visible", ItemParse_visible, null),
	        new keywordHash_t("ownerdraw", ItemParse_ownerdraw, null),
	        new keywordHash_t("align", ItemParse_align, null),
	        new keywordHash_t("textalign", ItemParse_textalign, null),
	        new keywordHash_t("textalignx", ItemParse_textalignx, null),
	        new keywordHash_t("textaligny", ItemParse_textaligny, null),
	        new keywordHash_t("textscale", ItemParse_textscale, null),
	        new keywordHash_t("textstyle", ItemParse_textstyle, null),
	        new keywordHash_t("textfont", ItemParse_textfont, null),
	        new keywordHash_t("backcolor", ItemParse_backcolor, null),
	        new keywordHash_t("forecolor", ItemParse_forecolor, null),
	        new keywordHash_t("bordercolor", ItemParse_bordercolor, null),
	        new keywordHash_t("outlinecolor", ItemParse_outlinecolor, null),
	        new keywordHash_t("background", ItemParse_background, null),
	        new keywordHash_t("onFocus", ItemParse_onFocus, null),
	        new keywordHash_t("leaveFocus", ItemParse_leaveFocus, null),
	        new keywordHash_t("mouseEnter", ItemParse_mouseEnter, null),
	        new keywordHash_t("mouseExit", ItemParse_mouseExit, null),
	        new keywordHash_t("mouseEnterText", ItemParse_mouseEnterText, null),
	        new keywordHash_t("mouseExitText", ItemParse_mouseExitText, null),
	        new keywordHash_t("action", ItemParse_action, null),
	        new keywordHash_t("accept", ItemParse_accept, null),              // NERVE - SMF
	        new keywordHash_t("special", ItemParse_special, null),
	        new keywordHash_t("cvar", ItemParse_cvar, null),
	        new keywordHash_t("maxChars", ItemParse_maxChars, null),
	        new keywordHash_t("maxPaintChars", ItemParse_maxPaintChars, null),
	        new keywordHash_t("focusSound", ItemParse_focusSound, null),
	        new keywordHash_t("cvarFloat", ItemParse_cvarFloat, null),
	        new keywordHash_t("cvarStrList", ItemParse_cvarStrList, null),
	        new keywordHash_t("cvarFloatList", ItemParse_cvarFloatList, null),
	        new keywordHash_t("addColorRange", ItemParse_addColorRange, null),
	        new keywordHash_t("addColorRangeRel", ItemParse_addColorRangeRel, null),
	        new keywordHash_t("ownerdrawFlag", ItemParse_ownerdrawFlag, null),
	        new keywordHash_t("enableCvar", ItemParse_enableCvar, null),
	        new keywordHash_t("cvarTest", ItemParse_cvarTest, null),
	        new keywordHash_t("disableCvar", ItemParse_disableCvar, null),
	        new keywordHash_t("showCvar", ItemParse_showCvar, null),
	        new keywordHash_t("hideCvar", ItemParse_hideCvar, null),
	        new keywordHash_t("cinematic", ItemParse_cinematic, null),
	        new keywordHash_t("doubleclick", ItemParse_doubleClick, null),
        };
        /*
        ===============
        Item_Parse
        ===============
        */
        public static void Item_Parse(ref idUserInterfaceItem item, ref idUserInterfaceFile ui)
        {
            bool foundKeyFunction = false;

            if (ui.ReachedEndOfBuffer)
                throw new Exception("EOF at menu item");

            if (ui.NextToken != "{")
                throw new Exception("UI item expected open bracket");

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
                foreach (keywordHash_t key in itemParseKeywords)
                {
                    if (key.name.ToLower() == token)
                    {
                        key.function(ref item, ref ui);
                        foundKeyFunction = true;
                        break;
                    }
                }

                if (foundKeyFunction == false)
                {
                    throw new Exception("unknown menu item keyword " + token);
                }
                foundKeyFunction = false;
            }
        }
    }
}
