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

// Common.cs (c) 2010 JV Software
//

using System;
using System.Diagnostics;
using idLib.Engine.Public;
using rtcw.Server;
using rtcw.Client;


namespace rtcw.Framework
{
    //
    // idCommonLocal
    //
    public class idCommonLocal : idCommon
    {
        private string formatedMessage = "";

        public static idCVar com_viewlog;
        public static idCVar com_speeds;
        public static idCVar com_developer;
        public static idCVar com_dedicated;
        public static idCVar com_timescale;
        public static idCVar com_fixedtime;
        public static idCVar com_dropsim;       // 0.0 to 1.0, simulated packet drops
        public static idCVar com_journal;
        public static idCVar com_maxfps;
        public static idCVar com_timedemo;
        public static idCVar com_sv_running;
        public static idCVar com_cl_running;
        public static idCVar com_logfile;       // 1 = buffer log, 2 = flush after each print
        public static idCVar com_showtrace;
        public static idCVar com_version;
        public static idCVar com_blood;
        public static idCVar com_buildScript;   // for automated data building scripts
        public static idCVar com_introPlayed;
        public static idCVar cl_paused;
        public static idCVar sv_paused;
        public static idCVar com_cameraMode;
        public static idCVar com_recommendedSet;
        public static idCVar com_hunkused;

        // bk001129 - here we go again: upped from 64
        const int MAX_PUSHED_EVENTS    = 256;
        // bk001129 - init
        private int com_pushedEventsHead = 0;
        private int com_pushedEventsTail = 0;
        // bk001129 - static
        private sysEvent_t[] com_pushedEvents = new sysEvent_t[MAX_PUSHED_EVENTS];

        private int com_eventprintedWarning = 0;
        private idServerManager sv = new idServerManager();
        private idClientManager cl = new idClientManager();

        private int com_frameTime = 0;
        private bool com_fullyInitialized = false;

        /*
        =================
        Com_InitPushEvent
        =================
        */
        // bk001129 - added
        private void Com_InitPushEvent() {
	        // clear the static buffer array
	        // this requires SE_NONE to be accepted as a valid but NOP event
            //for( int i = 0; i < MAX_PUSHED_EVENTS; i++ )
            //{
                //com_pushedEvents[i] = new sysEvent_t();
            //}
	        //memset( com_pushedEvents, 0, sizeof( com_pushedEvents ) );
	        // reset counters while we are at it
	        // beware: GetEvent might still return an SE_NONE from the buffer
	        com_pushedEventsHead = 0;
	        com_pushedEventsTail = 0;
        }

        //
        // ForceGCCollect
        //
        public override void ForceGCCollect()
        {
            GC.Collect();
        }

        /*
        =================
        Com_PushEvent
        =================
        */
        public override void Com_PushEvent( sysEvent_t new_event ) 
        {
	        sysEvent_t      ev;

	        ev = com_pushedEvents[ com_pushedEventsHead & ( MAX_PUSHED_EVENTS - 1 ) ];

	        if ( ev != null && com_pushedEventsHead - com_pushedEventsTail >= MAX_PUSHED_EVENTS ) {

		        // don't print the warning constantly, or it can give time for more...
		        if ( com_eventprintedWarning == 0 ) {
			        com_eventprintedWarning = 1;
			        Warning( "Com_PushEvent overflow\n" );
		        }

		        if ( ev.evPtr.ptr != null ) {
                    ev.evPtr.ptr.Dispose();
                    ev.evPtr = null;
		        }
		        com_pushedEventsTail++;
	        } else {
		        com_eventprintedWarning = 0;
	        }

            com_pushedEvents[com_pushedEventsHead & (MAX_PUSHED_EVENTS - 1)] = new_event;
	        com_pushedEventsHead++;
        }

        /*
        =================
        Com_GetEvent
        =================
        */
        private sysEvent_t  Com_GetEvent() {
	        if ( com_pushedEventsHead > com_pushedEventsTail ) {
		        com_pushedEventsTail++;
		        return com_pushedEvents[ ( com_pushedEventsTail - 1 ) & ( MAX_PUSHED_EVENTS - 1 ) ];
	        }
            return Engine.Sys.GetEvent();
        }

