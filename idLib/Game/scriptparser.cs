// gamescript_parser.cs (c) 2010 JV Software
//

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

using idLib;
using idLib.Engine.Public;

namespace idLib.Game
{
    //
    // idBinaryScript
    //
    public class idBinaryScript
    {
        public idScriptEvent[] events;

        public idBinaryScript(idFile input)
        {
            int numEvents = input.ReadShort();

            events = new idScriptEvent[numEvents];

            for (int i = 0; i < numEvents; i++)
            {
                events[i] = new idScriptEvent();
                events[i].ReadFromFile(ref input);
            }
        }
    }

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
                actionsraw[i] = new idScriptAction();
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

        public bool Parse(ref idParser parser, bool isAIScript)
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
                action.Parse(ref parser, isAIScript);
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
        public idDict attributes = new idDict();

        //
        // ReadFromFile
        //
        public void ReadFromFile(ref idFile file)
        {
            int numAttributes = 0;

            type = file.ReadString();
            name = file.ReadString();

            funcsraw = new idScriptFuncBinary[file.ReadShort()];

            numAttributes = file.ReadShort();
            for (int i = 0; i < numAttributes; i++)
            {
                string key = file.ReadString();
                string val = file.ReadString();

                attributes.AddKey(key, val);
            }

            for (int i = 0; i < funcsraw.Length; i++)
            {
                funcsraw[i] = new idScriptFuncBinary();
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

            writer.Write((short)attributes.NumKeys);
            for (int i = 0; i < attributes.NumKeys; i++)
            {
                if (attributes[i].name == null)
                {
                    throw new Exception("Attribute " + i + " has a null name" );
                }
                if (attributes[i].val == null)
                {
                    throw new Exception("Attribute " + attributes[i].name + " val is null");
                }
                writer.Write(attributes[i].name);
                writer.Write(attributes[i].val);
            }

            for (int i = 0; i < funcsraw.Length; i++)
            {
                funcsraw[i].WriteToFile(ref writer);
            }
        }

        //
        // Parse
        //
        public void Parse(ref idParser parser, bool isAIScript)
        {
            string token;
            int startFunc = 0;
            bool isAttributes = false;

            funcs = new List<idScriptFuncBinary>();
            

            type = parser.NextToken;
            if (type == "attributes")
            {
                isAttributes = true;
            }

            name = parser.NextToken;

            if (name == "{")
            {
                name = "";
                parser.UngetToken();
            }

            // This is just a hack for now until I figure out how to do this properly,
            // some actions have a additional param so I'm just piggy backing this on to name for now,
            token = parser.NextToken;
            if (token != "{")
            {
                name += " " + token;

                parser.ExpectNextToken("{");
            }

            if (isAIScript)
                startFunc = (int)ScriptReaderShared.gameScriptOpcode.ai_gotomarker;

            while (true)
            {
                token = parser.NextToken;

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
                else if (isAttributes)
                {
                    string key = parser.GetNextTokenFromLine();

                    if(key == null)
                    {
                        key = " ";
                    }

                    attributes.AddKey(token, key);
                }
                else
                {
                    for (int i = startFunc; i <= ScriptReaderShared.gScriptActions.Length; i++)
                    {
                        if (i == ScriptReaderShared.gScriptActions.Length)
                        {
                            throw new Exception("Invalid or unexpected token " + token + " in " + type + "::" + name);
                        }

                        if (ScriptReaderShared.gScriptActions[i].name == token)
                        {
                            idScriptFuncBinary func = new idScriptFuncBinary();

                            func.opCode = (short)ScriptReaderShared.gScriptActions[i].opCode;

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

            // Precache the sounds
            if (opCode == (short)ScriptReaderShared.gameScriptOpcode.ai_playsound)
            {
                idLib.Engine.Public.Engine.soundManager.LoadSound(parms[0]);
            }
        }
    }

    //
    // ScriptReaderShared
    //
    public static class ScriptReaderShared
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
	        mu_queue, // (char *new_music				// music that will start when previous fades to 0

            // AI
            ai_gotomarker,
	        ai_runtomarker,
	        ai_walktomarker,
	        ai_crouchtomarker,
	        ai_gotocast,
	        ai_runtocast,
	        ai_walktocast,
	        ai_crouchtocast,
	        ai_followcast,
	        ai_playsound,
	        ai_playanim,
	        ai_clearanim,
	        ai_wait,
	        ai_abort_if_loadgame,
	        ai_trigger,
	        ai_setammo,
	        ai_setclip,
	        ai_selectweapon,
	        ai_noattack,
	        ai_suggestweapon, 
	        ai_attack,
	        ai_givearmor,
	        ai_setarmor,
	        ai_giveinventory,
	        ai_giveweapon,
	        ai_takeweapon,
	        ai_movetype,
	        ai_alertentity,
	        ai_savegame,
	        ai_fireattarget,
	        ai_godmode,
	        ai_accum,
	        ai_spawncast,    
	        ai_missionfailed, 
	        ai_missionsuccess, 
	        ai_objectivemet, 
	        ai_objectivesneeded,
	        ai_noaidamage,     
	        ai_print,  
	        ai_facetargetangles,
	        ai_resetscript,  
	        ai_mount,        
	        ai_unmount,       
	        ai_savepersistant, 
	        ai_changelevel,   
	        ai_endgame,   
	        ai_teleport, 
	        ai_foundsecret, 
	        ai_nosight,     
	        ai_sight,  
	        ai_noavoid, 
	        ai_avoid, 
	        ai_attrib, 
	        ai_denyactivate,
	        ai_lightningdamage,  
	        ai_deny,
	        ai_headlook,
	        ai_backupscript, 
	        ai_restorescript, 
	        ai_statetype,
	        ai_knockback,  
	        ai_zoom,    
	        ai_parachute,  
	        ai_cigarette, 
	        ai_startcam,  
	        ai_startcamblack, 
	        ai_stopcam,  
	        ai_entityscriptname,
	        ai_aiscriptname,
	        ai_sethealth, 
	        ai_notarget,  
	        ai_cvar, 

        //----(SA)	added some music interface
	        ai_mu_start,
	        ai_mu_play,
	        ai_mu_stop,
	        ai_mu_fade,
	        ai_mu_queue, 
        //----(SA)	end

	        ai_explicit_routing,
	        ai_lockplayer, 
	        ai_anim_condition, 
	        ai_pushaway, 
	        ai_catchfire,

            max_script_opcodes
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
	        new idScriptActionStorage(gameScriptOpcode.mu_queue ,"mu_queue"), // (char *new_music)				// music that will start when previous fades to 0

            // AI Functions.
            new idScriptActionStorage(gameScriptOpcode.ai_gotomarker, "gotomarker"), //       AICast_ScriptAction_GotoMarker}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_runtomarker, "runtomarker"), //      AICast_ScriptAction_GotoMarker}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_walktomarker, "walktomarker"), // AICast_ScriptAction_WalkToMarker}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_crouchtomarker, "crouchtomarker"), //   AICast_ScriptAction_CrouchToMarker}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_gotocast, "gotocast"), //     AICast_ScriptAction_GotoCast}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_runtocast, "runtocast"), //        AICast_ScriptAction_GotoCast}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_walktocast, "walktocast"), //       AICast_ScriptAction_WalkToCast}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_crouchtocast, "crouchtocast"), // AICast_ScriptAction_CrouchToCast}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_followcast, "followcast"), //       AICast_ScriptAction_FollowCast}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_playsound, "playsound"), //        AICast_ScriptAction_PlaySound}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_playanim, "playanim"), //     AICast_ScriptAction_PlayAnim}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_clearanim, "clearanim"), //        AICast_ScriptAction_ClearAnim}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_wait, "wait"), //         AICast_ScriptAction_Wait}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_abort_if_loadgame, "abort_if_loadgame"), //AICast_ScriptAction_AbortIfLoadgame}), //   //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_trigger, "trigger"), //          AICast_ScriptAction_Trigger}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_setammo, "setammo"), //          AICast_ScriptAction_SetAmmo}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_setclip, "setclip"), //          AICast_ScriptAction_SetClip}), //           //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_selectweapon, "selectweapon"), // AICast_ScriptAction_SelectWeapon}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_noattack, "noattack"), //     AICast_ScriptAction_NoAttack}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_suggestweapon, "suggestweapon"), //    AICast_ScriptAction_SuggestWeapon}), //     //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_attack, "attack"), //           AICast_ScriptAction_Attack}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_givearmor, "givearmor"), //        AICast_ScriptAction_GiveArmor}), //         //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_setarmor, "setarmor"), //     AICast_ScriptAction_SetArmor}), //          //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_giveinventory, "giveinventory"), //    AICast_ScriptAction_GiveInventory}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_giveweapon, "giveweapon"), //       AICast_ScriptAction_GiveWeapon}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_takeweapon, "takeweapon"), //       AICast_ScriptAction_TakeWeapon}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_movetype, "movetype"), //     AICast_ScriptAction_Movetype}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_alertentity, "alertentity"), //      AICast_ScriptAction_AlertEntity}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_savegame, "savegame"), //     AICast_ScriptAction_SaveGame}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_fireattarget, "fireattarget"), // AICast_ScriptAction_FireAtTarget}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_godmode, "godmode"), //          AICast_ScriptAction_GodMode}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_accum, "accum"), //            AICast_ScriptAction_Accum}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_spawncast, "spawncast"), //        AICast_ScriptAction_SpawnCast}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_missionfailed, "missionfailed"), //    AICast_ScriptAction_MissionFailed}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_missionsuccess, "missionsuccess"), //   AICast_ScriptAction_ObjectiveMet}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_objectivemet, "objectivemet"), // AICast_ScriptAction_ObjectiveMet}), //  // dupe of missionsuccess so scripts can changeover to a more logical name
	        new idScriptActionStorage(gameScriptOpcode.ai_objectivesneeded, "objectivesneeded"), //AICast_ScriptAction_ObjectivesNeeded}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_noaidamage, "noaidamage"), //       AICast_ScriptAction_NoAIDamage}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_print, "print"), //            AICast_ScriptAction_Print}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_facetargetangles, "facetargetangles"), //AICast_ScriptAction_FaceTargetAngles}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_resetscript, "resetscript"), //      AICast_ScriptAction_ResetScript}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_mount, "mount"), //            AICast_ScriptAction_Mount}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_unmount, "unmount"), //          AICast_ScriptAction_Unmount}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_savepersistant, "savepersistant"), //   AICast_ScriptAction_SavePersistant}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_changelevel, "changelevel"), //      AICast_ScriptAction_ChangeLevel}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_endgame, "endgame"), //          AICast_ScriptAction_EndGame}), //   //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_teleport, "teleport"), //     AICast_ScriptAction_Teleport}), //  //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_foundsecret, "foundsecret"), //      AICast_ScriptAction_FoundSecret}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_nosight, "nosight"), //          AICast_ScriptAction_NoSight}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_sight, "sight"), //            AICast_ScriptAction_Sight}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_noavoid, "noavoid"), //          AICast_ScriptAction_NoAvoid}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_avoid, "avoid"), //            AICast_ScriptAction_Avoid}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_attrib, "attrib"), //           AICast_ScriptAction_Attrib}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_denyactivate, "denyactivate"), // AICast_ScriptAction_DenyAction}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_lightningdamage, "lightningdamage"), //  AICast_ScriptAction_LightningDamage}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_deny, "deny"), //         AICast_ScriptAction_DenyAction}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_headlook, "headlook"), //     AICast_ScriptAction_Headlook}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_backupscript, "backupscript"), // AICast_ScriptAction_BackupScript}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_restorescript, "restorescript"), //    AICast_ScriptAction_RestoreScript}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_statetype, "statetype"), //        AICast_ScriptAction_StateType}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_knockback, "knockback"), //        AICast_ScriptAction_KnockBack}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_zoom, "zoom"), //         AICast_ScriptAction_Zoom}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_parachute, "parachute"), //        AICast_ScriptAction_Parachute}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_cigarette, "cigarette"), //        AICast_ScriptAction_Cigarette}), // //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_startcam, "startcam"), //     AICast_ScriptAction_StartCam}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_startcamblack, "startcamblack"), //    AICast_ScriptAction_StartCamBlack}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_stopcam, "stopcam"), //          AICast_ScriptAction_StopCam}), //   //----(SA)	added
	        new idScriptActionStorage(gameScriptOpcode.ai_entityscriptname, "entityscriptname"), //AICast_ScriptAction_EntityScriptName}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_aiscriptname, "aiscriptname"), // AICast_ScriptAction_AIScriptName}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_sethealth, "sethealth"), //        AICast_ScriptAction_SetHealth}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_notarget, "notarget"), //     AICast_ScriptAction_NoTarget}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_cvar, "cvar"), //         AICast_ScriptAction_Cvar}), //

        //----(SA)	added some music interface
	        new idScriptActionStorage(gameScriptOpcode.ai_mu_start, "mu_start"), //     AICast_ScriptAction_MusicStart}), //    // (char *new_music), // int time)	// time to fade in
	        new idScriptActionStorage(gameScriptOpcode.ai_mu_play, "mu_play"), //          AICast_ScriptAction_MusicPlay}), //     // (char *new_music)
	        new idScriptActionStorage(gameScriptOpcode.ai_mu_stop, "mu_stop"), //          AICast_ScriptAction_MusicStop}), //     // (int time)	// time to fadeout
	        new idScriptActionStorage(gameScriptOpcode.ai_mu_fade, "mu_fade"), //          AICast_ScriptAction_MusicFade}), //     // (float target_volume), // int time)	// time to fade to target
	        new idScriptActionStorage(gameScriptOpcode.ai_mu_queue, "mu_queue"), //     AICast_ScriptAction_MusicQueue}), //    // (char *new_music)	// music that will start when previous fades to 0
        //----(SA)	end

	        new idScriptActionStorage(gameScriptOpcode.ai_explicit_routing, "explicit_routing"), // AICast_ScriptAction_ExplicitRouting}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_lockplayer, "lockplayer"), //       AICast_ScriptAction_LockPlayer}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_anim_condition, "anim_condition"), //   AICast_ScriptAction_AnimCondition}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_pushaway, "pushaway"), //     AICast_ScriptAction_PushAway}), //
	        new idScriptActionStorage(gameScriptOpcode.ai_catchfire, "catchfire"), //        AICast_ScriptAction_CatchFire}), //
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
                if (ev.Parse(ref parser, path.Contains(".ai")) == false)
                    break;

                events.Add(ev);
            }
        }

        public void WriteToFile(ref BinaryWriter writer)
        {
            writer.Write((short)events.Count);
            for (int i = 0; i < events.Count; i++)
            {
                events[i].WriteToFile(ref writer);
            }
        }
    }
}
