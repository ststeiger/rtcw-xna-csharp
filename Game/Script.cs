// script.cs (c) 2010 JV Software
//

using System;

using idLib;
using idLib.Game;
using idLib.Engine.Public;

using Game.AI;

namespace Game
{
    //
    // idScriptPendingAction
    //
    struct idScriptPendingAction
    {
        public int nextActionRunTime;
        public int pendingFunctionNum;
        public idScriptAction pendingaction;
        public idEntity actionEntity;
    }

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
            public delegate bool idScriptFunc_t(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent);

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
            idFile input;
            
            input = Engine.fileSystem.OpenFileRead(scriptpath + ".xnb", true);
            if (input == null)
            {
                Engine.common.ErrorFatal("Failed to open level script.\n");
            }

            script = new idBinaryScript(input);

            Engine.fileSystem.CloseFile(ref input);
            
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

        private idScriptPendingAction[] actionqueue = new idScriptPendingAction[2];

        //
        // Script_GetTime
        //
        public static int Script_GetTime()
        {
            return Engine.Sys.Sys_Milliseconds();
        }

        //
        // RunAction
        //
        public bool Execute(idEntity entity, idScriptAction _action)
        {
            if (_action == null)
            {
                Engine.common.ErrorFatal("Script_Execute action is null\n");
            }

            if (actionqueue[0].pendingaction != null)
            {
                Engine.common.Warning("Too many actions running in script, execute failed.\n");
                return false;
            }

            actionqueue[0].pendingaction = _action;
            actionqueue[0].actionEntity = entity;
            actionqueue[0].pendingFunctionNum = 0;

            return true;
        }

