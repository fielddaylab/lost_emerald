mergeInto(LibraryManager.library, {

    FBMissionStart: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("mission_start", {
            mission_id: missionId
        });
    },

    FBMissionComplete: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("mission_complete", {
            mission_id: missionId
        });
    },

    FBMissionUnlock: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("mission_unlock", {
            mission_id: missionId
        });
    },

    FBNewEvidence: function(missionId, evidenceKey) {
        var missionId = Pointer_stringify(missionId);
        var evidenceKey = Pointer_stringify(evidenceKey);

        analytics.logEvent("new_evidence", {
            mission_id: missionId,
            evidence_key: evidenceKey
        });
    },

    FBOpenEvidenceBoard: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("open_evidence_board", {
            mission_id: missionId
        });
    },

    FBEvidenceChainHint: function(missionId, chainName, feedbackKey) {
        var missionId = Pointer_stringify(missionId);
        var chainName = Pointer_stringify(chainName);
        var feedbackKey = Pointer_stringify(feedbackKey);

        analytics.logEvent("evidence_chain_hint", {
            mission_id: missionId,
            chain_name: chainName,
            feedback_key: feedbackKey
        });
    },

    FBEvidenceChainIncorrect: function(missionId, chainName, feedbackKey) {
        var missionId = Pointer_stringify(missionId);
        var chainName = Pointer_stringify(chainName);
        var feedbackKey = Pointer_stringify(feedbackKey);

        analytics.logEvent("evidence_chain_incorrect", {
            mission_id: missionId,
            chain_name: chainName,
            feedback_key: feedbackKey
        });
    },

    FBEvidenceChainCorrect: function(missionId, chainName, feedbackKey) {
        var missionId = Pointer_stringify(missionId);
        var chainName = Pointer_stringify(chainName);
        var feedbackKey = Pointer_stringify(feedbackKey);

        analytics.logEvent("evidence_chain_correct", {
            mission_id: missionId,
            chain_name: chainName,
            feedback_key: feedbackKey
        });
    },

    FBUnlockLocation: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("unlock_location", {
            mission_id: missionId,
        });
    },

    FBEvidenceBoardComplete: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("evidence_board_complete", {
            mission_id: missionId,
        });
    },

    FBOpenOffice: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("open_office", {
            mission_id: missionId,
        });
    },

    FBOpenMap: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("open_map", {
            mission_id: missionId,
        });
    },

    FBSonarStart: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("sonar_start", {
            mission_id: missionId,
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
            mission_id: missionId,
        });
    },

    FBDiveStart: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_start", {
            mission_id: missionId,
        });
    },

    FBDiveExit: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_exit", {
            mission_id: missionId,
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
            mission_id: missionId,
        });
    },

    FBDiveCameraActivate: function(missionId, diveNodeId) {
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        
        analytics.logEvent("dive_activate_camera", {
            mission_id: missionId,
            dive_node_id: diveNodeId,
        });
    },

    FBDiveNewPhoto: function(missionId, diveNodeId) {
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        
        analytics.logEvent("dive_new_photo", {
            mission_id: missionId,
            dive_node_id: diveNodeId,
        });
    },

    FBDivePhotoFail: function(missionId, diveNodeId) {
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        
        analytics.logEvent("dive_photo_fail", {
            mission_id: missionId,
            dive_node_id: diveNodeId,
        });
    },

    FBDivePhotoAlreadyTaken: function(missionId, diveNodeId) {
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        
        analytics.logEvent("dive_photo_already_taken", {
            mission_id: missionId,
            dive_node_id: diveNodeId,
        });
    },

    FBDiveNoPhotoAvailable: function(missionId, diveNodeId) {
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        
        analytics.logEvent("dive_no_photo_available", {
            mission_id: missionId,
            dive_node_id: diveNodeId,
        });
    },

    FBDiveAllPhotosTaken: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("dive_all_photos_taken", {
            mission_id: missionId,
        });
    },

    FBDiveJournalOpen: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        var diveNodeId = Pointer_stringify(diveNodeId);
        
        analytics.logEvent("dive_journal_open", {
            mission_id: missionId,
        });
    },

    FBViewCutscene: function(missionId) {
        var missionId = Pointer_stringify(missionId);
        
        analytics.logEvent("view_cutscene", {
            mission_id: missionId,
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

});
