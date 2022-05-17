# The Legend of the Lost Emerald
A grade 3-6 underwater archilogy practices games for shipwrecks in the great lakes

![WebGL Build](https://github.com/fielddaylab/shipwreck/workflows/WebGL%20Build/badge.svg)

## Change Log
1. Initial version (4/24/22)
2. Add sequence index (5/17/22)

## Telementry Events
* scene_load (sequenceIndex, appVersion, logVersion, missionId, scene, timestamp)
* checkpoint (sequenceIndex, appVersion, logVersion, missionId, status)
* new_evidence (sequenceIndex, appVersion, logVersion, mission id, actor, evidence id)
* open_evidence_board (sequenceIndex, appVersion, logVersion, mission id)
* evidence_board_click (sequenceIndex, appVersion, logVersion, missionId, evidenceType, factOrigin, factTarget, accurate)
* unlock_location (sequenceIndex, appVersion, logVersion, mission id)
* evidence_board_complete (sequenceIndex, appVersion, logVersion, mission id, evidence chain, feedback id)
* open_map (sequenceIndex, appVersion, logVersion, mission id)
* open_office (sequenceIndex, appVersion, logVersion, mission id)
* sonar_start (sequenceIndex, appVersion, logVersion, mission id)
* sonar_percentage_update (sequenceIndex, appVersion, logVersion, mission id, percentage [0-100])
* sonar_complete (sequenceIndex, appVersion, logVersion, mission id)
* dive_start (sequenceIndex, appVersion, logVersion, mission id)
* dive_exit (sequenceIndex, appVersion, logVersion, mission id)
* dive_moveto_location (sequenceIndex, appVersion, logVersion, mission id, dive node id, next node id)
* dive_moveto_ascend (sequenceIndex, appVersion, logVersion, mission id, dive node id)
* dive_activate_camera (sequenceIndex, appVersion, logVersion, mission id, dive node id)
* dive_photo_click (sequenceIndex, appVersion, logVersion, missionId, diveNodeId, accurate)
* dive_all_photos_taken (sequenceIndex, appVersion, logVersion, mission id)
* dive_journal_click (sequenceIndex, appVersion, logVersion, missionId, tasks, clickAction, actor)
* conversation_click (sequenceIndex, appVersion, logVersion, missionId, scene, clickType, character, clickAction)
* view_cutscene (sequenceIndex, appVersion, logVersion, mission id)
* view_dialog (sequenceIndex, appVersion, logVersion, mission id, dialog id)
* close_inspect (sequenceIndex, appVersion, logVersion, missionId, scene, itemId)
