# The Legend of the Lost Emerald
A grade 3-6 underwater archilogy practices games for shipwrecks in the great lakes

![WebGL Build](https://github.com/fielddaylab/shipwreck/workflows/WebGL%20Build/badge.svg)

## Change Log
1. Initial version (4/12/22)

## Telementry Events
* scene_load (appVersion, logVersion, missionId, scene, timestamp)
* checkpoint (appVersion, logVersion, missionId, status)
* new_evidence (appVersion, logVersion, mission id, actor, evidence id)
* open_evidence_board (appVersion, logVersion, mission id)
* evidence_board_click (appVersion, logVersion, missionId, evidenceType, factOrigin, factTarget, accurate)
* unlock_location (appVersion, logVersion, mission id)
* evidence_board_complete (appVersion, logVersion, mission id, evidence chain, feedback id)
* open_map (appVersion, logVersion, mission id)
* open_office (appVersion, logVersion, mission id)
* sonar_start (appVersion, logVersion, mission id)
* sonar_percentage_update (appVersion, logVersion, mission id, percentage [0-100])
* sonar_complete (appVersion, logVersion, mission id)
* dive_start (appVersion, logVersion, mission id)
* dive_exit (appVersion, logVersion, mission id)
* dive_moveto_location (appVersion, logVersion, mission id, dive node id, next node id)
* dive_moveto_ascend (appVersion, logVersion, mission id, dive node id)
* dive_activate_camera (appVersion, logVersion, mission id, dive node id)
* dive_photo_click (appVersion, logVersion, missionId, diveNodeId, accurate)
* dive_all_photos_taken (appVersion, logVersion, mission id)
* dive_journal_click (appVersion, logVersion, missionId, tasks, clickAction, actor)
* conversation_click (appVersion, logVersion, missionId, scene, clickType, character, clickAction)
* view_cutscene (appVersion, logVersion, mission id)
* view_dialog (appVersion, logVersion, mission id, dialog id)
* close_inspect (appVersion, logVersion, missionId, scene, itemId)