        /*
        ================
        Com_Milliseconds

        Can be used for profiling, but will be journaled accurately
        ================
        */
        public override int Com_Milliseconds() {
	        sysEvent_t ev;

	        // get events and push them until we get a null event with the current time
	        do {

                ev = Engine.Sys.GetEvent();
                if (ev.evType != sysEventType_t.SE_NONE)
                {
			        Com_PushEvent( ev );
		        }
            } while (ev.evType != sysEventType_t.SE_NONE);

	        return ev.evTime;
        }

        //
        // ScaledMilliseconds
        //
        public override int ScaledMilliseconds()
        {
            return (int)(Engine.Sys.Sys_Milliseconds() * com_timescale.GetValueFloat());
        }

        /*
        ============================================================================

        COMMAND LINE FUNCTIONS

        + characters seperate the commandLine string into multiple console
        command lines.

        All of these are valid:

        quake3 +set test blah +map test
        quake3 set test blah+map test
        quake3 set test blah + map test

        ============================================================================
        */

        private const int MAX_CONSOLE_LINES = 32;
        int     com_numConsoleLines = 0;
        string[] com_consoleLines = new string[MAX_CONSOLE_LINES];

        /*
        ==================
        Com_ParseCommandLine

        Break it up into multiple console lines
        ==================
        */
        private void ParseCommandLine( string commandLine ) {
            int cmdLinePos = 0;
	        com_numConsoleLines = 0;

            com_consoleLines[0] = "";
            while (cmdLinePos < commandLine.Length)
            {
		        // look for a + seperating character
		        // if commandLine came from a file, we might have real line seperators
                if (commandLine[cmdLinePos] == '+' || commandLine[cmdLinePos] == '\n')
                {
                    if (com_numConsoleLines == MAX_CONSOLE_LINES)
                    {
                        return;
                    }
                    if (com_consoleLines[com_numConsoleLines].Length > 0)
                    {
                        com_numConsoleLines++;
                        com_consoleLines[com_numConsoleLines] = "";
                    }
                    
                    cmdLinePos++;
                }
                else
                {
                    com_consoleLines[com_numConsoleLines] += commandLine[cmdLinePos++];
                }
	        }

            if (com_numConsoleLines == 0)
            {
                com_numConsoleLines++;
            }
        }

        /*
        ===============
        Com_StartupVariable

        Searches for command line parameters that are set commands.
        If match is not NULL, only that cvar will be looked for.
        That is necessary because cddir and basedir need to be set
        before the filesystem is started, but all other sets shouls
        be after execing the config and default.
        ===============
        */
        private void StartupVariable( string match ) {
	        int i;
	        string s;
	        idCVarLocal cv;

	        for ( i = 0 ; i < com_numConsoleLines ; i++ ) {
		        Engine.cmdSystem.Cmd_TokenizeString( com_consoleLines[i] );
                if (Engine.cmdSystem.Cmd_Argv(0) != "set")
                {
			        continue;
		        }

                s = Engine.cmdSystem.Cmd_Argv(1);
		        if ( match == null || s == match ) 
                {
                    Engine.cvarManager.Cvar_Set(s, Engine.cmdSystem.Cmd_Argv(2), true);
                    cv = (idCVarLocal)Engine.cvarManager.Cvar_Get(s, "", 0);
			        cv.flags |= idCVar.CVAR_USER_CREATED;
        //			com_consoleLines[i] = 0;
		        }
	        }
        }

