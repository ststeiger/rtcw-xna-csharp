using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace idLib.Game
{
    //
    // entityState_t->eType
    //
    public enum entityType_t
    {
	    ET_GENERAL = 0,
	    ET_PLAYER,
	    ET_ITEM,
	    ET_MISSILE,
	    ET_MOVER,
	    ET_BEAM,
	    ET_PORTAL,
	    ET_SPEAKER,
	    ET_PUSH_TRIGGER,
	    ET_TELEPORT_TRIGGER,
	    ET_INVISIBLE,
	    ET_GRAPPLE,             // grapple hooked on wall

	    //---- (SA) Wolf
	    ET_EXPLOSIVE,           // brush that will break into smaller bits when damaged
	    ET_TESLA_EF,
	    ET_SPOTLIGHT_EF,
	    ET_EFFECT3,
	    ET_ALARMBOX,
	    ET_CORONA,
	    ET_TRAP,

	    ET_GAMEMODEL,           // misc_gamemodel.  similar to misc_model, but it's a dynamic model so we have LOD
	    ET_FOOTLOCKER,  //----(SA)	added
	    ET_LEAKY,       //----(SA)	added
	    ET_MG42,        //----(SA)	why didn't we do /this/ earlier...
	    //---- end

	    ET_ZOMBIESPIT,
	    ET_FLAMEBARREL,
	    ET_ZOMBIESPIRIT,

	    ET_FP_PARTS,

	    // FIRE PROPS
	    ET_FIRE_COLUMN,
	    ET_FIRE_COLUMN_SMOKE,
	    ET_RAMJET,

	    ET_EXPLO_PART,

	    ET_CROWBAR,

	    ET_PROP,
	    ET_BAT,

	    ET_AI_EFFECT,

	    ET_CAMERA,
	    ET_MOVERSCALED,

	    ET_RUMBLE,

	    ET_SPIRIT_SPAWNER,

	    ET_FLAMETHROWER_PROP,

	    ET_EVENTS               // any of the EV_* events can be added freestanding
							    // by setting eType to ET_EVENTS + eventNum
							    // this avoids having to set eFlags and eventNum
    };
}
