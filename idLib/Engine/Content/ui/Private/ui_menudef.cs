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


// ui_menudef.cs (c) 2010 JV Software
//

using System;

namespace idLib.Engine.Content.ui.Private
{
    public static class ui_menudef
    {
        
        public const int ITEM_TYPE_TEXT			=	0;		// simple text
        public const int ITEM_TYPE_BUTTON		=	1;		// button, basically text with a border 
        public const int ITEM_TYPE_RADIOBUTTON	=	2;		// toggle button, may be grouped 
        public const int ITEM_TYPE_CHECKBOX		=	3;		// check box
        public const int ITEM_TYPE_EDITFIELD 	=	4;		// editable text, associated with a cvar
        public const int ITEM_TYPE_COMBO 		=	5;		// drop down list
        public const int ITEM_TYPE_LISTBOX		=	6;		// scrollable list	
        public const int ITEM_TYPE_MODEL 		=	7;		// model
        public const int ITEM_TYPE_OWNERDRAW 	=	8;		// owner draw, name specs what it is
        public const int ITEM_TYPE_NUMERICFIELD	=	9;		// editable text, associated with a cvar
        public const int ITEM_TYPE_SLIDER		=	10;		// mouse speed, volume, etc.
        public const int ITEM_TYPE_YESNO 		=	11;		// yes no cvar setting
        public const int ITEM_TYPE_MULTI 		=	12;		// multiple list setting, enumerated
        public const int ITEM_TYPE_BIND			=	13;		// multiple list setting, enumerated
        public const int ITEM_TYPE_MENUMODEL 	=	14;		// special menu model
        public const int ITEM_TYPE_VALIDFILEFIELD =	15;		// text must be valid for use in a dos filename
	
        public const int ITEM_ALIGN_LEFT 		=	0;		// left alignment
        public const int ITEM_ALIGN_CENTER		=	1;		// center alignment
        public const int ITEM_ALIGN_RIGHT		=	2;		// right alignment

        public const int ITEM_TEXTSTYLE_NORMAL		=	0;	// normal text
        public const int ITEM_TEXTSTYLE_BLINK		=	1;	// fast blinking
        public const int ITEM_TEXTSTYLE_PULSE		=	2;	// slow pulsing
        public const int ITEM_TEXTSTYLE_SHADOWED 	=	3;	// drop shadow ( need a color for this )
        public const int ITEM_TEXTSTYLE_OUTLINED 	=	4;	// drop shadow ( need a color for this )
        public const int ITEM_TEXTSTYLE_OUTLINESHADOWED = 5;	// drop shadow ( need a color for this )
        public const int ITEM_TEXTSTYLE_SHADOWEDMORE 	= 6;	// drop shadow ( need a color for this )
						  
        public const int WINDOW_BORDER_NONE		=	0;		// no border
        public const int WINDOW_BORDER_FULL		=	1;		// full border based on border color ( single pixel )
        public const int WINDOW_BORDER_HORZ		=	2;		// horizontal borders only
        public const int WINDOW_BORDER_VERT		=	3;		// vertical borders only 
        public const int WINDOW_BORDER_KCGRADIENT =	4;		// horizontal border using the gradient bars
  
        public const int WINDOW_STYLE_EMPTY		=	0;		// no background
        public const int WINDOW_STYLE_FILLED 	=	1;		// filled with background color
        public const int WINDOW_STYLE_GRADIENT	=	2;		// gradient bar based on background color 
        public const int WINDOW_STYLE_SHADER 	=	3;		// gradient bar based on background color 
        public const int WINDOW_STYLE_TEAMCOLOR	=	4;		// team color
        public const int WINDOW_STYLE_CINEMATIC	=	5;		// cinematic

        public const int MENU_TRUE		=	1;		// uh.. true
        public const int MENU_FALSE		=	0;		// and false

        public const int HUD_VERTICAL	=	0x00;
        public const int HUD_HORIZONTAL	=	0x01;

