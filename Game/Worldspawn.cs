// Worldspawn.cs (c) 2010 JV Software
//

using System;
using idLib.Engine.Public;

namespace Game
{
    //
    // idWorldspawn
    //
    public class idWorldspawn : idEntity
    {
        //
        // Spawn
        //
        public override void Spawn()
        {
            Level.script = new idScript("maps/scripts/" + Level.mapname);
            Level.aiscript = new idScript("maps/aiscripts/" + Level.mapname);
        }

        //
        // Frame
        //
        public override void Frame()
        {

        }
    }
}
