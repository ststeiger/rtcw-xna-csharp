// script.cs (c) 2010 JV Software
//

using System;

using idLib;
using idLib.Game;
using idLib.Engine.Public;

namespace Game
{
    //
    // idScript
    //
    public class idScript
    {
        internal idBinaryScript script;

        //
        // idScriptFunctionDesc
        //
        private struct idScriptFunctionDesc
        {
            public delegate bool idScriptFunc_t(idEntity ent, idScriptFuncBinary func);

            public ScriptReaderShared.gameScriptOpcode opCode;
            public idScriptFunc_t function;

            //
            // idScriptFunctionDesc
            //
            public idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode opCode, idScriptFunc_t function)
            {
                this.opCode = opCode;
                this.function = function;
            }
        }

        //
        // idScript
        //
        public idScript(string scriptpath)
        {
            script = Engine.fileSystem.ReadContent<idBinaryScript>(scriptpath);
        }

        private static idScriptFunctionDesc[] scriptFuncTable = new idScriptFunctionDesc[]
        {
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.gotomarker,               G_ScriptAction_GotoMarker),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.playsound,                G_ScriptAction_PlaySound),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.playanim,             G_ScriptAction_PlayAnim),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wait,                 G_ScriptAction_Wait),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.trigger,                  G_ScriptAction_Trigger),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.alertentity,              G_ScriptAction_AlertEntity),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.accum,                    G_ScriptAction_Accum),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.missionfailed,            G_ScriptAction_MissionFailed),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.missionsuccess,           G_ScriptAction_MissionSuccess),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.print,                    G_ScriptAction_Print),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.faceangles,               G_ScriptAction_FaceAngles),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.resetscript,              G_ScriptAction_ResetScript),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.attachtotag,              G_ScriptAction_TagConnect),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.halt,                 G_ScriptAction_Halt),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.stopsound,                G_ScriptAction_StopSound),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.startcam,             G_ScriptAction_StartCam),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.entityscriptname,     G_ScriptAction_EntityScriptName),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.aiscriptname,         G_ScriptAction_AIScriptName),
	        // DHM - Nerve :: multiplayer scripting commands start with wm_ (Wolf Multiplayer)
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_mapdescription,        G_ScriptAction_MapDescription),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_axis_respawntime,      G_ScriptAction_AxisRespawntime),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_allied_respawntime,    G_ScriptAction_AlliedRespawntime),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_number_of_objectives,  G_ScriptAction_NumberofObjectives),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_objective_axis_desc,   G_ScriptAction_ObjectiveAxisDesc),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_objective_allied_desc,G_ScriptAction_ObjectiveAlliedDesc),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_setwinner,         G_ScriptAction_SetWinner),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_set_objective_status,  G_ScriptAction_SetObjectiveStatus),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_announce,              G_ScriptAction_Announce),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_endround,              G_ScriptAction_EndRound),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.wm_set_round_timelimit,   G_ScriptAction_SetRoundTimelimit),
	        // dhm
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.backupscript,         G_ScriptAction_BackupScript),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.restorescript,            G_ScriptAction_RestoreScript),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.sethealth,                G_ScriptAction_SetHealth),

	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.mu_start,             G_ScriptAction_MusicStart), // (char *new_music, int time)		// time to fadeup
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.mu_play,                  G_ScriptAction_MusicPlay),  // (char *music)
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.mu_stop,                  G_ScriptAction_MusicStop),  // (int time)						// time to fadeout
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.mu_fade,                  G_ScriptAction_MusicFade),  // (float target_volume, int time)	// time to fade to target
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.mu_queue,             G_ScriptAction_MusicQueue), // (char *new_music)				// music that will start when previous fades to 0

            new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_gotomarker,       AICast_ScriptAction_GotoMarker),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_runtomarker,      AICast_ScriptAction_GotoMarker),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_walktomarker, AICast_ScriptAction_WalkToMarker),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_crouchtomarker,   AICast_ScriptAction_CrouchToMarker),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_gotocast,     AICast_ScriptAction_GotoCast),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_runtocast,        AICast_ScriptAction_GotoCast),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_walktocast,       AICast_ScriptAction_WalkToCast),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_crouchtocast, AICast_ScriptAction_CrouchToCast),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_followcast,       AICast_ScriptAction_FollowCast),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_playsound,        AICast_ScriptAction_PlaySound),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_playanim,     AICast_ScriptAction_PlayAnim),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_clearanim,        AICast_ScriptAction_ClearAnim),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_wait,         AICast_ScriptAction_Wait),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_abort_if_loadgame,AICast_ScriptAction_AbortIfLoadgame),   //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_trigger,          AICast_ScriptAction_Trigger),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_setammo,          AICast_ScriptAction_SetAmmo),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_setclip,          AICast_ScriptAction_SetClip),           //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_selectweapon, AICast_ScriptAction_SelectWeapon),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_noattack,     AICast_ScriptAction_NoAttack),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_suggestweapon,    AICast_ScriptAction_SuggestWeapon),     //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_attack,           AICast_ScriptAction_Attack),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_givearmor,        AICast_ScriptAction_GiveArmor),         //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_setarmor,     AICast_ScriptAction_SetArmor),          //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_giveinventory,    AICast_ScriptAction_GiveInventory),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_giveweapon,       AICast_ScriptAction_GiveWeapon),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_takeweapon,       AICast_ScriptAction_TakeWeapon),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_movetype,     AICast_ScriptAction_Movetype),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_alertentity,      AICast_ScriptAction_AlertEntity),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_savegame,     AICast_ScriptAction_SaveGame),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_fireattarget, AICast_ScriptAction_FireAtTarget),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_godmode,          AICast_ScriptAction_GodMode),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_accum,            AICast_ScriptAction_Accum),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_spawncast,        AICast_ScriptAction_SpawnCast),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_missionfailed,    AICast_ScriptAction_MissionFailed),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_missionsuccess,   AICast_ScriptAction_ObjectiveMet),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_objectivemet, AICast_ScriptAction_ObjectiveMet),  // dupe of missionsuccess so scripts can changeover to a more logical name
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_objectivesneeded,AICast_ScriptAction_ObjectivesNeeded),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_noaidamage,       AICast_ScriptAction_NoAIDamage),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_print,            AICast_ScriptAction_Print),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_facetargetangles,AICast_ScriptAction_FaceTargetAngles),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_resetscript,      AICast_ScriptAction_ResetScript),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_mount,            AICast_ScriptAction_Mount),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_unmount,          AICast_ScriptAction_Unmount),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_savepersistant,   AICast_ScriptAction_SavePersistant),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_changelevel,      AICast_ScriptAction_ChangeLevel),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_endgame,          AICast_ScriptAction_EndGame),   //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_teleport,     AICast_ScriptAction_Teleport),  //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_foundsecret,      AICast_ScriptAction_FoundSecret),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_nosight,          AICast_ScriptAction_NoSight),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_sight,            AICast_ScriptAction_Sight),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_noavoid,          AICast_ScriptAction_NoAvoid),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_avoid,            AICast_ScriptAction_Avoid),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_attrib,           AICast_ScriptAction_Attrib),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_denyactivate, AICast_ScriptAction_DenyAction),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_lightningdamage,  AICast_ScriptAction_LightningDamage),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_deny,         AICast_ScriptAction_DenyAction),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_headlook,     AICast_ScriptAction_Headlook),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_backupscript, AICast_ScriptAction_BackupScript),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_restorescript,    AICast_ScriptAction_RestoreScript),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_statetype,        AICast_ScriptAction_StateType),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_knockback,        AICast_ScriptAction_KnockBack),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_zoom,         AICast_ScriptAction_Zoom),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_parachute,        AICast_ScriptAction_Parachute),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_cigarette,        AICast_ScriptAction_Cigarette), //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_startcam,     AICast_ScriptAction_StartCam),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_startcamblack,    AICast_ScriptAction_StartCamBlack),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_stopcam,          AICast_ScriptAction_StopCam),   //----(SA)	added
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_entityscriptname,AICast_ScriptAction_EntityScriptName),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_aiscriptname, AICast_ScriptAction_AIScriptName),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_sethealth,        AICast_ScriptAction_SetHealth),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_notarget,     AICast_ScriptAction_NoTarget),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_cvar,         AICast_ScriptAction_Cvar),

        //----(SA)	added some music interface
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_mu_start,     AICast_ScriptAction_MusicStart),    // (char *new_music, int time)	// time to fade in
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_mu_play,          AICast_ScriptAction_MusicPlay),     // (char *new_music)
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_mu_stop,          AICast_ScriptAction_MusicStop),     // (int time)	// time to fadeout
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_mu_fade,          AICast_ScriptAction_MusicFade),     // (float target_volume, int time)	// time to fade to target
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_mu_queue,     AICast_ScriptAction_MusicQueue),    // (char *new_music)	// music that will start when previous fades to 0
        //----(SA)	end

	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_explicit_routing, AICast_ScriptAction_ExplicitRouting),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_lockplayer,       AICast_ScriptAction_LockPlayer),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_anim_condition,   AICast_ScriptAction_AnimCondition),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_pushaway,     AICast_ScriptAction_PushAway),
	        new idScriptFunctionDesc(ScriptReaderShared.gameScriptOpcode.ai_catchfire,        AICast_ScriptAction_CatchFire)
        };

        //
        // Script_GetTime
        //
        public static int Script_GetTime()
        {
            return Engine.Sys.Sys_Milliseconds();
        }

        //
        // ScriptThreadWorker
        //
        private void ScriptThreadWorker(idEntity entity, idScriptAction _action)
        {
            // Run every function in the action.
            for (int i = 0; i < _action.funcsraw.Length; i++)
            {
                scriptFuncTable[_action.funcsraw[i].opCode].function(entity, _action.funcsraw[i]);
            }
        }

        //
        // RunAction
        //
        public void Execute(idEntity entity, idScriptAction _action)
        {
            if (_action == null)
            {
                Engine.common.ErrorFatal("Script_Execute action is null\n");
            }

            idThread thread = Engine.Sys.CreateThread(_action.name, () => ScriptThreadWorker(entity, _action));
            thread.Start(null);
        }

        //
        // FindAction
        //
        public idScriptAction FindAction(string _namespace, string _eventname, bool showWarning)
        {
            // Find the namespace in the script.
            foreach (idScriptEvent _event in script.events)
            {
                if (_event.name == _namespace)
                {
                    // Now try to find the requested action.
                    foreach (idScriptAction _action in _event.actionsraw)
                    {
                        // If we found the action name return it.
                        if (_action.type == _eventname)
                        {
                            return _action;
                        }
                        if (_action.name == _eventname)
                        {
                            return _action;
                        }
                    }
                    if (showWarning)
                    {
                        Engine.common.Warning("Script_FindAction could not find action " + _namespace + "::" + _eventname + " \n");
                    }
                    return null;
                }
            }

            if (showWarning)
            {
                Engine.common.Warning("Script_FindAction could not find namespace %s \n", _namespace);
            }
            return null;
        }

        //
        // GAME SCRIPT FUNCTIONS
        //

        private static bool G_ScriptAction_Trigger(idEntity ent, idScriptFuncBinary func) {
            idScriptAction action;

            action = Level.script.FindAction(func.parms[0], func.parms[1], false);
            if (action == null)
            {
                action = Level.aiscript.FindAction(func.parms[0], func.parms[1], true);
                if (action == null)
                {
                    return false;
                }
                else
                {
                    Level.aiscript.Execute(ent, action);
                    return true;
                }
            }

            Level.script.Execute(ent, action);
            return true;
        }

        //----(SA)	added
        private static bool G_ScriptAction_MusicStart(idEntity ent, idScriptFuncBinary func) {
            Level.net.PlayBackgroundTrackForPlayer(ent, func.parms[0]);
            return true; 
        }
        private static bool G_ScriptAction_MusicPlay(idEntity ent, idScriptFuncBinary func) {
            Level.net.PlayBackgroundTrackForPlayer(ent, func.parms[0]);
            return true; 
        }
        private static bool G_ScriptAction_MusicStop(idEntity ent, idScriptFuncBinary func) {
            Level.net.StopBackgroundTrackForPlayer(ent);
            return true;
        }
        private static bool G_ScriptAction_MusicFade(idEntity ent, idScriptFuncBinary func) {
            Level.net.StopBackgroundTrackForPlayer(ent);
            return true;
        }
        private static bool G_ScriptAction_MusicQueue(idEntity ent, idScriptFuncBinary func) {
            Level.net.StopBackgroundTrackForPlayer(ent);
            Level.net.PlayBackgroundTrackForPlayer(ent, func.parms[0]);
            return true; 
        }

        private static bool G_ScriptAction_Wait(idEntity ent, idScriptFuncBinary func) {
            System.Threading.Thread.Sleep(int.Parse(func.parms[0]));
            return true;
        }

        //
        // AI SCRIPT FUNCTIONS
        //

        private static bool AICast_ScriptAction_StartCam(idEntity ent, idScriptFuncBinary func) {
            idCameraManager.loadCamera(0, "cameras/" + func.parms[0] + ".camera");
            Level.cameranum = 0;
            return true;
        }

        private static bool AICast_ScriptAction_StopCam(idEntity ent, idScriptFuncBinary func) {
            Level.cameranum = -1;
            return true;
        }  //----(SA)	added

        private static bool AICast_ScriptAction_StartCamBlack(idEntity ent, idScriptFuncBinary func) {
            idCameraManager.loadCamera(0, "cameras/" + func.parms[0] + ".camera");
            Level.cameranum = 0;
            return true;
        }

        private static bool AICast_ScriptAction_MusicStart(idEntity ent, idScriptFuncBinary func)
        {
            Level.net.PlayBackgroundTrackForPlayer(ent, func.parms[0]);
            return true;
        }
        private static bool AICast_ScriptAction_MusicPlay(idEntity ent, idScriptFuncBinary func)
        {
            Level.net.PlayBackgroundTrackForPlayer(ent, func.parms[0]);
            return true;
        }
        private static bool AICast_ScriptAction_MusicStop(idEntity ent, idScriptFuncBinary func)
        {
            Level.net.StopBackgroundTrackForPlayer(ent);
            return true;
        }
        private static bool AICast_ScriptAction_MusicFade(idEntity ent, idScriptFuncBinary func)
        {
            Level.net.StopBackgroundTrackForPlayer(ent);
            return true;
        }
        private static bool AICast_ScriptAction_MusicQueue(idEntity ent, idScriptFuncBinary func)
        {
            Level.net.StopBackgroundTrackForPlayer(ent);
            Level.net.PlayBackgroundTrackForPlayer(ent, func.parms[0]);
            return true;
        }

        private static bool AICast_ScriptAction_Wait(idEntity ent, idScriptFuncBinary func)
        {
            System.Threading.Thread.Sleep(int.Parse(func.parms[0]));
            return true;
        }

        private static bool AICast_ScriptAction_Trigger(idEntity ent, idScriptFuncBinary func)
        {
            idScriptAction action;

            action = Level.script.FindAction(func.parms[0], func.parms[1], false);
            if (action == null)
            {
                action = Level.aiscript.FindAction(func.parms[0], func.parms[1], true);
                if (action == null)
                {
                    return false;
                }
                else
                {
                    Level.aiscript.Execute(ent, action);
                    return true;
                }
            }

            Level.script.Execute(ent, action);
            return true;
        }
        //----(SA)	end

        #region ScriptStubs_ImplementMe
        private static bool G_ScriptAction_GotoMarker(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_PlaySound(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_PlayAnim(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_AlertEntity(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_Accum(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_MissionFailed(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_MissionSuccess(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_Print(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_FaceAngles(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_ResetScript(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_TagConnect(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_Halt(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_StopSound(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_StartCam(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_EntityScriptName(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_AIScriptName(idEntity ent, idScriptFuncBinary func) { return false; }
        // DHM - Nerve :: Multiplayer scripting commands
        private static bool G_ScriptAction_MapDescription(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_AxisRespawntime(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_AlliedRespawntime(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_NumberofObjectives(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_ObjectiveAxisDesc(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_ObjectiveAlliedDesc(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_SetWinner(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_SetObjectiveStatus(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_Announce(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_EndRound(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_SetRoundTimelimit(idEntity ent, idScriptFuncBinary func) { return false; }
        // dhm
        private static bool G_ScriptAction_BackupScript(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_RestoreScript(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool G_ScriptAction_SetHealth(idEntity ent, idScriptFuncBinary func) { return false; }

        // AI scripting functions
        private static bool AICast_ScriptAction_GotoMarker(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_WalkToMarker(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_CrouchToMarker(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_GotoCast(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_WalkToCast(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_CrouchToCast(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_AbortIfLoadgame(idEntity ent, idScriptFuncBinary func) { return false; } //----(SA)	added
        private static bool AICast_ScriptAction_FollowCast(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_PlaySound(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_NoAttack(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Attack(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_PlayAnim(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_ClearAnim(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_SetAmmo(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_SetClip(idEntity ent, idScriptFuncBinary func) { return false; }         //----(SA)	added
        private static bool AICast_ScriptAction_SelectWeapon(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_GiveArmor(idEntity ent, idScriptFuncBinary func) { return false; }       //----(SA)	added
        private static bool AICast_ScriptAction_SetArmor(idEntity ent, idScriptFuncBinary func) { return false; }        //----(SA)	added
        private static bool AICast_ScriptAction_SuggestWeapon(idEntity ent, idScriptFuncBinary func) { return false; }   //----(SA)	added
        private static bool AICast_ScriptAction_GiveWeapon(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_GiveInventory(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_TakeWeapon(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Movetype(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_AlertEntity(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_SaveGame(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_FireAtTarget(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_GodMode(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Accum(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_SpawnCast(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_MissionFailed(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_ObjectiveMet(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_ObjectivesNeeded(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_NoAIDamage(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Print(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_FaceTargetAngles(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_ResetScript(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Mount(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Unmount(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_SavePersistant(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_ChangeLevel(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_EndGame(idEntity ent, idScriptFuncBinary func) { return false; } //----(SA)	added
        private static bool AICast_ScriptAction_Teleport(idEntity ent, idScriptFuncBinary func) { return false; }    //----(SA)	added
        private static bool AICast_ScriptAction_FoundSecret(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_NoSight(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Sight(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_NoAvoid(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Avoid(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Attrib(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_DenyAction(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_LightningDamage(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Headlook(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_BackupScript(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_RestoreScript(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_StateType(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_KnockBack(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Zoom(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Parachute(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Cigarette(idEntity ent, idScriptFuncBinary func) { return false; }    //----(SA)	added
        private static bool AICast_ScriptAction_EntityScriptName(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_AIScriptName(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_SetHealth(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_NoTarget(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_Cvar(idEntity ent, idScriptFuncBinary func) { return false; }

        private static bool AICast_ScriptAction_ExplicitRouting(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_LockPlayer(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_AnimCondition(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_PushAway(idEntity ent, idScriptFuncBinary func) { return false; }
        private static bool AICast_ScriptAction_CatchFire(idEntity ent, idScriptFuncBinary func) { return false; }
        #endregion
    }
}