        public const int RANGETYPE_ABSOLUTE =	0;
        public const int RANGETYPE_RELATIVE =	1;

        // list box element types
        public const int LISTBOX_TEXT		= 0x00;
        public const int LISTBOX_IMAGE		= 0x01;

        // list feeders
        public const int FEEDER_HEADS			=	0x00;	// model heads
        public const int FEEDER_MAPS 			=	0x01;	// text maps based on game type
        public const int FEEDER_SERVERS			=	0x02;	// servers
        public const int FEEDER_CLANS			=	0x03;	// clan names
        public const int FEEDER_ALLMAPS			=	0x04;	// all maps available, in graphic format
        public const int FEEDER_REDTEAM_LIST 	=	0x05;	// red team members
        public const int FEEDER_BLUETEAM_LIST	=	0x06;	// blue team members
        public const int FEEDER_PLAYER_LIST		=	0x07;	// players
        public const int FEEDER_TEAM_LIST		=	0x08;	// team members for team voting
        public const int FEEDER_MODS 			=	0x09;	// team members for team voting
        public const int FEEDER_DEMOS			=	0x0a;	// team members for team voting
        public const int FEEDER_SCOREBOARD		=	0x0b;	// team members for team voting
        public const int FEEDER_Q3HEADS			=	0x0c;	// model heads
        public const int FEEDER_SERVERSTATUS 	=	0x0d;	// server status
        public const int FEEDER_FINDPLAYER		=	0x0e;	// find player
        public const int FEEDER_CINEMATICS		=	0x0f;	// cinematics
        public const int FEEDER_SAVEGAMES		=	0x10;	// savegames
        public const int FEEDER_PICKSPAWN		=	0x11;	// NERVE - SMF - wolf mp pick spawn point

        // display flags
        public const int CG_SHOW_BLUE_TEAM_HAS_REDFLAG	=	0x00000001;
        public const int CG_SHOW_RED_TEAM_HAS_BLUEFLAG	=	0x00000002;
        public const int CG_SHOW_ANYTEAMGAME 			=	0x00000004;
        public const int CG_SHOW_HARVESTER				=	0x00000008;
        public const int CG_SHOW_ONEFLAG 				=	0x00000010;
        public const int CG_SHOW_CTF 					=	0x00000020;
        public const int CG_SHOW_OBELISK 				=	0x00000040;
        public const int CG_SHOW_HEALTHCRITICAL			=	0x00000080;
        public const int CG_SHOW_SINGLEPLAYER			=	0x00000100;
        public const int CG_SHOW_TOURNAMENT				=	0x00000200;
        public const int CG_SHOW_DURINGINCOMINGVOICE 	=	0x00000400;
        public const int CG_SHOW_IF_PLAYER_HAS_FLAG		=	0x00000800;
        public const int CG_SHOW_LANPLAYONLY 			=	0x00001000;
        public const int CG_SHOW_MINED					=	0x00002000;
        public const int CG_SHOW_HEALTHOK				=	0x00004000;
        public const int CG_SHOW_TEAMINFO				=	0x00008000;
        public const int CG_SHOW_NOTEAMINFO				=	0x00010000;
        public const int CG_SHOW_OTHERTEAMHASFLAG		=	0x00020000;
        public const int CG_SHOW_YOURTEAMHASENEMYFLAG	=	0x00040000;
        public const int CG_SHOW_ANYNONTEAMGAME			=	0x00080000;
        //(SA)
        public const int CG_SHOW_TEXTASINT				=	0x00200000;
        public const int CG_SHOW_HIGHLIGHTED			=	0x00100000;

        public const int CG_SHOW_NOT_V_BINOC			=	0x00200000;	//----(SA)	added	// hide on binoc huds
        public const int CG_SHOW_NOT_V_SNIPER			=	0x00400000;	//----(SA)	added	// hide on sniper huds
        public const int CG_SHOW_NOT_V_SNOOPER			=	0x00800000;	//----(SA)	added	// hide on snooper huds
        public const int CG_SHOW_NOT_V_FGSCOPE			=	0x01000000;	//----(SA)	added	// hide on fg42 scope huds
        public const int CG_SHOW_NOT_V_CLEAR			=	0x02000000;	//----(SA)	added	// hide on normal, full-view huds

