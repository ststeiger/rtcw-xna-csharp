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

// Anim_Cfg.cs (c) 2010 JV Software
//

using System.Collections.Generic;

using idLib;
using idLib.Engine.Public;

namespace Game.Anim
{
    //
    // idAnim
    //
    public class idAnim
    {
        public int _firstFrame;
        public int _numFrames;
        public int _loopFrames;
        public int _frameLerp;
        public int _initialLerp;
        public int _moveSpeed;
        public int _priority;
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public idAnim(string name, ref idParser parser)
        {
            string token;

            this.name = name;

            // First Frame.
            token = parser.GetNextTokenFromLine();
            if (token != null)
            {
                if(!int.TryParse(token, out _firstFrame))
                {
                    name += " " + token;
                    token = parser.GetNextTokenFromLine();
                    _firstFrame = int.Parse(token);
                }
            }

            // NumFrames
            token = parser.GetNextTokenFromLine();
            if (token != null)
            {
                _numFrames = int.Parse(token);
            }

            // LoopFrames
            token = parser.GetNextTokenFromLine();
            if (token != null)
            {
                _loopFrames = int.Parse(token);
            }

            // Frame Lerp
            token = parser.GetNextTokenFromLine();
            if (token != null)
            {
                _frameLerp = int.Parse(token);
            }

            // initialLerp
            token = parser.GetNextTokenFromLine();
            if (token != null)
            {
                _initialLerp = int.Parse(token);
            }

            // moveSpeed
            token = parser.GetNextTokenFromLine();
            if (token != null)
            {
                _moveSpeed = int.Parse(token);
            }

            // priority
            token = parser.GetNextTokenFromLine();
            if (token != null)
            {
                _priority = int.Parse(token);
            }
        }
    }

    //
    // idAnimCfg
    //
    public class idAnimCfg
    {
        List<idAnim> animpool = new List<idAnim>();

        public idAnim this[int index]
        {
            get
            {
                return animpool[index];
            }
        }

        //
        // FindAnim
        //
        public idAnim FindAnim(string name)
        {
            for (int i = 0; i < animpool.Count; i++)
            {
                if (animpool[i].Name == name)
                {
                    return animpool[i];
                }
            }

            Engine.common.Warning("idAnimCfg::FindAnim: Failed to find anim " + name + "\n");
            return null;
        }

        //
        // idAnimCfg
        //
        public idAnimCfg(string path)
        {
            idFile file;
            idParser parser;

            bool parseAnims = false;
                
            file = Engine.fileSystem.OpenFileRead(path, true);
            if (file == null)
            {
                Engine.common.ErrorFatal("idAnimCfg Failed to load " + path + "\n");
            }

            parser = new idParser(file);

            parser.ExpectNextToken("VERSION");
            parser.ExpectNextToken("2");

            parser.ExpectNextToken("SKELETAL");

            while (true)
            {
                string token = parser.NextToken;

                if (token == null || token.Length <= 0)
                    Engine.common.ErrorFatal("Unexpected EOF in animcfg.\n");

                if (token == "ENDANIMS")
                {
                    break;
                }
                else if (token == "speed")
                {
                    token = parser.NextToken;
                }
                else if (token == "STARTANIMS")
                {
                    parseAnims = true;
                }
                else if (parseAnims)
                {
                    idAnim anim = new idAnim(token, ref parser);

                    animpool.Add(anim);
                }
                else
                {
                    Engine.common.ErrorFatal("Invalid or unexpected token " + token + "\n");
                }
            }

            parser.Dispose();
            Engine.fileSystem.CloseFile(ref file);
        }
    }
}
