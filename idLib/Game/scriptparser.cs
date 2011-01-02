// gamescript_parser.cs (c) 2010 JV Software
//

using System;
using System.IO;
using System.Collections.Generic;
using idLib;
using idLib.Engine.Public;

namespace idLib.Game
{
    //
    // idScriptEvent
    //
    public class idScriptEvent
    {
        public string name;
        public idScriptAction[] actionsraw;
        public List<idScriptAction> actions;

        //
        // ReadFromFile
        //
        public void ReadFromFile(ref idFile file)
        {
            name = file.ReadString();
            actionsraw = new idScriptAction[file.ReadShort()];

            for (int i = 0; i < actionsraw.Length; i++)
            {
                actionsraw[i].ReadFromFile(ref file);
            }
        }

        //
        // WriteToFile
        //
        public void WriteToFile(ref BinaryWriter writer)
        {
            writer.Write(name);
            writer.Write((short)actionsraw.Length);

            for (int i = 0; i < actionsraw.Length; i++)
            {
                actionsraw[i].WriteToFile(ref writer);
            }
        }

        public bool Parse(ref idParser parser)
        {
            string token;
            name = parser.NextToken;

            if (name == null || name.Length <= 0)
                return false;

            parser.ExpectNextToken("{");

            actions = new List<idScriptAction>();

            while (true)
            {
                if (parser.ReachedEndOfBuffer == true)
                    throw new Exception("Unexpected EOF in event " + name);

                token = parser.NextToken;

                if (token == "}")
                    break;

                parser.UngetToken();

                idScriptAction action = new idScriptAction();
                action.Parse(ref parser);
                actions.Add(action);
            }

            actionsraw = actions.ToArray();
            actions.Clear();
            actions = null;

            return true;
        }
    }

    //
    // idScriptAction
    //
    public class idScriptAction
    {
        public string type;
        public string name;
        public idScriptFuncBinary[] funcsraw;
        public List<idScriptFuncBinary> funcs;

        //
        // ReadFromFile
        //
        public void ReadFromFile(ref idFile file)
        {
            type = file.ReadString();
            name = file.ReadString();
            funcsraw = new idScriptFuncBinary[file.ReadShort()];

            for (int i = 0; i < funcsraw.Length; i++)
            {
                funcsraw[i].ReadFromFile(ref file);
            }
        }

        //
        // WriteToFile
        //
        public void WriteToFile(ref BinaryWriter writer)
        {
            writer.Write(type);
            writer.Write(name);
            writer.Write((short)funcsraw.Length);

            for (int i = 0; i < funcsraw.Length; i++)
            {
                funcsraw[i].WriteToFile(ref writer);
            }
        }

        //
        // Parse
        //
        public void Parse(ref idParser parser)
        {
            funcs = new List<idScriptFuncBinary>();

            type = parser.NextToken;
            name = parser.NextToken;

            if (name == "{")
            {
                name = "";
                parser.UngetToken();
            }

            parser.ExpectNextToken("{");

            while (true)
            {
                string token = parser.NextToken;

                if (token == null || token.Length <= 0 || parser.ReachedEndOfBuffer == true)
                    throw new Exception("Unexpected EOF");

                if (token == "}")
                {
                    break;
                }
                else if (token == "{")
                {
                    throw new Exception("Illegal open bracket detected in " + " in " + type + "::" + name);
                }
                else
                {
                    for (int i = 0; i <= ScriptReaderShared.gScriptActions.Length; i++)
                    {
                        if (i == ScriptReaderShared.gScriptActions.Length)
                        {
                            throw new Exception("Invalid or unexpected token " + token + " in " + type + "::" + name);
                        }

                        if (ScriptReaderShared.gScriptActions[i].name == token)
                        {
                            idScriptFuncBinary func = new idScriptFuncBinary();

                            if (parser.LineHasMoreTokens)
                            {
                                while (true)
                                {
                                    token = parser.GetNextTokenFromLine();

                                    if (token == null || token.Length < 0)
                                        break;

                                    if (token == "}")
                                    {
                                      //  System.Diagnostics.Debugger.Launch();
                                        if (func.numparams > 0)
                                        {
                                            throw new Exception("Close bracket should be on its own line " + token + " in " + type + "::" + name + " at position " + parser.Position);
                                        }
                                        else
                                        {
                                            throw new Exception("Close bracket should be on its own line " + token + " in " + type + "::" + name + " at position " + parser.Position);
                                        }
                                    }

                                    func.AddParam(token);
                                }
                            }

                            funcs.Add(func);
                            break;
                        }
                    }
                }
            }

            funcsraw = funcs.ToArray();
            funcs.Clear();
            funcs = null;
        }
    }