        public const int CG_SHOW_2DONLY					=	0x10000000;


        public const int UI_SHOW_LEADER					=	0x00000001;
        public const int UI_SHOW_NOTLEADER				=	0x00000002;
        public const int UI_SHOW_FAVORITESERVERS 		=	0x00000004;
        public const int UI_SHOW_ANYNONTEAMGAME			=	0x00000008;
        public const int UI_SHOW_ANYTEAMGAME 			=	0x00000010;
        public const int UI_SHOW_NEWHIGHSCORE			=	0x00000020;
        public const int UI_SHOW_DEMOAVAILABLE			=	0x00000040;
        public const int UI_SHOW_NEWBESTTIME 			=	0x00000080;
        public const int UI_SHOW_FFA 					=	0x00000100;
        public const int UI_SHOW_NOTFFA					=	0x00000200;
        public const int UI_SHOW_NETANYNONTEAMGAME		=	0x00000400;
        public const int UI_SHOW_NETANYTEAMGAME			=	0x00000800;
        public const int UI_SHOW_NOTFAVORITESERVERS		=	0x00001000;

        // font types
        public const int UI_FONT_DEFAULT		=	0;	// auto-chose betwen big/reg/small
        public const int UI_FONT_NORMAL			=   1;
        public const int UI_FONT_BIG			=	2;
        public const int UI_FONT_SMALL			=   3;
        public const int UI_FONT_HANDWRITING	=	4;

        // owner draw types
        // ideally these should be done outside of this file but
        // this makes it much easier for the macro expansion to 
        // convert them for the designers ( from the .menu files )
        public const int CG_OWNERDRAW_BASE		=	1;
        public const int CG_PLAYER_ARMOR_ICON	=	1;			   
        public const int CG_PLAYER_ARMOR_VALUE	=	2;
        public const int CG_PLAYER_HEAD			=	3;
        public const int CG_PLAYER_HEALTH		=	4;
        public const int CG_PLAYER_AMMO_ICON 	=	5;
        public const int CG_PLAYER_AMMO_VALUE	=	6;
        public const int CG_SELECTEDPLAYER_HEAD	=	7;
        public const int CG_SELECTEDPLAYER_NAME	=	8;
        public const int CG_SELECTEDPLAYER_LOCATION=9;
        public const int CG_SELECTEDPLAYER_STATUS =	10;
        public const int CG_SELECTEDPLAYER_WEAPON =	11;
        public const int CG_SELECTEDPLAYER_POWERUP=	12;

        public const int CG_FLAGCARRIER_HEAD 	=	13;
        public const int CG_FLAGCARRIER_NAME 	=	14;
        public const int CG_FLAGCARRIER_LOCATION =	15;
        public const int CG_FLAGCARRIER_STATUS	 =	16;
        public const int CG_FLAGCARRIER_WEAPON	 =	17;
        public const int CG_FLAGCARRIER_POWERUP	 =	18;

        public const int CG_PLAYER_ITEM			=	19;
        public const int CG_PLAYER_SCORE 		=	20;

        public const int CG_BLUE_FLAGHEAD		=	21;
        public const int CG_BLUE_FLAGSTATUS		=	22;
        public const int CG_BLUE_FLAGNAME		=	23;
        public const int CG_RED_FLAGHEAD 		=	24;
        public const int CG_RED_FLAGSTATUS		=	25;
        public const int CG_RED_FLAGNAME 		=	26;

        public const int CG_BLUE_SCORE			=	27;
        public const int CG_RED_SCORE			=	28;
        public const int CG_RED_NAME 			=	29;
        public const int CG_BLUE_NAME			=	30;
        public const int CG_HARVESTER_SKULLS 	=	31;	// only shows in harvester
        public const int CG_ONEFLAG_STATUS		=	32;	// only shows in one flag
        public const int CG_PLAYER_LOCATION		=	33;
        public const int CG_TEAM_COLOR			=	34;
        public const int CG_CTF_POWERUP			=	35;
										
