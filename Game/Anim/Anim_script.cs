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

// Anim_script.cs (c) 2010 JV Software
//

using System.Collections.Generic;

using idLib;
using idLib.Engine.Public;

namespace Game.Anim
{
    //
    // idAnimScript
    //
    public class idAnimScript
    {
        List<idAnimScriptDefine> defines = new List<idAnimScriptDefine>();
        List<idAnimScriptAnimationState> animationstates = new List<idAnimScriptAnimationState>();
        List<idAnimScriptAnimationState> canned_animationstates = new List<idAnimScriptAnimationState>();

        enum animScriptParseMode_t
        {
	        PARSEMODE_DEFINES,
	        PARSEMODE_ANIMATION,
	        PARSEMODE_CANNED_ANIMATIONS,
	        PARSEMODE_STATECHANGES,
	        PARSEMODE_EVENTS
        };

        //
        // GetAnimationForState
        //
        public idAnimScriptAnimation GetAnimationForState(string statename, string posename)
        {
            for (int i = 0; i < animationstates.Count; i++)
            {
                if (animationstates[i].StateName == statename)
                {
                    for (int c = 0; c < animationstates[i].NumPoses; c++)
                    {
                        if (animationstates[i][c].Name == posename)
                        {
                            return animationstates[i][c];
                        }
                    }
                }
            }

            Engine.common.Warning("GetAnimationForState: Failed to get animation for state %s \n", statename);
            return null;
        }

        //
        // InitFromFile
        //
        public void InitFromFile(string path)
        {
            idFile file;
            idParser parser;
            string token;
            animScriptParseMode_t parsemode = (animScriptParseMode_t)(-1);

            // Try to open the script for parsing.
            file = Engine.fileSystem.OpenFileRead(path, true);
            if (file == null)
            {
                Engine.common.ErrorFatal("idAnimScript::Init: Failed to open player animation script.\n");
                return;
            }

            // Init the parser.
            parser = new idParser(file);

            while (true)
            {
                token = parser.NextToken;
                if (token == null || token.Length <= 0)
                {
                    break;
                }

                token = token.ToLower();

                if (token == "defines")
                {
                    parsemode = animScriptParseMode_t.PARSEMODE_DEFINES;
                }
                else if (token == "animations")
                {
                    parsemode = animScriptParseMode_t.PARSEMODE_ANIMATION;
                }
                else if (token == "canned_animations")
                {
                    parsemode = animScriptParseMode_t.PARSEMODE_CANNED_ANIMATIONS;
                    break; // skip these for now.
                }
                else if (token == "statechanges")
                {
                    parsemode = animScriptParseMode_t.PARSEMODE_STATECHANGES;
                    break; // skip these for now.
                }
                else if (token == "events")
                {
                    parsemode = animScriptParseMode_t.PARSEMODE_EVENTS;
                    break; // skip these for now.
                }
                else
                {
                    parser.UngetToken();
                }

                switch (parsemode)
                {
                    case animScriptParseMode_t.PARSEMODE_DEFINES:
                        idAnimScriptDefine newDefine = new idAnimScriptDefine(ref parser);
                        defines.Add(newDefine);
                        break;

                    case animScriptParseMode_t.PARSEMODE_ANIMATION:
                        idAnimScriptAnimationState newanim = new idAnimScriptAnimationState(ref parser);
                        animationstates.Add(newanim);
                        break;

                    case animScriptParseMode_t.PARSEMODE_CANNED_ANIMATIONS:
                        idAnimScriptAnimationState newcannedanim = new idAnimScriptAnimationState(ref parser);
                        canned_animationstates.Add(newcannedanim);
                        break;

                    case animScriptParseMode_t.PARSEMODE_STATECHANGES:

                        break;

                    case animScriptParseMode_t.PARSEMODE_EVENTS:

                        break;

                    default:
                        Engine.common.ErrorFatal("idAnimScript::Init: Unknown parse mode.\n");
                        break;
                }
            }

            // Close the anim acript.
            Engine.fileSystem.CloseFile(ref file);

            // Dispose of the parser.
            parser.Dispose();
        }
    }

    //
    // idAnimScriptAnimPart
    //
    public struct idAnimScriptAnimPart
    {
        public enum idAnimPartType
        {
            ANIM_BOTH = 0,
            ANIM_LEGS,
            ANIM_TORSO
        }

        
        public string animname;
        
        public idAnimPartType type;
    }

    //
    // idAnimScriptAnimation
    //
    public class idAnimScriptAnimation
    {
        idAnimScriptAnimPart[] parts = new idAnimScriptAnimPart[5];
        int numparts = 0;
        int curpart = 0;

