/*
===========================================================================

Return to Castle Wolfenstein XNA Managed C# Port
Copyright (c) 2010 JV Software
Copyright (C) 1999-2010 id Software LLC, a ZeniMax Media company. 

This file is part of the Return to Castle Wolfenstein XNA Managed C# Port GPL Source Code.  

RTCW C# Source Code is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

RTCW C# Source Code is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with RTCW C# Source Code.  If not, see <www.gnu.org/licenses/>.

In addition, the RTCW SP Source Code is also subject to certain additional terms. 
You should have received a copy of these additional terms immediately following the terms 
and conditions of the GNU General Public License which accompanied the RTCW C# Source Code.  
If not, please request a copy in writing from id Software at the address below.

If you have questions concerning this license or the applicable additional terms, you may contact in writing 
id Software LLC, c/o ZeniMax Media Inc., Suite 120, Rockville, Maryland 20850 USA.

===========================================================================
*/

// UI_cmds.cs (c) 2010 JV Software
//
using System;
using idLib;
using idLib.Math;
using idLib.Engine.Public;
using idLib.Engine.Content.ui;
using idLib.Engine.Content.ui.Private;

namespace ui
{
    //
    // idUserInterfaceCommandHandler
    //
    class idUserInterfaceCommandHandler
    {
        //
        // commandDef_t
        //
        struct commandDef_t {
	        public string name;
            public delegate void handler_t( ref idUserInterfaceItem item, ref idParser parser );
            public handler_t handler;
            
            public commandDef_t( string name, handler_t handler )
            {
                this.name = name;
                this.handler = handler;
            }
        };

        
        private static void Script_Show( ref idUserInterfaceItem item, ref idParser parser ) {
            string name = parser.NextToken;
#if false
	        const char *name;
	        if ( String_Parse( args, &name ) ) {
		        Menu_ShowItemByName( item->parent, name, qtrue );
	        }
#endif
        }

        private static void Script_Hide( ref idUserInterfaceItem item, ref idParser parser ) {
            string name = parser.NextToken;
#if false
	        const char *name;
	        if ( String_Parse( args, &name ) ) {
		        Menu_ShowItemByName( item->parent, name, qfalse );
	        }
#endif
        }

        private static void Script_FadeIn( ref idUserInterfaceItem item, ref idParser parser ) {
            string name = parser.NextToken;
#if false
	        const char *name;
	        if ( String_Parse( args, &name ) ) {
		        Menu_FadeItemByName( item->parent, name, qfalse );
	        }
#endif
        }

        private static void Script_FadeOut( ref idUserInterfaceItem item, ref idParser parser ) {
            string name = parser.NextToken;
#if false
	        const char *name;
	        if ( String_Parse( args, &name ) ) {
		        Menu_FadeItemByName( item->parent, name, qtrue );
	        }
#endif
        }



        private static void Script_Open( ref idUserInterfaceItem item, ref idParser parser ) {
            string name = parser.NextToken;

            ((idUserInterfaceLocal)item.parentUI).OpenUI(name);
#if false
	        const char *name;
	        if ( String_Parse( args, &name ) ) {
		        Menus_OpenByName( name );
	        }
#endif
        }

        private static void Script_Close( ref idUserInterfaceItem item, ref idParser parser ) {
            string name = parser.NextToken;

            ((idUserInterfaceLocal)item.parentUI).CloseUI();
#if false
	        const char *name;
	        if ( String_Parse( args, &name ) ) {
		        Menus_CloseByName( name );
	        }
#endif
        }




        /*
        ==============
        Script_Clipboard
        ==============
        */
        private static void Script_Clipboard( ref idUserInterfaceItem item, ref idParser parser ) {
#if false
	        char curscript[64];
	        DC->getCVarString( "cg_clipboardName", curscript, sizeof( curscript ) ); // grab the string the client set
	        Menu_ShowItemByName( item->parent, curscript, qtrue );
#endif
        }




        const int NOTEBOOK_MAX_PAGES = 6;    // this will not be a define