        public const int CG_AREA_POWERUP 		=	36;
        public const int CG_AREA_LAGOMETER		=	37;	// painted with old system
        public const int CG_PLAYER_HASFLAG		=	38;			  
        public const int CG_GAME_TYPE			=	39;	// not done

        public const int CG_SELECTEDPLAYER_ARMOR =	40;		
        public const int CG_SELECTEDPLAYER_HEALTH=	41;
        public const int CG_PLAYER_STATUS		=	42;
        public const int CG_FRAGGED_MSG			=	43;	// painted with old system
        public const int CG_PROXMINED_MSG		=	44;	// painted with old system
        public const int CG_AREA_FPSINFO 		=	45;	// painted with old system
        public const int CG_AREA_SYSTEMCHAT		=	46;	// painted with old system
        public const int CG_AREA_TEAMCHAT		=	47;	// painted with old system
        public const int CG_AREA_CHAT			=	48;	// painted with old system
        public const int CG_GAME_STATUS			=	49;
        public const int CG_KILLER				=	50;
        public const int CG_PLAYER_ARMOR_ICON2D	=	51;				
        public const int CG_PLAYER_AMMO_ICON2D	=	52;
        public const int CG_ACCURACY 			=	53;
        public const int CG_ASSISTS				=	54;
        public const int CG_DEFEND				=	55;
        public const int CG_EXCELLENT			=	56;
        public const int CG_IMPRESSIVE			=	57;
        public const int CG_PERFECT				=	58;
        public const int CG_GAUNTLET 			=	59;
        public const int CG_SPECTATORS			=	60;
        public const int CG_TEAMINFO 			=	61;
        public const int CG_VOICE_HEAD			=	62;
        public const int CG_VOICE_NAME			=	63;
        public const int CG_PLAYER_HASFLAG2D 	=	64;			  
        public const int CG_HARVESTER_SKULLS2D	=	65;	// only shows in harvester
        public const int CG_CAPFRAGLIMIT 		=	66;	 
        public const int CG_1STPLACE 			=	67;
        public const int CG_2NDPLACE 			=	68;
        public const int CG_CAPTURES 			=	69;

        // (SA) adding
        public const int CG_PLAYER_AMMOCLIP_VALUE =	70;	
        public const int CG_PLAYER_WEAPON_ICON2D  =	71;
        public const int CG_CURSORHINT			=	72;
        public const int CG_STAMINA				=	73;
        public const int CG_PLAYER_WEAPON_HEAT	=	74;
        public const int CG_PLAYER_POWERUP		=	75;
        public const int CG_PLAYER_HOLDABLE		=	76;
        public const int CG_PLAYER_INVENTORY	=	77;
        public const int CG_AREA_WEAPON			=	78;	// draw weapons here
        public const int CG_AREA_HOLDABLE		=	79;
        public const int CG_CURSORHINT_STATUS	=	80;	// like 'health' bar when pointing at a func_explosive
        public const int CG_PLAYER_WEAPON_STABILITY=81;	// shows aimSpreadScale value
        public const int CG_NEWMESSAGE			=	82;	// 'you got mail!'	//----(SA)	added

