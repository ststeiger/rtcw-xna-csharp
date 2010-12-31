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

// CmdManager.cs (c) 2010 JV Software
//

using System;
using System.Collections.Generic;

using idLib;
using idLib.Engine.Public;

namespace rtcw.Framework
{
    //
    // idCmdManagerLocal
    //
    public class idCmdManagerLocal : idCmdManager
    {
        private int cmd_wait = 0;
        private string cmd_text = "";

        /*
        ============
        Cmd_Wait_f

        Causes execution of the remainder of the command buffer to be delayed until
        next frame.  This allows commands like:
        bind g "cmd use rocket ; +attack ; wait ; -attack ; cmd use blaster"
        ============
        */
        private void Cmd_Wait_f() {
	        if ( Cmd_Argc() == 2 ) {
		        cmd_wait = int.Parse( Cmd_Argv( 1 ) );
	        } else {
		        cmd_wait = 1;
	        }
        }

        /*
        =============================================================================

                                COMMAND BUFFER

        =============================================================================
        */

        //
        // Cbuf_AddText
        //
        public override void Cbuf_AddText(string text)
        {
            cmd_text += text;
        }

        /*
        ============
        Cbuf_InsertText

        Adds command text immediately after the current command
        Adds a \n to the text
        ============
        */
        public override void Cbuf_InsertText(string text)
        {
            Cbuf_AddText(text + '\n');
        }

        /*
        ============
        Cbuf_ExecuteText
        ============
        */
        public override void Cbuf_ExecuteText( cbufExec_t exec_when, string text ) {
	        switch ( exec_when )
	        {
	            case cbufExec_t.EXEC_NOW:
		            if ( text != null && text.Length > 0 ) {
			            Cmd_ExecuteString( text );
		            } else {
			            Cbuf_Execute();
		            }
		            break;
	            case cbufExec_t.EXEC_INSERT:
		            Cbuf_InsertText( text );
		            break;
	            case cbufExec_t.EXEC_APPEND:
		            Cbuf_AddText( text );
		            break;
	            default:
		            Engine.common.ErrorFatal( "Cbuf_ExecuteText: bad exec_when\n" );
                    break;
	        }
        }

        /*
        ============
        Cbuf_Execute
        ============
        */
        public override void Cbuf_Execute() 
        {
	        int i;
            int cmd_text_pos = 0;
	        string text;
	        string line;
	        int quotes;

            

            while (cmd_text_pos < cmd_text.Length)
	        {
		        if ( cmd_wait != 0 ) {
			        // skip out while text still remains in buffer, leaving it
			        // for next frame
			        cmd_wait--;
			        break;
		        }

                // find a \n or ; line break
                line = "";
                text = cmd_text;
		        quotes = 0;
                for (i = cmd_text_pos; i < cmd_text.Length; i++)
		        {
                    cmd_text_pos++;

			        if ( text[i] == '"' ) {
				        quotes++;
			        }
			        if ( ( quotes & 1 ) == 0 &&  text[i] == ';' ) {
				        break;  // don't break if inside a quoted string
			        }
			        if ( text[i] == '\n' || text[i] == '\r' ) {
				        break;
			        }

                    line += text[i];
		        }

                // execute the command line
		        Cmd_ExecuteString( line );
	        }

            cmd_text = "";
        }

        /*
        ==============================================================================

						        SCRIPT COMMANDS

        ==============================================================================
        */

        /*
        ===============
        Cmd_Exec_f
        ===============
        */
        private void Cmd_Exec_f() {
	        string f;
            idFile file;
	        string filename;

	        if ( Cmd_Argc() != 2 ) {
		        Engine.common.Warning( "exec <filename> : execute a script file\n" );
		        return;
	        }

            filename = Cmd_Argv(1);

	        if(Engine.fileSystem.HasExtension( filename, ".cfg" ) == false)
            {
                filename += ".cfg";
            }

            file = Engine.fileSystem.OpenFileRead( filename, true );
            if( file == null || file.Length() <= 0)
            {
                if( file != null )
                {
                    Engine.fileSystem.CloseFile( ref file );
                }
                Engine.common.Warning("couldn't exec %s\n", Cmd_Argv(1));
		        return;
            }

            f = new string(file.ReadChars( file.Length() ));

	        Engine.common.Printf( "execing %s\n",Cmd_Argv( 1 ) );

	        Cbuf_InsertText( f );

            Engine.fileSystem.CloseFile(ref file);
        }

