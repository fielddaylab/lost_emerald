using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Shipwreck;

namespace Shipwreck.Level
{
    public class LevelManager : MonoBehaviour {

        [SerializeField] public Transform[] levels = null;

        private List<string> LockedLevels = new List<string>();

        private void Start() 
        {
            if(PlayerProgress.instance.IsGameStart()) {
                LockedLevels.Add("2");
                UpdateButtons();
            }
        }

        private void UpdateButtons() 
        {
            foreach(Transform level in levels) {
                var levelID = level.GetComponent<LoadLevel>().LevelID;
                if(LockedLevels.Contains(levelID)) {
                    level.gameObject.SetActive(false);
                }
            }
        }

        public bool UpdateLevel(string levelID) {
            LockedLevels.Remove(levelID);
            

        }
    }
}