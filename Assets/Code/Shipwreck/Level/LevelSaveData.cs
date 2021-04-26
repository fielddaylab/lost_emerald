using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCP;
using System;
using Shipwreck;

namespace Shipwreck.Level
{

    public class LevelSaveData
    {
        private string levelName = null;
        public Dictionary<string, PlayerProgress.InfoEntry> ShipLog { get; set; }
        public HashSet<string> Unlocks { get; set; }

        public bool IsLevel(string level) {
            return levelName == level;
        }

        public void Clear() {
            ShipLog.Clear();
            Unlocks.Clear();
        }

        //load document info, documents, evidences

    }
}
