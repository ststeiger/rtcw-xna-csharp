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

// CvarManager.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using idLib.Engine.Public;

namespace rtcw.Framework
{
    //
    // idCVarLocal
    //
    public class idCVarLocal : idCVar
    {
        string name;
        string valstr;
        int cvar_flags;
        int modifiedcount = 0;
        bool hasBeenModified = false;

        public string resetString = "";
        public string latchedString = null;

        //
        // idCVarLocal
        //
        public idCVarLocal(string cvar_name, string cvar_value, int cvar_flags)
        {
            name = cvar_name;
            valstr = cvar_value;
            this.cvar_flags = cvar_flags;
            modifiedcount++;
            hasBeenModified = true;
            resetString = cvar_value;
        }

        //
        // SetValue
        //
        public override void SetValue(string value, bool force)
        {
            if ( value == null ) {
		        value = resetString;
	        }

	        if ( value == valstr ) {
                return;
	        }

            if ( !force ) {
		        if ( (flags & CVAR_ROM) != 0 ) {
			        Engine.common.Warning( "%s is read only.\n", name );
			        return;
		        }

		        if ( (flags & CVAR_INIT) != 0 ) {
			        Engine.common.Warning( "%s is write protected.\n", name );
			        return;
		        }

		        if ( (flags & CVAR_LATCH) != 0 ) {
			        if ( latchedString != null ) {
                        if( value == latchedString )
                        {
                            return;
                        }
                        
                        latchedString = null;
			        } 
                    else
			        {
                        if( value == valstr )
                        {
                            return;
                        }
			        }

			        Engine.common.Printf( "%s will be changed upon restarting.\n", name );
			        latchedString = value;
			        hasBeenModified = true;
			        modifiedcount++;
			        return;
		        }

		        if ( ( flags & CVAR_CHEAT ) != 0 && idCVarManagerLocal.cvar_cheats.GetValueBool() ) {
			        Engine.common.Warning( "%s is cheat protected.\n", name );
			        return;
		        }

	        } 
            else
	        {
		        if ( latchedString != null ) {
			        latchedString = null;
		        }
	        }

	        if ( value == valstr ) {
		        return;     // not changed
	        }

	        hasBeenModified = true;
	        modifiedcount++;

	        valstr = value;
        }

        //
        // Modified
        //
        public bool Modified
        {
            get
            {
                return hasBeenModified;
            }
        }

        //
        // flags
        //
        public int flags
        {
            get
            {
                return cvar_flags;
            }
            set
            {
                cvar_flags = value;
            }
        }

        //
        // GetName
        //
        public override string GetName()
        {
            return name;
        }

        //
        // GetValue
        //
        public override string GetValue()
        {
            return valstr;
        }

        //
        // GetValueBool
        //
        public override bool GetValueBool()
        {
            return Boolean.Parse(valstr);
        }

        //
        // GetValueFloat
        //
        public override float GetValueFloat()
        {
            return float.Parse(valstr);
        }

        //
        // GetValueInteger
        //
        public override int GetValueInteger()
        {
            return int.Parse(valstr);
        }

        //
        // GetValueShort
        //
        public override short GetValueShort()
        {
            return short.Parse(valstr);
        }

        //
        // ResetVar
        //
        public override void ResetVar()
        {
            valstr = resetString;
        }

        //
        // SetValueBool
        //
        public override void SetValueBool(bool val)
        {
            valstr = "" + val;
        }

        //
        // SetValueFloat
        //
        public override void SetValueFloat(float val)
        {
            valstr = "" + val;
        }

        //
        // SetValueInt
        //
        public override void SetValueInt(int val)
        {
            valstr = "" + val;
        }

        //
        // SetValueShort
        //
        public override void SetValueShort(short val)
        {
            valstr = "" + val;
        }
    }

    //
    // idCVarManagerLocal
    //
    public class idCVarManagerLocal : idCVarManager
    {
        public static idCVar cvar_cheats;

        List<idCVarLocal> cvarpool = new List<idCVarLocal>();
        public static int cvar_modifiedFlags;