        /*
        ==============
        Script_NotebookShowpage
	        hide all notebook pages and show just the active one

	        inc == 0	- show current page
	        inc == val	- turn inc pages in the notebook (negative numbers are backwards)
	        inc == 999	- key number.  +999 is jump to last page, -999 is jump to cover page
        ==============
        */
        private static void Script_NotebookShowpage( ref idUserInterfaceItem item, ref idParser parser ) {
#if false
	        int i, inc, curpage, newpage = 0, pages;

	        pages = DC->getCVarValue( "cg_notebookpages" );

	        if ( Int_Parse( args, &inc ) ) {

		        curpage = DC->getCVarValue( "ui_notebookCurrentPage" );


		        if ( inc == 0 ) {  // opening
			        if ( pages && !curpage ) { // only open to cover if no pages exist
				        inc = 1;    // otherwise, go to first available page
			        }
		        }

		        if ( inc == 999 ) {            // jump to end
        //			newpage = NOTEBOOK_MAX_PAGES;	// = lastpage;
			        curpage = 0;
			        inc = -1;
		        } else if ( inc == -999 ) {    // jump to start
			        curpage = 0;
			        inc = 0;
		        } else if ( inc > 500 ) {
        //			curpage = DEBRIEFING_BASE + (inc - 500);
			        curpage = inc;
			        inc = 0;
		        }

		        if ( inc ) {
			        int dec = 0;

			        if ( inc > 0 ) {
				        for ( i = 1; i < NOTEBOOK_MAX_PAGES; i++ ) {
					        newpage = curpage + i;
					        if ( newpage > NOTEBOOK_MAX_PAGES ) {
						        newpage = newpage % NOTEBOOK_MAX_PAGES;
					        }

					        if ( newpage == 0 ) {
						        continue;
					        }

					        if ( pages & ( 1 << ( newpage - 1 ) ) ) {
						        dec++;
        //						if(dec == inc)
        //							break;
						        break;
					        }
				        }
				        if ( i < NOTEBOOK_MAX_PAGES ) {  // a valid page was found
					        curpage = newpage;
				        }

			        } else {
				        for ( i = 1; i < NOTEBOOK_MAX_PAGES; i++ ) {
					        newpage = curpage - i;
					        if ( newpage <= 0 ) {
						        newpage = newpage + NOTEBOOK_MAX_PAGES;
					        }

					        if ( pages & ( 1 << ( newpage - 1 ) ) ) {
						        break;
					        }
				        }
				        if ( i < NOTEBOOK_MAX_PAGES ) {  // a valid page was found
					        curpage = newpage;
				        }

			        }
		        }


		        // hide all the pages
        //		Menu_ShowItemByName(item->parent, "page_*", qfalse);
		        Menu_ShowItemByName( item->parent, "cover", qfalse );
		        for ( i = 1; i <= NOTEBOOK_MAX_PAGES; i++ ) {
			        Menu_ShowItemByName( item->parent, va( "page%d", i ), qfalse );
		        }

		        // show the visible one

		        if ( curpage ) {
			        Menu_ShowItemByName( item->parent, va( "page%d", curpage ), qtrue );
		        } else {
			        Menu_ShowItemByName( item->parent, "cover", qtrue );
		        }

		        DC->setCVar( "ui_notebookCurrentPage", va( "%d", curpage ) ); // store new current page

	        }
#endif
        }


#if false
        private static void Menu_TransitionItemByName( menuDef_t *menu, const char *p, rectDef_t rectFrom, rectDef_t rectTo, int time, float amt ) {
	        itemDef_t *item;
	        int i;
	        int count = Menu_ItemsMatchingGroup( menu, p );
	        for ( i = 0; i < count; i++ ) {
		        item = Menu_GetMatchingItemByNumber( menu, i, p );
		        if ( item != NULL ) {
			        item->window.flags |= ( WINDOW_INTRANSITION | WINDOW_VISIBLE );
			        item->window.offsetTime = time;
			        memcpy( &item->window.rectClient, &rectFrom, sizeof( rectDef_t ) );
			        memcpy( &item->window.rectEffects, &rectTo, sizeof( rectDef_t ) );
			        item->window.rectEffects2.x = abs( rectTo.x - rectFrom.x ) / amt;
			        item->window.rectEffects2.y = abs( rectTo.y - rectFrom.y ) / amt;
			        item->window.rectEffects2.w = abs( rectTo.w - rectFrom.w ) / amt;
			        item->window.rectEffects2.h = abs( rectTo.h - rectFrom.h ) / amt;
			        Item_UpdatePosition( item );
		        }
	        }
        }
#endif

