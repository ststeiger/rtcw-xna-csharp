// sys_input.cs (c) 2010 JV Software
//

using System;
using Microsoft.Xna.Framework.Input;
using idLib.Engine.Public;

namespace rtcw.sys
{
    //
    // idSystemInput
    //
    class idSystemInput
    {
#if WINDOWS
        Keys[] lastFrameKeys;
#endif
        //
        // Init
        //
        public void Init()
        {

        }

#if WINDOWS
        //
        // GetWindowsKeyInput
        //
        private void GetWindowsKeyInput()
        {
            KeyboardState keyState = Keyboard.GetState();
            Keys[] downKeys;
            idSysLocal sys = (idSysLocal)Engine.Sys;

            // Get all the keys that are currently down.
            downKeys = keyState.GetPressedKeys();

            foreach(Keys key in downKeys)
            {
                sys.Sys_QueEvent(sys.Sys_Milliseconds(), sysEventType_t.SE_KEY, (int)key, 1, 0, null);
            }
        }
#endif

        //
        // Frame
        //
        public void Frame()
        {
#if WINDOWS
            GetWindowsKeyInput();
#endif
        }
    }
}
