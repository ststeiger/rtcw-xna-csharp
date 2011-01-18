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

// dict.cs (c) 2010 id Software
//

using System;
using System.IO;
using System.Collections.Generic;

using idLib.Math;
using idLib.Engine.Public;

namespace idLib
{
    //
    // idKey
    //
    public struct idKey
    {
        public string name;
        public string val;
    }

    //
    // idDict
    //
    public class idDict
    {
        List<idKey> keys = new List<idKey>();

        //
        // Index
        //
        public idKey this[int index]
        {
            get
            {
                return keys[index];
            }
        }

        //
        // NumKeys
        //
        public int NumKeys
        {
            get
            {
                return keys.Count;
            }
        }

        //
        // InitFromFile
        //
        public void InitFromFile(ref idFile file)
        {
            int numKeys = file.ReadInt();
            for (int i = 0; i < numKeys; i++)
            {
                idKey newKey = new idKey();

                newKey.name = file.ReadString();
                newKey.val = file.ReadString();

                keys.Add(newKey);
            }
        }

        //
        // WriteToStream
        //
        public void WriteToStream(ref BinaryWriter writer)
        {
            writer.Write(keys.Count);

            for (int i = 0; i < keys.Count; i++)
            {
                writer.Write(keys[i].name);
                writer.Write(keys[i].val);
            }
        }

        //
        // AddKey
        //
        public void AddKey(string keyname, string val)
        {
            idKey key = new idKey();

            key.name = keyname.ToLower();
            key.val = val;

            if (val == null)
                throw new Exception(keyname + " has a null value");

            keys.Add(key);
        }

        //
        // AddKey
        //
        public void AddKey(string keyname, int val)
        {
            idKey key = new idKey();

            key.name = keyname.ToLower();
            key.val = "" + val;

            keys.Add(key);
        }

        //
        // FindKeyInt
        //
        public int FindKeyInt(string keyname)
        {
            string keyval = FindKey(keyname);

            if (keyval == null)
            {
                return 0;
            }

            return Int32.Parse(keyval);
        }


        //
        // FindKeyFloat
        //
        public float FindKeyFloat(string keyname)
        {
            string keyval = FindKey(keyname);

            if (keyval == null)
            {
                return 0;
            }

            return float.Parse(keyval);
        }

        //
        // FindKeyidVector3
        //
        public idVector3 FindKeyidVector3(string keyname)
        {
            string keyval = FindKey(keyname);
            idParser parser;
            idVector3 ret = new idVector3();

            if (keyval == null)
            {
                return new idVector3(0, 0, 0);
            }

            parser = new idParser(keyval);

            ret.X = parser.NextFloat;
            ret.Y = parser.NextFloat;
            ret.Z = parser.NextFloat;

            parser.Dispose();

            return ret;
        }

        //
        // FindKey
        //
        public string FindKey(string keyname)
        {
            keyname = keyname.ToLower();
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].name == keyname)
                {
                    return keys[i].val;
                }
            }
            return null;
        }

        //
        // Dipose
        //
        public void Dispose()
        {
            keys.Clear();
        }
    }
}