    //
    // idScriptFuncBinary
    //
    public class idScriptFuncBinary
    {
        public short opCode;
        public byte numparams;
        public string[] parms;

        public void AddParam(string token)
        {
            if(parms == null)
                parms = new string[20];

            parms[numparams++] = token;
        }

        public void WriteToFile(ref BinaryWriter writer)
        {
            writer.Write(opCode);
            writer.Write(numparams);

            for (int i = 0; i < numparams; i++)
            {
                writer.Write(parms[i]);
            }
        }

        public void ReadFromFile(ref idFile file)
        {
            opCode = file.ReadShort();
            numparams = file.ReadByte();

            parms = new string[numparams];
            for (int i = 0; i < numparams; i++)
            {
                parms[i] = file.ReadString();
            }
        }
    }

    //
    // ScriptReaderShared
    //
    static class ScriptReaderShared
    {
        //
        // gameScriptOpcode
        //
        public enum gameScriptOpcode
        {
            gotomarker = 0,
	        playsound,
	        playanim,
	        wait,
	        trigger,
	        alertentity,
	        accum,
	        missionfailed,
	        missionsuccess,
	        print,
	        faceangles,
	        resetscript,
	        attachtotag,
	        halt,
	        stopsound,
	        startcam,
	        entityscriptname,
	        aiscriptname,
	        // DHM - Nerve :: multiplayer scripting commands start with wm_ (Wolf Multiplayer
	        wm_mapdescription,
	        wm_axis_respawntime,
	        wm_allied_respawntime,
	        wm_number_of_objectives,
	        wm_objective_axis_desc,
	        wm_objective_allied_desc,
	        wm_setwinner,
	        wm_set_objective_status,
	        wm_announce,
	        wm_endround,
	        wm_set_round_timelimit,
	        // dhm
	        backupscript,
	        restorescript,
	        sethealth,

	        mu_start, // (char *new_music, int time		// time to fadeup
	        mu_play,  // (char *music
	        mu_stop,  // (int time						// time to fadeout
	        mu_fade,  // (float target_volume, int time	// time to fade to target
	        mu_queue // (char *new_music				// music that will start when previous fades to 0

        }

        public struct idScriptActionStorage
        {
            public gameScriptOpcode opCode;     // This gets saved/read from the compiled script.
            public string name;       // Script name this only gets read from the text script.

