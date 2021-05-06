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
            levelID = PlayerProgress.instance?.GetCurrentLevel();
            PlayerProgress.instance.PrevLevel = levelID;
            StartCoroutine(EndCutscene());
        }
        
        private void Popup() {
            levelName.SetText(levelID.ToUpper());
            PlayerProgress.instance.SetComplete();
            PopupEnd.gameObject.SetActive(true);
        }

        IEnumerator EndCutscene() {
            yield return new WaitForSeconds(5f);
            Popup();
        }
    }
}