        /*
        =================
        Com_EventLoop

        Returns last event time
        =================
        */
        private int EventLoop() 
        {
	        sysEvent_t ev;
	        //netadr_t evFrom;
	        //byte bufData[MAX_MSGLEN];
	        //msg_t buf;

	       // MSG_Init( &buf, bufData, sizeof( bufData ) );

	        while ( true ) {
		        ev = Com_GetEvent();

		        // if no more events are available
		        if ( ev.evType == sysEventType_t.SE_NONE ) {
                    /*
			        // manually send packet events for the loopback channel
			        while ( NET_GetLoopPacket( NS_CLIENT, &evFrom, &buf ) ) {
				        CL_PacketEvent( evFrom, &buf );
			        }

			        while ( NET_GetLoopPacket( NS_SERVER, &evFrom, &buf ) ) {
				        // if the server just shut down, flush the events
				        if ( com_sv_running->integer ) {
					        Com_RunAndTimeServerPacket( &evFrom, &buf );
				        }
			        }
                    */

			        return ev.evTime;
		        }


		        switch ( ev.evType ) {
		        default:
			        // bk001129 - was ev.evTime
			        ErrorFatal( "Com_EventLoop: bad event type %i", ev.evType );
			        break;
		        case sysEventType_t.SE_NONE:
			        break;
		        case sysEventType_t.SE_KEY:
			        cl.KeyEvent( ev.evValue, (ev.evValue2 != 0), ev.evTime );
			        break;
		        case sysEventType_t.SE_CHAR:
			       // CL_CharEvent( ev.evValue );
			        break;
		        case sysEventType_t.SE_MOUSE:
			        //CL_MouseEvent( ev.evValue, ev.evValue2, ev.evTime );
			        break;
		        case sysEventType_t.SE_JOYSTICK_AXIS:
			       // CL_JoystickEvent( ev.evValue, ev.evValue2, ev.evTime );
			        break;
		        case sysEventType_t.SE_CONSOLE:
			        //Cbuf_AddText( (char *)ev.evPtr );
			       // Cbuf_AddText( "\n" );
			        break;
		        case sysEventType_t.SE_PACKET:
#if false
			        // this cvar allows simulation of connections that
			        // drop a lot of packets.  Note that loopback connections
			        // don't go through here at all.
			        if ( com_dropsim->value > 0 ) {
				        static int seed;

				        if ( Q_random( &seed ) < com_dropsim->value ) {
					        break;      // drop this packet
				        }
			        }

			        evFrom = *(netadr_t *)ev.evPtr;
			        buf.cursize = ev.evPtrLength - sizeof( evFrom );

			        // we must copy the contents of the message out, because
			        // the event buffers are only large enough to hold the
			        // exact payload, but channel messages need to be large
			        // enough to hold fragment reassembly
			        if ( (unsigned)buf.cursize > buf.maxsize ) {
				        Com_Printf( "Com_EventLoop: oversize packet\n" );
				        continue;
			        }
			        memcpy( buf.data, ( byte * )( (netadr_t *)ev.evPtr + 1 ), buf.cursize );
			        if ( com_sv_running->integer ) {
				        Com_RunAndTimeServerPacket( &evFrom, &buf );
			        } else {
				        CL_PacketEvent( evFrom, &buf );
			        }
#endif
			        break;
		        }

		        // free any block data
		        if ( ev.evPtr.ptr != null ) {
                    ev.evPtr.ptr.Dispose();
                    ev.evPtr.ptr = null;
		        }
	        }

	        //return 0;   // never reached
        }

        /*
        =================
        Com_AddStartupCommands

        Adds command line parameters as script statements
        Commands are seperated by + signs

        Returns qtrue if any late commands were added, which
        will keep the demoloop from immediately starting
        =================
        */
        private bool AddStartupCommands() {
	        int i;
	        bool added = false;

	        // quote every token, so args with semicolons can work
	        for ( i = 0 ; i < com_numConsoleLines ; i++ ) {
		        if ( com_consoleLines[i] == null || com_consoleLines[i].Length <= 0 ) {
			        continue;
		        }

		        // set commands won't override menu startup
		        if ( com_consoleLines[i].Contains( "set" ) == false )
                {
			        added = true;
		        }
		        Engine.cmdSystem.Cbuf_AddText( com_consoleLines[i] );
		        Engine.cmdSystem.Cbuf_AddText( "\n" );
	        }

	        return added;
        }