        private static void Script_Transition( ref idUserInterfaceItem item, ref idParser parser ) {
            parser.ParseRestOfLine();
#if false
	        const char *name;
	        rectDef_t rectFrom, rectTo;
	        int time;
	        float amt;

	        if ( String_Parse( args, &name ) ) {
		        if ( Rect_Parse( args, &rectFrom ) && Rect_Parse( args, &rectTo ) && Int_Parse( args, &time ) && Float_Parse( args, &amt ) ) {
			        Menu_TransitionItemByName( item->parent, name, rectFrom, rectTo, time, amt );
		        }
	        }
#endif
        }

#if false
        private static void Menu_OrbitItemByName( menuDef_t *menu, const char *p, float x, float y, float cx, float cy, int time ) {

	        itemDef_t *item;
	        int i;
	        int count = Menu_ItemsMatchingGroup( menu, p );
	        for ( i = 0; i < count; i++ ) {
		        item = Menu_GetMatchingItemByNumber( menu, i, p );
		        if ( item != NULL ) {
			        item->window.flags |= ( WINDOW_ORBITING | WINDOW_VISIBLE );
			        item->window.offsetTime = time;
			        item->window.rectEffects.x = cx;
			        item->window.rectEffects.y = cy;
			        item->window.rectClient.x = x;
			        item->window.rectClient.y = y;
			        Item_UpdatePosition( item );
		        }
	        }

        }
#endif

        private static void Script_Orbit( ref idUserInterfaceItem item, ref idParser parser ) {
            string name = parser.NextToken;
#if false
	        const char *name;
	        float cx, cy, x, y;
	        int time;

	        if ( String_Parse( args, &name ) ) {
		        if ( Float_Parse( args, &x ) && Float_Parse( args, &y ) && Float_Parse( args, &cx ) && Float_Parse( args, &cy ) && Int_Parse( args, &time ) ) {
			        Menu_OrbitItemByName( item->parent, name, x, y, cx, cy, time );
		        }
	        }
#endif
        }



        private static void Script_SetFocus( ref idUserInterfaceItem item, ref idParser parser ) {
            string name = parser.NextToken;
#if false
	        const char *name;
	        itemDef_t *focusItem;

	        if ( String_Parse( args, &name ) ) {
		        focusItem = Menu_FindItemByName( item->parent, name );
		        if ( focusItem && !( focusItem->window.flags & WINDOW_DECORATION ) && !( focusItem->window.flags & WINDOW_HASFOCUS ) ) {
			        Menu_ClearFocus( item->parent );
			        focusItem->window.flags |= WINDOW_HASFOCUS;
			        if ( focusItem->onFocus ) {
				        Item_RunScript( focusItem, focusItem->onFocus );
			        }
			        if ( DC->Assets.itemFocusSound ) {
				        DC->startLocalSound( DC->Assets.itemFocusSound, CHAN_LOCAL_SOUND );
			        }
		        }
	        }
#endif
        }

        private static void Script_SetPlayerModel( ref idUserInterfaceItem item, ref idParser parser ) {
            Engine.cvarManager.Cvar_Set("team_model", parser.NextToken, true);
        }

        private static void Script_SetPlayerHead( ref idUserInterfaceItem item, ref idParser parser ) 
        {
            Engine.cvarManager.Cvar_Set( "team_headmodel", parser.NextToken, true );
        }