        /*
        ===============
        Cmd_Vstr_f

        Inserts the current value of a variable as command text
        ===============
        */
        private void Cmd_Vstr_f() {
            Engine.common.Warning("VStr not implemented\n");
        }

        /*
        ===============
        Cmd_Echo_f

        Just prints the rest of the line to the console
        ===============
        */
        private void Cmd_Echo_f() {
	        int i;

	        for ( i = 1 ; i < Cmd_Argc() ; i++ )
		        Engine.common.Printf( "%s ",Cmd_Argv( i ) );
            Engine.common.Printf("\n");
        }

        /*
        =============================================================================

					        COMMAND EXECUTION

        =============================================================================
        */
        private int cmd_argc = 0;
        private string[] cmd_argv = new string[256];

        //
        // cmd_function_t
        //
        private struct cmd_function_t
        {
	        public string name;
	        public xcommand_t function;
        };
        private List<cmd_function_t> cmd_functions = new List<cmd_function_t>();

        //
        // Init
        //
        public override void Init()
        {
            //Cmd_AddCommand("cmdlist", Cmd_List_f);
            Cmd_AddCommand("exec", Cmd_Exec_f);
            Cmd_AddCommand("vstr", Cmd_Vstr_f);
            Cmd_AddCommand("echo", Cmd_Echo_f);
            Cmd_AddCommand("wait", Cmd_Wait_f);
        }

        //
        // Cmd_Argc
        //
        public override int Cmd_Argc()
        {
            return cmd_argc;
        }

        //
        // Cmd_Argv
        //
        public override string Cmd_Argv(int arg)
        {
            if (arg >= cmd_argc)
            {
                return "";
            }
            return cmd_argv[arg];
        }

        //
        // Cmd_Args
        //
        public override string Cmd_Args()
        {
            return Cmd_ArgsFrom(1);
        }

        //
        // Cmd_ArgsFrom
        //
        public override string Cmd_ArgsFrom(int arg)
        {
            string cmd_args = "";
            int i;

            if (arg < 0)
            {
                arg = 0;
            }

            for (i = arg; i < cmd_argc; i++)
            {
                cmd_args += cmd_argv[i];
                if (i != cmd_argc - 1)
                {
                    cmd_args += " ";
                }
            }

            return cmd_args;
        }

        //
        // Cmd_TokenizeString
        //
        public override void Cmd_TokenizeString(string text_in)
        {
            idParser parser;

            // clear previous args
            cmd_argc = 0;

            if (text_in.Contains(" ") == false && text_in.Length > 0)
            {
                cmd_argv[cmd_argc++] = text_in;
                return;
            }
            parser = new idParser(text_in);

            // Parse the entire buffer.
            while (parser.ReachedEndOfBuffer == false)
            {
                cmd_argv[cmd_argc++] = parser.NextToken;
            }
        }

        //
        // Cmd_AddCommand
        //
        public override void Cmd_AddCommand(string cmd_name, xcommand_t function)
        {
            // fail if the command already exists
            foreach (cmd_function_t cmd in cmd_functions)
            {
                if (cmd.name == cmd_name)
                {
                    if (function != null)
                    {
                        Engine.common.Printf("Cmd_AddCommand: %s already defined\n", cmd_name);
                    }

                    return;
                }
            }

            cmd_function_t newcmd = new cmd_function_t();

            newcmd.name = cmd_name;
            newcmd.function = function;

            cmd_functions.Add(newcmd);
        }

        //
        // Cmd_ExecuteString
        //
        public override void Cmd_ExecuteString(string text)
        {
            // execute the command line
	        Cmd_TokenizeString( text );
	        if ( Cmd_Argc() <= 0 ) {
		        return;     // no tokens
	        }

            foreach (cmd_function_t func in cmd_functions)
            {
                if (func.name == cmd_argv[0])
                {
                    if (func.function != null)
                    {
                        func.function();
                        break;
                    }
                }
            }
        }

        //
        // Cmd_RemoveCommand
        //
        public override void Cmd_RemoveCommand(string cmd_name)
        {
            for (int i = 0; i < cmd_functions.Count; i++)
            {
                if (cmd_functions[i].name == cmd_name)
                {
                    cmd_functions.RemoveAt(i);
                    return;
                }
            }

            Engine.common.Warning("RemoveCommand: Command %s wasn't active\n", cmd_name);
        }
    }
}