        //
        // Init
        //
        public override void Init(string commandLine)
        {
            Printf("%s %s %s\n", Engine.Q3_VERSION, Engine.CPUSTRING, Engine.BUILDNUM);

            // bk001129 - do this before anything else decides to push events
            Com_InitPushEvent();

            // Init the cvar manager.
            Engine.cvarManager.Init();

            // prepare enough of the subsystems to handle
            // cvar and command buffer management
            ParseCommandLine(commandLine);

            Engine.cmdSystem.Init();

            // override anything from the config files with command line args
            StartupVariable(null);

            // get the developer cvar set as early as possible
            StartupVariable("developer");

            Engine.fileSystem.Init();

            Engine.cmdSystem.Cbuf_AddText("exec default.cfg\n");
            Engine.cmdSystem.Cbuf_AddText("exec language.cfg\n"); //----(SA)	added

            Engine.cmdSystem.Cbuf_Execute();

            // override anything from the config files with command line args
            StartupVariable(null);
            com_developer = Engine.cvarManager.Cvar_Get("developer", "0", idCVar.CVAR_TEMP);

            // get dedicated here for proper hunk megs initialization
        #if DEDICATED
	        com_dedicated = Cvar_Get( "dedicated", "1", CVAR_ROM );
        #else
            com_dedicated = Engine.cvarManager.Cvar_Get("dedicated", "0", idCVar.CVAR_LATCH);
        #endif
	        // allocate the stack based hunk allocator
	        //Com_InitHunkMemory();

	        // if any archived cvars are modified after this, we will trigger a writing
	        // of the config file
            idCVarManagerLocal.cvar_modifiedFlags &= ~idCVar.CVAR_ARCHIVE;

	        //
	        // init commands and vars
	        //
            com_maxfps = Engine.cvarManager.Cvar_Get("com_maxfps", "85", idCVar.CVAR_ARCHIVE);
            com_blood = Engine.cvarManager.Cvar_Get("com_blood", "1", idCVar.CVAR_ARCHIVE);

            com_developer = Engine.cvarManager.Cvar_Get("developer", "0", idCVar.CVAR_TEMP);
            com_logfile = Engine.cvarManager.Cvar_Get("logfile", "0", idCVar.CVAR_TEMP);

            com_timescale = Engine.cvarManager.Cvar_Get("timescale", "1", idCVar.CVAR_CHEAT | idCVar.CVAR_SYSTEMINFO);
            com_fixedtime = Engine.cvarManager.Cvar_Get("fixedtime", "0", idCVar.CVAR_CHEAT);
            com_showtrace = Engine.cvarManager.Cvar_Get("com_showtrace", "0", idCVar.CVAR_CHEAT);
            com_dropsim = Engine.cvarManager.Cvar_Get("com_dropsim", "0", idCVar.CVAR_CHEAT);
            com_viewlog = Engine.cvarManager.Cvar_Get("viewlog", "0", idCVar.CVAR_CHEAT);
	        com_speeds = Engine.cvarManager.Cvar_Get( "com_speeds", "0", 0 );
            com_timedemo = Engine.cvarManager.Cvar_Get("timedemo", "0", idCVar.CVAR_CHEAT);
            com_cameraMode = Engine.cvarManager.Cvar_Get("com_cameraMode", "0", idCVar.CVAR_CHEAT);

            cl_paused = Engine.cvarManager.Cvar_Get("cl_paused", "0", idCVar.CVAR_ROM);
            sv_paused = Engine.cvarManager.Cvar_Get("sv_paused", "0", idCVar.CVAR_ROM);
            com_sv_running = Engine.cvarManager.Cvar_Get("sv_running", "0", idCVar.CVAR_ROM);
            com_cl_running = Engine.cvarManager.Cvar_Get("cl_running", "0", idCVar.CVAR_ROM);
	        com_buildScript = Engine.cvarManager.Cvar_Get( "com_buildScript", "0", 0 );

            com_introPlayed = Engine.cvarManager.Cvar_Get("com_introplayed", "0", idCVar.CVAR_ARCHIVE);
            com_recommendedSet = Engine.cvarManager.Cvar_Get("com_recommendedSet", "0", idCVar.CVAR_ARCHIVE);

            Engine.cvarManager.Cvar_Get("savegame_loading", "0", idCVar.CVAR_ROM);

            com_hunkused = Engine.cvarManager.Cvar_Get("com_hunkused", "0", 0);

            com_version = Engine.cvarManager.Cvar_Get("version", Engine.Q3_VERSION, idCVar.CVAR_ROM | idCVar.CVAR_SERVERINFO);

            Engine.Sys.Init();
            Engine.net.Netchan_Init(Com_Milliseconds() & 0xffff);    // pick a port value that should be nice and random.
            sv.Init();

            cl.Init();

            // set com_frameTime so that if a map is started on the
            // command line it will still be able to count on com_frameTime
            // being random enough for a serverid
            com_frameTime = Com_Milliseconds();

            // start in full screen ui mode
            Engine.cvarManager.Cvar_Set("r_uiFullScreen", "1", true);

            cl.StartHunkUsers();

            // jmarshall - moved this here so devmap will work.
            // add + commands from command line
            if (!AddStartupCommands())
            {
                // if the user didn't give any commands, run default action
                if (com_introPlayed.GetValueInteger() == 0)
                {
                    //Cvar_Set( com_introPlayed->name, "1" );		//----(SA)	force this to get played every time (but leave cvar for override)
                    Engine.cmdSystem.Cbuf_AddText("cinematic wolfintro 3\n");
                    //Cvar_Set( "nextmap", "cinematic wolfintro.RoQ" );
                }
            }

            com_fullyInitialized = true;
            Engine.common.Printf("--- Common Initialization Complete ---\n");
        }

