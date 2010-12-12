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

// SoundManager.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using idLib.Engine.Public;
using rtcw.Framework.Files;

namespace rtcw.Sound
{
    //
    // idSoundSteaming
    //
    public class idSoundSteaming : idSound
    {
        DynamicSoundEffectInstance dynamicBuffer;

        //
        // idSoundLocal
        //
        public idSoundSteaming(int bitrate, int numChannels)
        {
            dynamicBuffer = new DynamicSoundEffectInstance(bitrate, (AudioChannels)numChannels);
            
        }

        //
        // BlitSoundData
        //
        public override void BlitSoundData(byte[] buffer, int offset, int length)
        {
            dynamicBuffer.SubmitBuffer(buffer, offset, length);
        }

        public override void Play()
        {
            if (dynamicBuffer.State == SoundState.Stopped)
            {
                dynamicBuffer.Play();
            }
            else
            {
                dynamicBuffer.Stop();
            }
        }

        public override void Dipose()
        {
            dynamicBuffer.Dispose();
        }

        public override void SetVolume(float vol)
        {
            dynamicBuffer.Volume = vol;
        }

        public override void Stop()
        {
            dynamicBuffer.Stop();
        }

        public override void Pause()
        {
            dynamicBuffer.Pause();
        }

        public override string GetName()
        {
            return "dynamicsound";
        }
    }

    //
    // idSoundLocal
    //
    public class idSoundLocal : idSound
    {
        SoundEffect handle;
        SoundEffectInstance instance;
        string name;

        //
        // idSoundLocal
        //
        public idSoundLocal(string fileName)
        {
            handle = Engine.fileSystem.ReadContent<SoundEffect>(fileName);
            name = fileName;
        }

        //
        // BlitSoundData
        //
        public override void BlitSoundData(byte[] buffer, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public override void Play()
        {
            if (instance == null)
            {
                instance = handle.CreateInstance();
                instance.Play();
            }
            else
            {
                instance.Resume();
            }
        }

        public override void Dipose()
        {
            if (instance != null)
            {
                instance.Dispose();
            }

            handle.Dispose();
        }

        public override void SetVolume(float vol)
        {
            instance.Volume = vol;
        }

        public override void Stop()
        {
            instance.Stop();
        }

        public override void Pause()
        {
            instance.Pause();
        }

        public override string GetName()
        {
            return name;
        }
    }

    //
    // idSoundManagerLocal
    //
    public class idSoundManagerLocal : idSoundManager
    {
        List<idSound> soundpool = new List<idSound>();

        //
        // Init
        //
        public override void Init()
        {
            
        }

        //
        // Update
        //
        public override void Update()
        {
            
        }

        //
        // LoadSound
        //
        public override idSound LoadSound(string fileName)
        {
            idSound sound;

            // Check to see if the sound is already loaded.
            for (int i = 0; i < soundpool.Count; i++)
            {
                if (soundpool[i].GetName() == fileName)
                {
                    return soundpool[i];
                }
            }

            // Check to see if the sound exists.
            if (!Engine.fileSystem.FileExists(fileName))
            {
                Engine.common.Warning("S_LoadSound: Failed to find sound " + fileName + " defaulting...\n");
                return null;
            }

            sound = new idSoundLocal(fileName);
            soundpool.Add(sound);

            return soundpool[soundpool.Count - 1];
        }

        //
        // CreateStreamingSound
        //
        public override idSound CreateStreamingSound(int bitrate, int numChannels)
        {
            return new idSoundSteaming(bitrate, numChannels);
        }
       
    }
}