        //
        // RunActionFromQueue
        //
        private void RunActionFromQueue(ref idScriptPendingAction action)
        {
            if (action.nextActionRunTime > Level.time || action.pendingaction == null)
            {
                return;
            }

            // Run until we run out of functions to execute or hit a blocking function.
            while (true)
            {
                if (action.pendingFunctionNum < action.pendingaction.funcsraw.Length)
                {
                    // Function will return false if its blocking.
                    if (scriptFuncTable[action.pendingaction.funcsraw[action.pendingFunctionNum].opCode].function(ref action, action.pendingaction.funcsraw[action.pendingFunctionNum], this))
                    {
                        action.pendingFunctionNum++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    action.pendingaction = null;
                    break;
                }
            }
        }

        //
        // Think
        //
        public void Think()
        {
            for (int i = 0; i < actionqueue.Length; i++)
            {
                RunActionFromQueue(ref actionqueue[i]);
            }
        }

        //
        // RunAction
        //
        private bool ExecuteTrigger(idEntity entity, idScriptAction _action)
        {
            if (actionqueue[1].pendingaction != null)
            {
                return false;
            }

            actionqueue[1].pendingaction = _action;
            actionqueue[1].actionEntity = entity;
            actionqueue[1].pendingFunctionNum = 0;

            return true;
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

        private static bool G_ScriptAction_Trigger(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            idScriptAction action2;
            idScript script = Level.script;
            idEntity triggerentity;

            action2 = script.FindAction(func.parms[0], func.parms[1], false);
            if (action2 == null)
            {
                script = Level.aiscript;
                action2 = script.FindAction(func.parms[0], func.parms[1], true);
                if (action2 == null)
                {
                    return false;
                }
            }

            triggerentity = Level.FindEntity(func.parms[0]);
            if (triggerentity == null)
            {
                triggerentity = action.actionEntity;
            }

            if (!script.ExecuteTrigger(triggerentity, action2))
            {
                return false;
            }
            return true;
        }

        //----(SA)	added
        private static bool G_ScriptAction_MusicStart(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.net.PlayBackgroundTrackForPlayer(action.actionEntity, func.parms[0]);
            return true; 
        }
        private static bool G_ScriptAction_MusicPlay(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.net.PlayBackgroundTrackForPlayer(action.actionEntity, func.parms[0]);
            return true; 
        }
        private static bool G_ScriptAction_MusicStop(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.net.StopBackgroundTrackForPlayer(action.actionEntity);
            return true;
        }
        private static bool G_ScriptAction_MusicFade(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.net.StopBackgroundTrackForPlayer(action.actionEntity);
            return true;
        }
        private static bool G_ScriptAction_MusicQueue(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.net.StopBackgroundTrackForPlayer(action.actionEntity);
            Level.net.PlayBackgroundTrackForPlayer(action.actionEntity, func.parms[0]);
            return true; 
        }

        private static bool G_ScriptAction_Wait(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            action.nextActionRunTime = Level.time + int.Parse(func.parms[0]);
            action.pendingFunctionNum++;
            return false;
        }

        private static bool G_ScriptAction_PlaySound(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            idSound sound = Engine.soundManager.LoadSound(func.parms[0]);
            sound.Play();
            return true; 
        }

        private static bool G_ScriptAction_AlertEntity(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.TriggerEntity(action.actionEntity, func.parms[0]);
            return true;
        }

        //
        // AI SCRIPT FUNCTIONS
        //

        private static bool AICast_ScriptAction_StartCam(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.camerapath = "cameras/" + func.parms[0] + ".camera";
            Level.cameranum = 0;
            return true;
        }

        private static bool AICast_ScriptAction_StopCam(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.cameranum = -1;
            return true;
        }  //----(SA)	added

        private static bool AICast_ScriptAction_StartCamBlack(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            Level.camerapath = "cameras/" + func.parms[0] + ".camera";
            Level.cameranum = 0;
            return true;
        }

        private static bool AICast_ScriptAction_MusicStart(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent)
        {
            Level.net.PlayBackgroundTrackForPlayer(action.actionEntity, func.parms[0]);
            return true;
        }
        private static bool AICast_ScriptAction_MusicPlay(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent)
        {
            Level.net.PlayBackgroundTrackForPlayer(action.actionEntity, func.parms[0]);
            return true;
        }
        private static bool AICast_ScriptAction_MusicStop(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent)
        {
            Level.net.StopBackgroundTrackForPlayer(action.actionEntity);
            return true;
        }
        private static bool AICast_ScriptAction_MusicFade(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent)
        {
            Level.net.StopBackgroundTrackForPlayer(action.actionEntity);
            return true;
        }
        private static bool AICast_ScriptAction_MusicQueue(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent)
        {
            Level.net.StopBackgroundTrackForPlayer(action.actionEntity);
            Level.net.PlayBackgroundTrackForPlayer(action.actionEntity, func.parms[0]);
            return true;
        }

        private static bool AICast_ScriptAction_Wait(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent)
        {
            action.nextActionRunTime = Level.time + int.Parse(func.parms[0]);
            action.pendingFunctionNum++;
            return false;
        }

        private static bool AICast_ScriptAction_Trigger(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent)
        {
            idScriptAction action2;
            idScript script = Level.script;

            action2 = script.FindAction(func.parms[0], func.parms[1], false);
            if (action2 == null)
            {
                script = Level.aiscript;
                action2 = script.FindAction(func.parms[0], func.parms[1], true);
                if (action2 == null)
                {
                    return false;
                }
            }

            if (!script.ExecuteTrigger(action.actionEntity, action2))
            {
                return false;
            }
            return true;
        }

        private static bool AICast_ScriptAction_PlaySound(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent)
        {
            idSound sound = Engine.soundManager.LoadSound(func.parms[0]);
            sound.Play();
            return true;
        }
        //----(SA)	end

        private static bool AICast_ScriptAction_GotoMarker(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { 
            idEntity marker = Level.FindEntity( func.parms[0] );

            if(marker == null)
            {
                Engine.common.Warning("AICast_ScriptAction_GotoMarker: Failed to find ai_marker %s \n", func.parms[0] );
                return true;
            }

            return ((idBot)action.actionEntity).MoveToEntity(marker); 
        }

        private static bool AICast_ScriptAction_WalkToMarker(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) {
            idEntity marker = Level.FindEntity(func.parms[0]);

            if (marker == null)
            {
                Engine.common.Warning("AICast_ScriptAction_GotoMarker: Failed to find ai_marker %s \n", func.parms[0]);
                return true;
            }


            return ((idBot)action.actionEntity).WalkToEntity(marker);
        }

        #region ScriptStubs_ImplementMe
        private static bool G_ScriptAction_GotoMarker(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        
        private static bool G_ScriptAction_PlayAnim(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_Accum(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_MissionFailed(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_MissionSuccess(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_Print(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_FaceAngles(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_ResetScript(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_TagConnect(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_Halt(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_StopSound(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_StartCam(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_EntityScriptName(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_AIScriptName(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        // DHM - Nerve :: Multiplayer scripting commands
        private static bool G_ScriptAction_MapDescription(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_AxisRespawntime(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_AlliedRespawntime(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_NumberofObjectives(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_ObjectiveAxisDesc(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_ObjectiveAlliedDesc(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_SetWinner(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_SetObjectiveStatus(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_Announce(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_EndRound(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_SetRoundTimelimit(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        // dhm
        private static bool G_ScriptAction_BackupScript(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_RestoreScript(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool G_ScriptAction_SetHealth(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }

        // AI scripting functions
        
        private static bool AICast_ScriptAction_CrouchToMarker(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_GotoCast(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_WalkToCast(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_CrouchToCast(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_AbortIfLoadgame(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; } //----(SA)	added
        private static bool AICast_ScriptAction_FollowCast(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_NoAttack(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Attack(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_PlayAnim(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_ClearAnim(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_SetAmmo(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_SetClip(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }         //----(SA)	added
        private static bool AICast_ScriptAction_SelectWeapon(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_GiveArmor(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }       //----(SA)	added
        private static bool AICast_ScriptAction_SetArmor(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }        //----(SA)	added
        private static bool AICast_ScriptAction_SuggestWeapon(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }   //----(SA)	added
        private static bool AICast_ScriptAction_GiveWeapon(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_GiveInventory(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_TakeWeapon(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Movetype(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_AlertEntity(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_SaveGame(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_FireAtTarget(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_GodMode(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Accum(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_SpawnCast(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_MissionFailed(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_ObjectiveMet(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_ObjectivesNeeded(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_NoAIDamage(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Print(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_FaceTargetAngles(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_ResetScript(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Mount(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Unmount(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_SavePersistant(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_ChangeLevel(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_EndGame(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; } //----(SA)	added
        private static bool AICast_ScriptAction_Teleport(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }    //----(SA)	added
        private static bool AICast_ScriptAction_FoundSecret(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_NoSight(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Sight(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_NoAvoid(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Avoid(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Attrib(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_DenyAction(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_LightningDamage(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Headlook(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_BackupScript(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_RestoreScript(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_StateType(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_KnockBack(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Zoom(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Parachute(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Cigarette(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }    //----(SA)	added
        private static bool AICast_ScriptAction_EntityScriptName(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_AIScriptName(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_SetHealth(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_NoTarget(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_Cvar(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }

        private static bool AICast_ScriptAction_ExplicitRouting(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_LockPlayer(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_AnimCondition(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_PushAway(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        private static bool AICast_ScriptAction_CatchFire(ref idScriptPendingAction action, idScriptFuncBinary func, idScript parent) { return true; }
        #endregion
    }
}