        //
        // Printf
        //
        public override void Printf(string fmt, params object[] args)
        {
            int argNum = 0;

            formatedMessage = "";

            // Properly format the string for output.
            for (int i = 0; i < fmt.Length; i++)
            {
                if (fmt[i] == '%')
                {
                    switch (fmt[i + 1])
                    {
                        case 's':
                            formatedMessage += args[argNum++];
                            i++;
                            continue;
                        case 'd':
                            formatedMessage += args[argNum++];
                            i++;
                            continue;
                        case 'i':
                            formatedMessage += args[argNum++];
                            i++;
                            continue;
                        default:
                            break;
                    }
                }

                formatedMessage += fmt[i];
            }

            // Print the message to the VS debug screen.
#if XBOX360
            Debug.WriteLine(formatedMessage);
#else
            Debug.Write(formatedMessage);
#endif
        }

        //
        // DPrintf
        //
        public override void DPrintf(string fmt, params object[] args)
        {
            if (com_developer.GetValueInteger() == 0)
            {
                return;
            }
            Printf(fmt, args);
        }

        //
        // Warning
        //
        public override void Warning(string fmt, params object[] args)
        {
            Printf("WARNING: " + fmt, args);
        }

        //
        // ErrorFatal
        //
        public override void ErrorFatal(string fmt, params object[] args)
        {
            Printf("***********\n FATAL ERROR \n *************\n");
            Printf("ERROR MESSAGE: " + fmt, args);
            throw new Exception(formatedMessage);
        }

        //
        // ErrorDrop
        //
        public override void ErrorDrop(string fmt, params object[] args)
        {
            // Fix ME this should only DROP the client.
            ErrorFatal(fmt, args);
        }

        //
        // HandlePendingEvents
        //
        private void HandlePendingEvents()
        {
            // JV - Get the first frame, we don't need to skip frames since XNA does that for us.
            com_frameTime = EventLoop();

            // Execute any pending console commands.
            Engine.cmdSystem.Cbuf_Execute();
        }

        //
        // Frame
        //
        public override void Frame(int frameTime, int totalGameTime)
        {
            HandlePendingEvents();
            sv.Frame();

            //
            // run event loop a second time to get server to client packets
            // without a frame of latency
            //
            HandlePendingEvents();
            cl.Frame();
        }
    }
}
