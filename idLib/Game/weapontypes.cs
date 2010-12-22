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

// weapontypes.cs (c) 2010 JV Software 

namespace idLib.Game
{
    // NOTE: we can only use up to 15 in the client-server stream
    // SA NOTE: should be 31 now (I added 1 bit in msg.c)
    public enum WeaponType {
	    WP_NONE,                // 0

	    WP_KNIFE,               // 1
	    // German weapons
	    WP_LUGER,               // 2
	    WP_MP40,                // 3
	    WP_MAUSER,              // 4
	    WP_FG42,                // 5
	    WP_GRENADE_LAUNCHER,    // 6
	    WP_PANZERFAUST,         // 7
	    WP_VENOM,               // 8
	    WP_FLAMETHROWER,        // 9
	    WP_TESLA,               // 10
    //	WP_SPEARGUN,			// 11

    // weapon keys only go 1-0, so put the alternates above that (since selection will be a double click on the german weapon key)

	    // American equivalents
    //	WP_KNIFE2,				// 12
	    WP_COLT,                // 11	equivalent american weapon to german luger
	    WP_THOMPSON,            // 12	equivalent american weapon to german mp40
	    WP_GARAND,              // 13	equivalent american weapon to german mauser
    //	WP_BAR,					// 16	equivalent american weapon to german fg42
	    WP_GRENADE_PINEAPPLE,   // 14
    //	WP_ROCKET_LAUNCHER,		// 18	equivalent american weapon to german panzerfaust

	    // secondary fire weapons
	    WP_SNIPERRIFLE,         // 15
	    WP_SNOOPERSCOPE,        // 16
    //	WP_VENOM_FULL,			// 21
    //	WP_SPEARGUN_CO2,		// 22
	    WP_FG42SCOPE,           // 17	fg42 alt fire
    //	WP_BAR2,				// 24

	    // more weapons
	    WP_STEN,                // 18	silenced sten sub-machinegun
	    WP_SILENCER,            // 19	// used to be sp5
	    WP_AKIMBO,              // 20	//----(SA)	added

	    // specialized/one-time weapons
    // JPW NERVE -- swapped mortar & antitank (unused?) and added class_special
	    WP_CLASS_SPECIAL,       // 21	// class-specific multiplayer weapon (airstrike, engineer, or medpack)
	    // (SA) go ahead and take the 'freezeray' spot.  it ain't happenin
	    //      (I checked for instances of WP_CLASS_SPECIAL and I don't think this'll cause you a problem.  however, if it does, move it where you need to. ) (SA)
    // jpw
    //	WP_CROSS,				// 29
	    WP_DYNAMITE,            // 22
    //	WP_DYNAMITE2,			// 31
    //	WP_PROX,				// 32

	    WP_MONSTER_ATTACK1,     // 23	// generic monster attack, slot 1
	    WP_MONSTER_ATTACK2,     // 24	// generic monster attack, slot 2
	    WP_MONSTER_ATTACK3,     // 25	// generic monster attack, slot 2

	    WP_GAUNTLET,            // 26

	    WP_SNIPER,              // 27
	    WP_GRENADE_SMOKE,       // 28	// smoke grenade for LT multiplayer
	    WP_MEDIC_HEAL,          // 29	// DHM - Nerve :: Medic special weapon
	    WP_MORTAR,              // 30

	    VERYBIGEXPLOSION,       // 31	// explosion effect for airplanes

	    WP_NUM_WEAPONS          // 32   NOTE: this cannot be larger than 64 for AI/player weapons!

    };
}
