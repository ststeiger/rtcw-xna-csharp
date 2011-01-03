// script.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework.Content;

using idLib.Game;

namespace Game
{
    //
    // idMaterialTableReader
    //
    class idScriptReader : ContentTypeReader<idScript>
    {
        //
        // jvFileList
        //
        protected override idScript Read(ContentReader input, idScript existingInstance)
        {
            idScript newScript = new idScript(input);

            return newScript;
        }
    }

    //
    // idScript
    //
    public class idScript
    {
        idScriptEvent[] events;

        //
        // idScript
        //
        public idScript(ContentReader input)
        {
            int numEvents = input.ReadInt16();

            events = new idScriptEvent[numEvents];

            for (int i = 0; i < numEvents; i++)
            {
                events[i].ReadFromFile(ref input);
            }
        }
    }
}
