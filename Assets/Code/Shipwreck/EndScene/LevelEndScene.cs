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
        public EndLevelButton returnToDesk = null;
        public Transform PopupEnd = null;

        void Awake() {
            PopupEnd.gameObject.SetActive(false);
            StartCoroutine(EndCutscene());
        }
        
        private void Popup() {
            levelName.SetText(PlayerProgress.instance?.GetCurrentLevel().ToUpper());
            PopupEnd.gameObject.SetActive(true);
        }

        IEnumerator EndCutscene() {
            yield return new WaitForSeconds(3f);
            Popup();
        }
    }
}
