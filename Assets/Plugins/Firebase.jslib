mergeInto(LibraryManager.library, {

    FBMissionStart: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("mission_start", {
            mission_id: missionId
        });
    },

    FBViewTab: function(missionId, tabName) {
        var missionId = Pointer_stringify(missionId);
        var tabName = Pointer_stringify(tabName);

        analytics.logEvent("view_tab", {
            mission_id: missionId,
            tab_name: tabName
        });
    },

    FBViewDesk: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("view_desk", {
            mission_id: missionId
        });
    },

    FBViewChat: function(missionId, chatName) {
        var missionId = Pointer_stringify(missionId);
        var chatName = Pointer_stringify(chatName);

        analytics.logEvent("view_chat", {
            mission_id: missionId,
            chat_name: chatName
        });
    },

    FBOpenMap: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("open_map", {
            mission_id: missionId
        });
    },

    FBScanStart: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("scan_start", {
            mission_id: missionId
        });
    },

    FBScanComplete: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("scan_complete", {
            mission_id: missionId
        });
    },

    FBDiveStart: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("dive_start", {
            mission_id: missionId
        });
    },

    FBPlayerUnlock: function(missionId, unlockKey) {
        var missionId = Pointer_stringify(missionId);
        var unlockKey = Pointer_stringify(unlockKey);

        analytics.logEvent("player_unlock", {
            mission_id: missionId,
            unlock_key: unlockKey
        });
    },

    FBUpdateShipOverview: function(missionId, targetKey, infoKey, infoDisplay, sourceDisplay) {
        var missionId = Pointer_stringify(missionId);
        var targetKey = Pointer_stringify(targetKey);
        var infoKey = Pointer_stringify(infoKey);
        var infoDisplay = Pointer_stringify(infoDisplay);
        var sourceDisplay = Pointer_stringify(sourceDisplay);

        analytics.logEvent("update_ship_overview", {
            mission_id: missionId,
            target_key: targetKey,
            info_key: infoKey,
            info_display: infoDisplay,
            source_display: sourceDisplay
        });
    },

    FBMissionComplete: function(missionId) {
        var missionId = Pointer_stringify(missionId);

        analytics.logEvent("mission_complete", {
            mission_id: missionId
        });
    }

});
