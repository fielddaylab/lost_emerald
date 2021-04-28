using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using Shipwreck;
using TMPro;

namespace Shipwreck.EndLevel
{
    public class LevelEndScene : MonoBehaviour {

        public TextMeshProUGUI levelName = null;
        public Transform PopupEnd = null;

        private string levelID = null;

        void Awake() {
            PopupEnd.gameObject.SetActive(false);
            StartCoroutine(EndCutscene());
        }
        
        private void Popup() {
            levelID = PlayerProgress.instance?.GetCurrentLevel();
            levelName.SetText(levelID.ToUpper());
            PlayerProgress.instance.SetComplete();
            PlayerProgress.instance.PrevLevel = levelID;
            PopupEnd.gameObject.SetActive(true);
        }

        IEnumerator EndCutscene() {
            yield return new WaitForSeconds(3f);
            Popup();
        }
    }
}