        public const int UI_OWNERDRAW_BASE		=	200;
        public const int UI_HANDICAP 			=	200;
        public const int UI_EFFECTS				=	201;
        public const int UI_PLAYERMODEL			=	202;
        public const int UI_CLANNAME 			=	203;
        public const int UI_CLANLOGO 			=	204;
        public const int UI_GAMETYPE 			=	205;
        public const int UI_MAPPREVIEW			=	206;
        public const int UI_SKILL				=	207;
        public const int UI_BLUETEAMNAME 		=	208;
        public const int UI_REDTEAMNAME			=	209;
        public const int UI_BLUETEAM1			=	210;
        public const int UI_BLUETEAM2			=	211;
        public const int UI_BLUETEAM3			=	212;
        public const int UI_BLUETEAM4			=	213;
        public const int UI_BLUETEAM5			=	214;
        public const int UI_REDTEAM1 			=	215;
        public const int UI_REDTEAM2 			=	216;
        public const int UI_REDTEAM3 			=	217;
        public const int UI_REDTEAM4 			=	218;
        public const int UI_REDTEAM5 			=	219;
        public const int UI_NETSOURCE			=	220;
        public const int UI_NETMAPPREVIEW		=	221;
        public const int UI_NETFILTER			=	222;
        public const int UI_TIER 				=	223;
        public const int UI_OPPONENTMODEL		=	224;
        public const int UI_TIERMAP1 			=	225;
        public const int UI_TIERMAP2 			=	226;
        public const int UI_TIERMAP3 			=	227;
        public const int UI_PLAYERLOGO			=	228;
        public const int UI_OPPONENTLOGO 		=	229;
        public const int UI_PLAYERLOGO_METAL 	=	230;
        public const int UI_OPPONENTLOGO_METAL	=	231;
        public const int UI_PLAYERLOGO_NAME		=	232;
        public const int UI_OPPONENTLOGO_NAME	=	233;
        public const int UI_TIER_MAPNAME 		=	234;
        public const int UI_TIER_GAMETYPE		=	235;
        public const int UI_ALLMAPS_SELECTION	=	236;
        public const int UI_OPPONENT_NAME		=	237;
        public const int UI_VOTE_KICK			=	238;
        public const int UI_BOTNAME				=	239;
        public const int UI_BOTSKILL 			=	240;
        public const int UI_REDBLUE				=	241;
        public const int UI_CROSSHAIR			=	242;
        public const int UI_SELECTEDPLAYER		=	243;
        public const int UI_MAPCINEMATIC 		=	244;
        public const int UI_NETGAMETYPE			=	245;
        public const int UI_NETMAPCINEMATIC		=	246;
        public const int UI_SERVERREFRESHDATE	=	247;
        public const int UI_SERVERMOTD			=	248;
        public const int UI_GLINFO				=	249;
        public const int UI_KEYBINDSTATUS		=	250;
        public const int UI_CLANCINEMATIC		=	251;
        public const int UI_MAP_TIMETOBEAT		=	252;
        public const int UI_JOINGAMETYPE 		=	253;
        public const int UI_PREVIEWCINEMATIC 	=	254;
        public const int UI_STARTMAPCINEMATIC	=	255;
        public const int UI_MAPS_SELECTION		=	256;

        public const int UI_MENUMODEL = 257;
        public const int UI_SAVEGAME_SHOT		=	258;

        // NERVE - SMF
        public const int UI_LIMBOCHAT		=		259;
        // -NERVE - SMF

        public const int UI_LEVELSHOT		=		260;
        public const int UI_LOADSTATUSBAR	=		261;
        public const int UI_SAVEGAMENAME	=			262;
        public const int UI_SAVEGAMEINFO	=			263;

        // NERVE - SMF - wolf multiplayer class/item selection mechanism
        public const int WM_START_SELECT	=		0;

        public const int WM_SELECT_TEAM		=	1;
        public const int WM_SELECT_CLASS	=		2;
        public const int WM_SELECT_WEAPON	=	3;
        public const int WM_SELECT_PISTOL	=	4;
        public const int WM_SELECT_GRENADE	=	5;
        public const int WM_SELECT_ITEM1	=		6;

        public const int WM_AXIS			=		1;
        public const int WM_ALLIES			=	2;
        public const int WM_SPECTATOR		=	3;

        public const int WM_SOLDIER			=	1;
        public const int WM_MEDIC			=	2;
        public const int WM_LIEUTENANT		=	3;
        public const int WM_ENGINEER		=		4;

        public const int WM_PISTOL_1911		=	1;
        public const int WM_PISTOL_LUGER	=		2;

