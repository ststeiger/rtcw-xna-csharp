﻿/*
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

// game_network.cs (c) 2010 JV Software
//

using idLib;
using idLib.Engine.Public;

namespace Game
{
    //
    // idGameNetwork
    //
    public class idGameNetwork
    {
        string configstr;
        int numModels;
        int numSounds;
        int numSkins;
        idDict skinCache = new idDict();
        idDict modelCache = new idDict();
        idDict soundCache = new idDict();

        public string ConfigStr
        {
            get
            {
                return configstr;
            }
        }

        //
        // Reset
        //
        public void Reset()
        {
            numModels = 0;
            numSounds = 0;
            configstr = "";
        }

        //
        // ModelIndex
        //
        public int ModelIndex(string modelpath)
        {
            if (modelpath == null || modelpath.Length <= 0)
            {
                return -1;
            }

            int prevKey = modelCache.FindKeyInt(modelpath);
            if (prevKey > 0)
            {
                return prevKey - 1;
            }

            configstr += "model ";
            configstr += modelpath;
            configstr += " ";

            modelCache.AddKey(modelpath, numModels + 1);

            return numModels++;
        }

        //
        // SoundIndex
        //
        public int SoundIndex(string soundpath)
        {
            if (soundpath == null || soundpath.Length <= 0)
            {
                return -1;
            }

            int prevKey = soundCache.FindKeyInt(soundpath);
            if (prevKey > 0)
            {
                return prevKey - 1;
            }

            configstr += "sound ";
            configstr += soundpath;
            configstr += " ";

            soundCache.AddKey(soundpath, numSounds + 1);

            return numSounds++;
        }

        //
        // SkinIndex
        //
        public int SkinIndex(string skinpath)
        {
            if (skinpath == null || skinpath.Length <= 0)
            {
                return -1;
            }

            int prevKey = skinCache.FindKeyInt(skinpath);
            if (prevKey > 0)
            {
                return prevKey - 1;
            }

            configstr += "skin ";
            configstr += skinpath;
            configstr += " ";

            skinCache.AddKey(skinpath, numSkins + 1);

            return numSkins++;
        }

        //
        // PlaysoundForPlayer
        //
        public void PlaysoundForPlayer(idEntity entity, string path)
        {
            idSound snd = Engine.soundManager.LoadSound(path);
            snd.Play();
        }

        //
        // PlayBackgroundTrackForPlayer
        //
        public void PlayBackgroundTrackForPlayer(idEntity entity, string path)
        {
            // This is the wrong wya to do this but I'm testing on the phone atm.
            idSound music = Engine.soundManager.LoadBackgroundTrack(path);

            if (music != null)
                music.Play();
        }

        //
        // StopBackgroundTrackForPlayer
        //
        public void StopBackgroundTrackForPlayer(idEntity entity)
        {
            Engine.soundManager.StopBackgroundTrack();
        }
    }
}
