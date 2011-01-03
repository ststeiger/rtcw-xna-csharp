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
            Level.script = Engine.fileSystem.ReadContent<idScript>( "maps/scripts/" + Level.mapname );
            Level.aiscript = Engine.fileSystem.ReadContent<idScript>("maps/aiscripts/" + Level.mapname);
        }

        //
        // Frame
        //
        public override void Frame()
        {

        }
    }
}
