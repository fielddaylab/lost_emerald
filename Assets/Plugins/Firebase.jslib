mergeInto(LibraryManager.library, {

	FBSceneLoad: function(appVersion, logVersion, userCode, missionId, scene, timestamp) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
		var missionId = Pointer_stringify(missionId);
		var scene = Pointer_stringify(scene);
        var timestamp = Pointer_stringify(timestamp);

        analytics.logEvent("scene_load", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
			scene: scene,
            timestamp: timestamp
        });
	},
	
	FBCheckpoint: function(appVersion, logVersion, userCode, missionId, status) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
		var missionId = Pointer_stringify(missionId);
		var status = Pointer_stringify(status);

        analytics.logEvent("checkpoint", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
			status: status
        });
	},

    FBNewEvidence: function(appVersion, logVersion, userCode, missionId, actor, evidenceId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
		var actor = Pointer_stringify(actor);
        var evidenceId = Pointer_stringify(evidenceId);

        analytics.logEvent("new_evidence", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
			actor: actor,
            evidence_id: evidenceId
        });
    },

    FBOpenEvidenceBoard: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("open_evidence_board", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },
	
	FBEvidenceBoardClick: function(appVersion, logVersion, userCode, missionId, evidenceType, factOrigin, factTarget, accurate) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
		var missionId = Pointer_stringify(missionId);
		var evidenceType = Pointer_stringify(evidenceType);
        var factOrigin = Pointer_stringify(factOrigin);
		var factTarget = Pointer_stringify(factTarget);
		var accurate = Pointer_stringify(accurate);

        analytics.logEvent("evidence_board_click", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
			evidenceType: evidenceType,
            factOrigin: factOrigin,
			factTarget: factTarget,
			accurate: accurate
        });
	},

    FBUnlockLocation: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("unlock_location", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBEvidenceBoardComplete: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("evidence_board_complete", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBOpenMap: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("open_map", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },
	
	FBOpenOffice: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("open_office", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBSonarStart: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("sonar_start", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBSonarUpdateProgress: function(appVersion, logVersion, userCode, missionId, percentage) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        var percentage = Pointer_stringify(percentage);
        
        analytics.logEvent("sonar_percentage_update", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
            percentage: percentage
        });
    },

    FBSonarComplete: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("sonar_complete", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBDiveStart: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_start", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBDiveExit: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_exit", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBDiveMoveToNode: function(appVersion, logVersion, userCode, missionId, diveNodeId, targetNodeId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        var targetNodeId = Pointer_stringify(targetNodeId);
        
        analytics.logEvent("dive_moveto_location", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
            dive_node_id: diveNodeId,
            next_node_id: targetNodeId
        });
    },

    FBDiveMoveToAscend: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("dive_moveto_ascend", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBDiveCameraActivate: function(appVersion, logVersion, userCode, missionId, diveNodeId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        
        analytics.logEvent("dive_activate_camera", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
            dive_node_id: diveNodeId
        });
    },
	
	FBDivePhotoClick: function(appVersion, logVersion, userCode, missionId, diveNodeId, accurate) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
		var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
		var accurate = Pointer_stringify(accurate);
        
        analytics.logEvent("dive_photo_click", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
            dive_node_id: diveNodeId,
			accurate: accurate
        });
	},

    FBDiveAllPhotosTaken: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_all_photos_taken", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

	FBDiveJournalClick: function(appVersion, logVersion, userCode, missionId, tasks, clickAction, actor) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
		var missionId = Pointer_stringify(missionId);
        var tasks = Pointer_stringify(tasks);
		var clickAction = Pointer_stringify(clickAction);
		var actor = Pointer_stringify(actor);
        
        analytics.logEvent("dive_journal_click", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
			tasks: tasks,
            clickAction: clickAction,
			actor: actor
        });
	},

	FBConversationClick: function(appVersion, logVersion, userCode, missionId, scene, clickType, character, clickAction) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
		var missionId = Pointer_stringify(missionId);
        var scene = Pointer_stringify(scene);
		var clickType = Pointer_stringify(clickType);
		var character = Pointer_stringify(character);
		var clickAction = Pointer_stringify(clickAction);
        
        analytics.logEvent("conversation_click", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
			scene: scene,
            clickType: clickType,
			character: character,
			clickAction: clickAction
        });
	},

    FBViewCutscene: function(appVersion, logVersion, userCode, missionId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("view_cutscene", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId
        });
    },

    FBViewDialog: function(appVersion, logVersion, userCode, missionId, dialogId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
        var missionId = Pointer_stringify(missionId);
        var dialogId = Pointer_stringify(dialogId);
        
        analytics.logEvent("view_dialog", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
            dialog_id: dialogId
        });
    },
	
	FBCloseInspect: function(appVersion, logVersion, userCode, missionId, scene, itemId) {
        var appVersion = Pointer_stringify(appVersion);
        var userCode = Pointer_stringify(userCode);
		var missionId = Pointer_stringify(missionId);
        var scene = Pointer_stringify(scene);
		var itemId = Pointer_stringify(itemId);
        
        analytics.logEvent("close_inspect", {
            app_version: appVersion,
            log_version: logVersion,
            user_code: userCode,
            mission_id: missionId,
            scene: scene,
			itemId: itemId
        });
	}
});
