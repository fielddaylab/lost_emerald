using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Shipwreck;
using TMPro;
using Shipwreck.Scene;

namespace Shipwreck.EndLevel
{
    public class EndLevelButton : SceneSwitch
    {
        public void EndLevel() {
            PlayerProgress.instance?.SetComplete();
            PlayerProgress.instance.LoadLevel("2");
            Logging.instance?.LogMissionStart("2");
            GotoDesk();
        }
    }
}
