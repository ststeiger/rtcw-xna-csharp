﻿// sys_input.cs (c) 2010 JV Software
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

        int window_center_x = -1;
        int window_center_y = -1;
#elif XBOX360
        GamePadState gamepad;
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

        //
        // GetWindowMouseInput
        //
        private void GetWindowMouseInput()
        {
            MouseState mouseState = Mouse.GetState();
            int mx, my;
            idSysLocal sys = (idSysLocal)Engine.Sys;

            if( window_center_x < 0 )
            {
                window_center_x = ( idSysLocal.windowRect.Right + idSysLocal.windowRect.Left ) / 4;
	            window_center_y = ( idSysLocal.windowRect.Top + idSysLocal.windowRect.Bottom ) / 4;
                Mouse.SetPosition(window_center_x, window_center_y);
                return;
            }

            mx = mouseState.X - window_center_x;
            my = mouseState.Y - window_center_y;

            Mouse.SetPosition(window_center_x, window_center_y);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                sys.Sys_QueEvent(sys.Sys_Milliseconds(), sysEventType_t.SE_KEY, (int)keyNum.K_MOUSE1 + 0, 1, 0, null);
            }
            else if (mouseState.RightButton == ButtonState.Pressed)
            {
                sys.Sys_QueEvent(sys.Sys_Milliseconds(), sysEventType_t.SE_KEY, (int)keyNum.K_MOUSE1 + 1, 1, 0, null);
            }
            else if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                sys.Sys_QueEvent(sys.Sys_Milliseconds(), sysEventType_t.SE_KEY, (int)keyNum.K_MOUSE1 + 2, 1, 0, null);
            }



            if (mx == 0 || my == 0)
            {
                return;
            }

            sys.Sys_QueEvent(0, sysEventType_t.SE_MOUSE, mx, my, 0, null);
        }
#endif

#if XBOX360
        //
        // Get360GamepadState
        //
        private void Get360GamepadState()
        {
            idSysLocal sys = (idSysLocal)Engine.Sys;
            gamepad = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);

            if (gamepad.Buttons.A == ButtonState.Pressed)
            {
                sys.Sys_QueEvent(sys.Sys_Milliseconds(), sysEventType_t.SE_KEY, (int)keyNum.K_ESCAPE, 1, 0, null);
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
            GetWindowMouseInput();
#elif XBOX360
            Get360GamepadState();
#endif
        }
    }
}