        private static void Script_SetCvar( ref idUserInterfaceItem item, ref idParser parser ) {
	        string cvar, val;

            cvar = parser.NextToken;
            val = parser.NextToken;

	        Engine.cvarManager.Cvar_Set( cvar, val, true );

        }

        private static void Script_Exec( ref idUserInterfaceItem item, ref idParser parser ) {
	        string val = parser.NextToken;
            Engine.cmdSystem.Cbuf_ExecuteText( cbufExec_t.EXEC_APPEND, val );
        }

        private static void Script_Play( ref idUserInterfaceItem item, ref idParser parser ) {
            idSound snd = Engine.soundManager.LoadSound( parser.NextToken );
            if (snd == null)
            {
                return;
            }
            snd.Play();
        }

        private static void Script_playLooped( ref idUserInterfaceItem item, ref idParser parser ) {
            idSound music = Engine.soundManager.LoadBackgroundTrack( parser.NextToken );
            music.Play();
        }

        // NERVE - SMF
        private static void Script_AddListItem( ref idUserInterfaceItem item, ref idParser parser ) {

	        string itemname, val, name;
	        //itemDef_t *t;

            itemname = parser.NextToken;
            val = parser.NextToken;
            name = parser.NextToken;
#if false
	        if ( String_Parse( args, &itemname ) && String_Parse( args, &val ) && String_Parse( args, &name ) ) {
		        t = Menu_FindItemByName( item->parent, itemname );
		        if ( t && t->special ) {
			        DC->feederAddItem( t->special, name, atoi( val ) );
		        }
	        }
#endif
        }
        // -NERVE - SMF

        private static void Script_SetColor( ref idUserInterfaceItem item, ref idParser parser ) {
	        string name;
	        int i;
	        float f;


            name = parser.NextToken.ToLower();
	        // expecting type of color to set and 4 args for the color
	        if ( name.Length > 0 ) {
		        if ( name == "backcolor" ) {
			        parser.NextVector4NonGrouped( ref item.window.backColor );
			        item.window.flags |= ui_globals.WINDOW_BACKCOLORSET;
		        } else if ( name ==  "forecolor" ) {
                    parser.NextVector4NonGrouped( ref item.window.foreColor );
			        item.window.flags |= ui_globals.WINDOW_FORECOLORSET;
		        } else if ( name ==  "bordercolor" ) 
                {
                    parser.NextVector4NonGrouped( ref item.window.borderColor );
		        }
	        }
        }

        private static void Script_SetItemColor(ref idUserInterfaceItem item, ref idParser parser)
        {
            string name;
            string parent;
            int i;
            float f;

            parent = parser.NextToken.ToLower();
            name = parser.NextToken.ToLower();
            // expecting type of color to set and 4 args for the color
            if (name.Length > 0)
            {
                if (name == "backcolor")
                {
                    parser.NextVector4NonGrouped(ref item.window.backColor);
                    //item.window.flags |= ui_globals.WINDOW_BACKCOLORSET;
                }
                else if (name == "forecolor")
                {
                    parser.NextVector4NonGrouped(ref item.window.foreColor);
                   // item.window.flags |= ui_globals.WINDOW_FORECOLORSET;
                }
                else if (name == "bordercolor")
                {
                    parser.NextVector4NonGrouped(ref item.window.borderColor);
                }
            }
        }

        private static void Script_SetAsset( ref idUserInterfaceItem item, ref idParser parser ) {
            /*
	        const char *name;
	        // expecting name to set asset to
	        if ( String_Parse( args, &name ) ) {
		        // check for a model
		        if ( item->type == ITEM_TYPE_MODEL ) {
		        }
	        }
            */

            string token =parser.NextToken;
        }

       private static void Script_SetBackground( ref idUserInterfaceItem item, ref idParser parser ) {
           item.window.background = parser.NextToken;
           item.window.backgroundHandle = Engine.materialManager.FindMaterial(item.window.background, -1);
        }