            public idScriptActionStorage(gameScriptOpcode opCode, string name)
            {
                this.opCode = opCode;
                this.name = name;
            }
        }
        // these are the actions that each event can call
        public static idScriptActionStorage[] gScriptActions = new idScriptActionStorage[]
        {
	        new idScriptActionStorage(gameScriptOpcode.gotomarker, "gotomarker"),
	        new idScriptActionStorage(gameScriptOpcode.playsound,"playsound"),
	        new idScriptActionStorage(gameScriptOpcode.playanim,"playanim"),
	        new idScriptActionStorage(gameScriptOpcode.wait, "wait"),
	        new idScriptActionStorage(gameScriptOpcode.trigger, "trigger"),
	        new idScriptActionStorage(gameScriptOpcode.alertentity, "alertentity"),
	        new idScriptActionStorage(gameScriptOpcode.accum, "accum"),
	        new idScriptActionStorage(gameScriptOpcode.missionfailed, "missionfailed"),
	        new idScriptActionStorage(gameScriptOpcode.missionsuccess, "missionsuccess"),
	        new idScriptActionStorage(gameScriptOpcode.print, "print"),
	        new idScriptActionStorage(gameScriptOpcode.faceangles, "faceangles"),
	        new idScriptActionStorage(gameScriptOpcode.resetscript, "resetscript"),
	        new idScriptActionStorage(gameScriptOpcode.attachtotag, "attachtotag"),
	        new idScriptActionStorage(gameScriptOpcode.halt, "halt"),
	        new idScriptActionStorage(gameScriptOpcode.stopsound, "stopsound"),
	        new idScriptActionStorage(gameScriptOpcode.startcam, "startcam"),
	        new idScriptActionStorage(gameScriptOpcode.entityscriptname, "entityscriptname"),
	        new idScriptActionStorage(gameScriptOpcode.aiscriptname, "aiscriptname"),
	        // DHM - Nerve :: multiplayer scripting commands start with "wm_" (Wolf Multiplayer)
	        new idScriptActionStorage(gameScriptOpcode.wm_mapdescription, "wm_mapdescription"),
	        new idScriptActionStorage(gameScriptOpcode.wm_axis_respawntime , "wm_axis_respawntime"),
	        new idScriptActionStorage(gameScriptOpcode.wm_allied_respawntime ,"wm_allied_respawntime"),
	        new idScriptActionStorage(gameScriptOpcode.wm_number_of_objectives ,"wm_number_of_objectives"),
	        new idScriptActionStorage(gameScriptOpcode.wm_objective_axis_desc ,"wm_objective_axis_desc"),
	        new idScriptActionStorage(gameScriptOpcode.wm_objective_allied_desc ,"wm_objective_allied_desc"),
	        new idScriptActionStorage(gameScriptOpcode.wm_setwinner ,"wm_setwinner"),
	        new idScriptActionStorage(gameScriptOpcode.wm_set_objective_status ,"wm_set_objective_status"),
	        new idScriptActionStorage(gameScriptOpcode.wm_announce ,"wm_announce"),
	        new idScriptActionStorage(gameScriptOpcode.wm_endround ,"wm_endround"),
	        new idScriptActionStorage(gameScriptOpcode.wm_set_round_timelimit ,"wm_set_round_timelimit"),
	        // dhm
	        new idScriptActionStorage(gameScriptOpcode.backupscript ,"backupscript"),
	        new idScriptActionStorage(gameScriptOpcode.restorescript ,"restorescript"),
	        new idScriptActionStorage(gameScriptOpcode.sethealth ,"sethealth"),

	        new idScriptActionStorage(gameScriptOpcode.mu_start ,"mu_start"), // (char *new_music, int time)		// time to fadeup
	        new idScriptActionStorage(gameScriptOpcode.mu_play ,"mu_play"),  // (char *music)
	        new idScriptActionStorage(gameScriptOpcode.mu_stop ,"mu_stop"),  // (int time)						// time to fadeout
	        new idScriptActionStorage(gameScriptOpcode.mu_fade ,"mu_fade"),  // (float target_volume, int time)	// time to fade to target
	        new idScriptActionStorage(gameScriptOpcode.mu_queue ,"mu_queue") // (char *new_music)				// music that will start when previous fades to 0
        };

    }

    public class idGameScriptParser
    {
        List<idScriptEvent> events = new List<idScriptEvent>();
        //
        // idGameScriptParser
        //
        public idGameScriptParser(string path, ref idParser parser)
        {
            while (parser.ReachedEndOfBuffer == false)
            {
                idScriptEvent ev = new idScriptEvent();
                if (ev.Parse(ref parser) == false)
                    break;

                events.Add(ev);
            }
        }

        public void WriteToFile(ref BinaryWriter writer)
        {
            for (int i = 0; i < events.Count; i++)
            {
                events[i].WriteToFile(ref writer);
            }
        }
    }
}
