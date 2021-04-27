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
            if(PlayerProgress.instance.IsGameStart() && LockedLevels.Count == 0) {
                LockedLevels.Add("2");
                UpdateButtons();
            }
        }

        private void UpdateButtons() 
        {
            foreach(Transform level in levels) {
                var levelID = level.GetComponent<LoadLevel>().LevelID;
                if(LockedLevels.Contains(levelID)) {
                    level.GetComponent<Button>().enabled = false;
                    level.GetComponent<Image>().color = Color.gray;
                }
                else {
                    level.GetComponent<Button>().enabled = true;
                    level.GetComponent<Image>().color = Color.white;
                }
            }
        }

        public void UpdateLevel(string levelID) {
            LockedLevels.Remove(levelID);
            UpdateButtons();
        }
    }
}