       private static void Script_UiCmd(ref idUserInterfaceItem item, ref idParser parser)
       {
           string cmdToken = parser.NextToken.ToLower();

           if (cmdToken == "quit")
           {
               Engine.common.Quit();
           }
           else
           {
               Engine.common.Warning("Unknown UIScript Command " + cmdToken + "\n");

               // Skip the rest of the command.
               while (parser.ReachedEndOfBuffer == false) {
                   string token = parser.NextToken;

                   if (token == null)
                   {
                       Engine.common.ErrorFatal("Unexpected EOF in UiCmd.\n");
                   }

                   if (token == ";")
                   {
                       parser.UngetToken();
                       break;
                   }
                   else if(token.Contains(";"))
                   {
                       break;
                   }
               }

               
           }
       }


        commandDef_t[] commandList = new commandDef_t[]
        {
	        new commandDef_t("fadein", Script_FadeIn),                  // group/name
	        new commandDef_t("fadeout", Script_FadeOut),                // group/name
	        new commandDef_t("show", Script_Show),                      // group/name
	        new commandDef_t("hide", Script_Hide),                      // group/name
	        new commandDef_t("setcolor", Script_SetColor),              // works on this
	        new commandDef_t("open", Script_Open),                      // menu
	        new commandDef_t("close", Script_Close),                    // menu
	        new commandDef_t("clipboard", Script_Clipboard),            // show the current clipboard group by name
	        new commandDef_t("showpage", Script_NotebookShowpage),          //
	        new commandDef_t("setasset", Script_SetAsset),              // works on this
	        new commandDef_t("setbackground", Script_SetBackground),    // works on this
	        new commandDef_t("setitemcolor",  Script_SetItemColor),      // group/name
	        //new commandDef_t("setteamcolor", Script_SetTeamColor),      // sets this background color to team color
	        new commandDef_t("setfocus", Script_SetFocus),              // sets this background color to team color
	        new commandDef_t("setplayermodel", Script_SetPlayerModel),  // sets this background color to team color
	        new commandDef_t("setplayerhead", Script_SetPlayerHead),    // sets this background color to team color
	        new commandDef_t("transition", Script_Transition),          // group/name
	        new commandDef_t("setcvar", Script_SetCvar),                // group/name
	        new commandDef_t("exec", Script_Exec),                      // group/name
	        new commandDef_t("play", Script_Play),                      // group/name
	        new commandDef_t("playlooped", Script_playLooped),          // group/name
	        new commandDef_t("orbit", Script_Orbit),                    // group/name
	        new commandDef_t("addlistitem", Script_AddListItem),     // NERVE - SMF - special command to add text items to list box
            new commandDef_t("uiscript", Script_UiCmd),
        };

        public void Execute(ref idUserInterfaceMenuDef menu, string cmds)
        {
            idUserInterfaceItem item = menu;
            Execute(ref item, cmds);
            menu = (idUserInterfaceMenuDef)item;
        }

        public void Execute(ref idUserInterfaceItem item, string cmds)
        {
            idParser parser;

            if (cmds == null || cmds.Length <= 0)
            {
                return;
            }
            
            parser = new idParser(cmds);

            while (parser.ReachedEndOfBuffer == false)
            {
                string cmd = parser.NextToken;

                if (cmd == null || cmd.Length <= 0)
                {
                    break;
                }

                cmd = cmd.ToLower();

                for (int i = 0; i <= commandList.Length; i++)
                {
                    if (i == commandList.Length)
                    {
                        parser.UngetToken();

                        Engine.common.ErrorFatal("UI_ExecuteCmd: Unknown or unepxected token %s - lasttoken %s\n", cmd, parser.NextToken);
                    }

                    if (cmd == commandList[i].name)
                    {
                        commandList[i].handler(ref item, ref parser);

                        string lastToken = parser.NextToken;
                        if (lastToken != ";" || lastToken.Contains(";") == false)
                        {
                            if (parser.ReachedEndOfBuffer == false)
                            {
                                Engine.common.Warning("UI_Execute: Expected ; in command.\n");
                            }
                        }
                        break;
                    }
                }
            }

            parser.Dispose();
        }
    }
}