        //
        // FindCvar
        //
        private idCVarLocal FindCvar(string cvar_name)
        {
            for (int i = 0; i < cvarpool.Count; i++)
            {
                if (cvarpool[i].GetName() == cvar_name)
                {
                    return cvarpool[i];
                }
            }
            return null;
        }

        //
        // Cvar_Set
        //
        public override idCVar Cvar_Set(string var_name, string value, bool force)
        {
            idCVarLocal var;

            var = FindCvar(var_name);
            if (var == null)
            {
                if (value == null)
                {
                    return null;
                }
                // create it
                if (!force)
                {
                    return Cvar_Get(var_name, value, idCVar.CVAR_USER_CREATED);
                }

                return Cvar_Get(var_name, value, 0);
            }

            var.SetValue(value, force);
            if (var.Modified)
            {
                cvar_modifiedFlags |= var.flags;
            }

            return var;
        }

        //
        // Cvar_Get
        //
        public override idCVar Cvar_Get(string var_name, string var_value, int flags)
        {
            idCVarLocal var;
            int varhashval = 0;
            
            if (var_name == null || var_value == null)
            {
                Engine.common.ErrorFatal("Cvar_Get: NULL parameter");
                return null;
            }

            var = FindCvar(var_name);
            if (var != null)
            {
                // if the C code is now specifying a variable that the user already
                // set a value for, take the new value as the reset value
                if ((var.flags & idCVar.CVAR_USER_CREATED) != 0 && (flags & idCVar.CVAR_USER_CREATED) == 0
                     && var_value.Length != 0)
                {
                    var.flags &= ~idCVar.CVAR_USER_CREATED;
                    var.resetString = var_value;

                    // ZOID--needs to be set so that cvars the game sets as
                    // SERVERINFO get sent to clients
                    cvar_modifiedFlags |= flags;
                }

                var.flags |= flags;
		        // only allow one non-empty reset string without a warning
		        if ( var.resetString.Length <= 0 ) {
			        // we don't have a reset string yet
			        var.resetString = var_value;
                }
                else if (var.resetString.Length <= 0 && var.resetString != var_value)
                {
			        Engine.common.Warning( "cvar \"%s\" given initial values: \"%s\" and \"%s\"\n", var_name, var.resetString, var_value );
		        }
		        // if we have a latched string, take that value now
		        if ( var.latchedString != null ) {
			        string s;
                    s = var.latchedString;
			        var.latchedString = null;  // otherwise cvar_set2 would free it
			        Cvar_Set( var_name, s, true );
		        }

        // use a CVAR_SET for rom sets, get won't override
        #if false
		        // CVAR_ROM always overrides
		        if ( flags & CVAR_ROM ) {
			        Cvar_Set2( var_name, var_value, qtrue );
		        }
        #endif
		        return var;
            }

            varhashval = cvarpool.Count;

            // Create a new cvar.
            idCVarLocal cvar = new idCVarLocal(var_name, var_value, flags);
            cvarpool.Add(cvar);

            return cvarpool[varhashval];
        }

        /*
        ============
        Cvar_WriteVariables

        Appends lines containing "set variable value" for all variables
        with the archive flag set to qtrue.
        ============
        */
        public override void WriteVariables(ref idFile f) 
        {
	        idCVarLocal var;
	        string buffer = "";

	        for( int i = 0; i < cvarpool.Count; i++ )
            {
                var = cvarpool[i];

		        if ( var.GetName() == "cl_cdkey") {
			        continue;
		        }

		        if ( (var.flags & idCVar.CVAR_ARCHIVE) != 0 ) {
			        // write the latched value, even if it hasn't taken effect yet
			        if ( var.latchedString != null ) {
                        buffer = "seta " + var.GetName() + "\"" + var.latchedString + "\"\n";
			        } else {
                        buffer = "seta " + var.GetName() + "\"" + var.GetValue() + "\"\n";
			        }

                    f.WriteString(buffer);
		        }
	        }
        }

        //
        // Init
        //
        public override void Init()
        {
            cvar_cheats = Cvar_Get("sv_cheats", "0", idCVar.CVAR_ROM | idCVar.CVAR_SYSTEMINFO);
        }
    }
}