        public const int WM_WEAPON_MP40		=	3;
        public const int WM_WEAPON_THOMPSON	=	4;
        public const int WM_WEAPON_STEN		=	5;
        public const int WM_WEAPON_MAUSER	=	6;
        public const int WM_WEAPON_GARAND	=	7;
        public const int WM_WEAPON_PANZERFAUST=	8;
        public const int WM_WEAPON_VENOM	=		9;
        public const int WM_WEAPON_FLAMETHROWER= 10;

        public const int WM_PINEAPPLE_GRENADE =	11;
        public const int WM_STICK_GRENADE	  =	12;
        // -NERVE - SMF

        public const string VOICECHAT_GETFLAG		=	"getflag";				// command someone to get the flag
        public const string VOICECHAT_OFFENSE		=	"offense";				// command someone to go on offense
        public const string VOICECHAT_DEFEND		=	"defend"	;			// command someone to go on defense
        public const string VOICECHAT_DEFENDFLAG	=	"defendflag";			// command someone to defend the flag
        public const string VOICECHAT_PATROL		=	"patrol";				// command someone to go on patrol (roam)
        public const string VOICECHAT_CAMP			=	"camp"	;				// command someone to camp (we don't have sounds for this one)
        public const string VOICECHAT_FOLLOWME		=	"followme";				// command someone to follow you
        public const string VOICECHAT_RETURNFLAG	=	"returnflag";			// command someone to return our flag
        public const string VOICECHAT_FOLLOWFLAGCARRIER= "followflagcarrier"; 	// command someone to follow the flag carrier
        public const string VOICECHAT_YES			=	"yes";					// yes, affirmative, etc.
        public const string VOICECHAT_NO			=	"no";					// no, negative, etc.
        public const string VOICECHAT_ONGETFLAG 	=	"ongetflag"; 			// I'm getting the flag
        public const string VOICECHAT_ONOFFENSE 	=	"onoffense"; 			// I'm on offense
        public const string VOICECHAT_ONDEFENSE 	=	"ondefense"; 			// I'm on defense
        public const string VOICECHAT_ONPATROL		=	"onpatrol"	;			// I'm on patrol (roaming)
        public const string VOICECHAT_ONCAMPING 	=	"oncamp";				// I'm camping somewhere
        public const string VOICECHAT_ONFOLLOW		=	"onfollow";				// I'm following
        public const string VOICECHAT_ONFOLLOWCARRIER=	"onfollowcarrier";		// I'm following the flag carrier
        public const string VOICECHAT_ONRETURNFLAG	=	"onreturnflag";			// I'm returning our flag
        public const string VOICECHAT_INPOSITION	=	"inposition";			// I'm in position
        public const string VOICECHAT_IHAVEFLAG 	=	"ihaveflag"; 			// I have the flag
        public const string VOICECHAT_BASEATTACK	=	"baseattack";			// the base is under attack
        public const string VOICECHAT_ENEMYHASFLAG	=	"enemyhasflag";			// the enemy has our flag (CTF)
        public const string VOICECHAT_STARTLEADER	=	"startleader";			// I'm the leader
        public const string VOICECHAT_STOPLEADER	=	"stopleader";			// I resign leadership
        public const string VOICECHAT_WHOISLEADER	=	"whoisleader";			// who is the team leader
        public const string VOICECHAT_WANTONDEFENSE =	"wantondefense"; 		// I want to be on defense
        public const string VOICECHAT_WANTONOFFENSE =	"wantonoffense"; 		// I want to be on offense
        public const string VOICECHAT_KILLINSULT	=	"kill_insult";			// I just killed you
        public const string VOICECHAT_TAUNT 		=	"taunt" 	;			// I want to taunt you
        public const string VOICECHAT_DEATHINSULT	=	"death_insult"	;		// you just killed me
        public const string VOICECHAT_KILLGAUNTLET	=	"kill_gauntlet"; 		// I just killed you with the gauntlet
        public const string VOICECHAT_PRAISE		=	"praise";				// you did something good
    }
}
