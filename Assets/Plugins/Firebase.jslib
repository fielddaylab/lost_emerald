mergeInto(LibraryManager.library, {

	FBSceneLoad: function(missionId, scene, timestamp) {
		var missionId = Pointer_stringify(missionId);
		var scene = Pointer_stringify(scene);
        var timestamp = Pointer_stringify(timestamp);

        analytics.logEvent("scene_load", {
            mission_id: missionId,
			scene: scene,
            timestamp: timestamp
        });
	},
	
	FBCheckpoint: function(missionId, status) {
		var missionId = Pointer_stringify(missionId);
		var status = Pointer_stringify(status);

        analytics.logEvent("checkpoint", {
            mission_id: missionId,
			status: status
        });
	},

    FBNewEvidence: function(missionId, actor, evidenceId) {
        var missionId = Pointer_stringify(missionId);
		var actor = Pointer_stringify(actor);
        var evidenceId = Pointer_stringify(evidenceId);

        analytics.logEvent("new_evidence", {
            mission_id: missionId,
			actor: actor,
            evidence_id: evidenceId
        });
    },

    FBOpenEvidenceBoard: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("open_evidence_board", {
            mission_id: missionId
        });
    },
	
	FBEvidenceBoardClick: function(missionId, evidenceType, factOrigin, factTarget, accurate) {
		var missionId = Pointer_stringify(missionId);
		var evidenceType = Pointer_stringify(evidenceType);
        var factOrigin = Pointer_stringify(factOrigin);
		var factTarget = Pointer_stringify(factTarget);
		var accurate = Pointer_stringify(accurate);

        analytics.logEvent("evidence_board_click", {
            mission_id: missionId,
			evidenceType: evidenceType,
            factOrigin: factOrigin,
			factTarget: factTarget,
			accurate: accurate
        });
	},

    FBUnlockLocation: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("unlock_location", {
            mission_id: missionId
        });
    },

    FBEvidenceBoardComplete: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("evidence_board_complete", {
            mission_id: missionId
        });
    },

    FBOpenMap: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("open_map", {
            mission_id: missionId
        });
    },
	
	FBOpenOffice: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("open_office", {
            mission_id: missionId
        });
    },

    FBSonarStart: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("sonar_start", {
            mission_id: missionId
        });
    },

    FBSonarUpdateProgress: function(missionId, percentage) {
        var missionId = Pointer_stringify(missionId);
        var percentage = Pointer_stringify(percentage);
        
        analytics.logEvent("sonar_percentage_update", {
            mission_id: missionId,
            percentage: percentage
        });
    },

    FBSonarComplete: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("sonar_complete", {
            mission_id: missionId
        });
    },

    FBDiveStart: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_start", {
            mission_id: missionId
        });
    },

    FBDiveExit: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_exit", {
            mission_id: missionId
        });
    },

    FBDiveMoveToNode: function(missionId, diveNodeId, targetNodeId) {
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        var targetNodeId = Pointer_stringify(targetNodeId);
        
        analytics.logEvent("dive_moveto_location", {
            mission_id: missionId,
            dive_node_id: diveNodeId,
            next_node_id: targetNodeId
        });
    },

    FBDiveMoveToAscend: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("dive_moveto_ascend", {
            mission_id: missionId
        });
    },

    FBDiveCameraActivate: function(missionId, diveNodeId) {
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        
        analytics.logEvent("dive_activate_camera", {
            mission_id: missionId,
            dive_node_id: diveNodeId
        });
    },
	
	FBDivePhotoClick: function(missionId, diveNodeId, accurate) {
		var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
		var accurate = Pointer_stringify(accurate);
        
        analytics.logEvent("dive_photo_click", {
            mission_id: missionId,
            dive_node_id: diveNodeId,
			accurate: accurate
        });
	},

    FBDiveAllPhotosTaken: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_all_photos_taken", {
            mission_id: missionId
        });
    },

	FBDiveJournalClick: function(missionId, tasks, clickAction, actor) {
		var missionId = Pointer_stringify(missionId);
        var tasks = Pointer_stringify(tasks);
		var clickAction = Pointer_stringify(clickAction);
		var actor = Pointer_stringify(actor);
        
        analytics.logEvent("dive_journal_click", {
            mission_id: missionId,
			tasks: tasks,
            clickAction: clickAction,
			actor: actor
        });
	},

	FBConversationClick: function(missionId, scene, clickType, character, clickAction) {
		var missionId = Pointer_stringify(missionId);
        var scene = Pointer_stringify(scene);
		var clickType = Pointer_stringify(clickType);
		var character = Pointer_stringify(character);
		var clickAction = Pointer_stringify(clickAction);
        
        analytics.logEvent("conversation_click", {
            mission_id: missionId,
			scene: scene,
            clickType: clickType,
			character: character,
			clickAction: clickAction
        });
	},

    FBViewCutscene: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("view_cutscene", {
            mission_id: missionId
        });
    },

    FBViewDialog: function(missionId, dialogId) {
        var missionId = Pointer_stringify(missionId);
        var dialogId = Pointer_stringify(dialogId);
        
        analytics.logEvent("view_dialog", {
            mission_id: missionId,
            dialog_id: dialogId
        });
    },
	
	FBCloseInspect: function(missionId, scene, itemId) {
		var missionId = Pointer_stringify(missionId);
        var scene = Pointer_stringify(scene);
		var itemId = Pointer_stringify(itemId);
        
        analytics.logEvent("view_dialog", {
            mission_id: missionId,
            scene: scene,
			itemId: itemId
        });
	}
});