        string name;
        string movetype;

        public int CurrentPart
        {
            set
            {
                curpart = value;
            }
        }

        public int NumParts
        {
            get
            {
                return numparts;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        //
        // AnimName
        //
        public string AnimName
        {
            get
            {
                return parts[curpart].animname;
            }
        }

        //
        // MoveType
        //
        public string MoveType
        {
            get
            {
                return movetype;
            }
        }

        //
        // HasTorsoAnimation
        //
        public bool HasTorsoAnimation
        {
            get
            {
                if (parts[curpart].type == idAnimScriptAnimPart.idAnimPartType.ANIM_BOTH || parts[curpart].type == idAnimScriptAnimPart.idAnimPartType.ANIM_TORSO)
                {
                    return true;
                }

                return false;
            }
        }

        //
        // HasLegAnimation
        //
        public bool HasLegAnimation
        {
            get
            {
                if (parts[curpart].type == idAnimScriptAnimPart.idAnimPartType.ANIM_BOTH || parts[curpart].type == idAnimScriptAnimPart.idAnimPartType.ANIM_LEGS)
                {
                    return true;
                }

                return false;
            }
        }
        //
        // idAnimScriptAnimation
        //
        public idAnimScriptAnimation(string name, ref idParser parser)
        {
            string token;
            this.name = name;

            parser.ExpectNextToken("{");

            movetype = parser.NextToken;
            if (movetype == "movetype")
            {
                movetype = parser.NextToken;
            }
            else if (movetype == "weapons")
            {
                token = parser.NextToken;
            }

            parser.ExpectNextToken("{");

            while (true)
            {
                token = parser.NextToken;

                if (token == null || token.Length <= 0)
                {
                    Engine.common.ErrorFatal("Unexpected EOF in script anim.\n");
                }

                if (token == "}")
                {
                    break;
                }
                else if (token == "legs")
                {
                    parts[numparts].type = idAnimScriptAnimPart.idAnimPartType.ANIM_LEGS;
                }
                else if (token == "torso")
                {
                    parts[numparts].type = idAnimScriptAnimPart.idAnimPartType.ANIM_TORSO;
                }
                else if (token == "both")
                {
                    parts[numparts].type = idAnimScriptAnimPart.idAnimPartType.ANIM_BOTH;
                }
                else
                {
                    Engine.common.ErrorFatal("idAnimScriptAnimation: Invalid animation part state. \n");
                }

                parts[numparts].animname = parser.NextToken;
                numparts++;
            }
            parser.ExpectNextToken("}");
        }
    }

    //
    // idAnimScriptAnimationState
    //
    public class idAnimScriptAnimationState
    {
        string statename;
        List<idAnimScriptAnimation> animations = new List<idAnimScriptAnimation>();

        public int NumPoses
        {
            get
            {
                return animations.Count;
            }
        }

        public idAnimScriptAnimation this[int index]
        {
            get
            {
                return animations[index];
            }
        }

        public string StateName
        {
            get
            {
                return statename;
            }
        }

        //
        // idAnimScriptAnimations
        //
        public idAnimScriptAnimationState(ref idParser parser)
        {
            string token;

            parser.ExpectNextToken("STATE");
            statename = parser.NextToken;

            parser.ExpectNextToken("{");

            while (true)
            {
                token = parser.NextToken;

                if (token == null || token.Length <= 0)
                {
                    Engine.common.ErrorFatal("idAnimScriptAnimationState: Unexpected EOF. \n");
                }

                if (token == "}")
                {
                    break;
                }

                idAnimScriptAnimation newanim = new idAnimScriptAnimation(token, ref parser);
                animations.Add(newanim);
            }
        }
    }

    //
    // idAnimScriptDefine
    //
    public class idAnimScriptDefine
    {
        public string definetype;
        public string name;
        public string[] vals = new string[20];
        public int numvals = 0;

        //
        // idAnimScriptDefine
        //
        public idAnimScriptDefine(ref idParser parser)
        {
            parser.ExpectNextToken("set");

            definetype = parser.NextToken;

            parser.ParseRestOfLine();
            return;

            if (definetype == "weapons")
            {
                ParseWeaponDefine(ref parser);
            }
            else
            {
                Engine.common.ErrorFatal("idAnimScriptDefine: Unknown define type %s \n", definetype);
            }
        }

        //
        // ParseWeaponDefine
        //
        private void ParseWeaponDefine(ref idParser parser)
        {
            string token;
            name = parser.NextToken;

            while (true)
            {
                token = parser.GetNextTokenFromLine();

                if (token == null || token.Length <= 0)
                    break;

                vals[numvals++] = token;
            }
        }
    }
}
