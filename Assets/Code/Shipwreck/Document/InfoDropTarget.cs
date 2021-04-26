using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Shipwreck;

namespace Shipwreck.Document
{
    public class InfoDropTarget : MonoBehaviour
    {
        public string targetKey;
        public TextMeshProUGUI infoLabel;
        public TextMeshProUGUI sourceLabel;
        public string correctInfoKey;

        public bool isFilled = false;

        // Start is called before the first frame update
        void Start()
        {
            PlayerProgress.instance?.FillInfo(this, true);
        }

        public void Fill(string info, string source) {
            infoLabel.SetText(info);
            sourceLabel.SetText(source);
            isFilled = true;
        }

        public bool IsFilled() {
            return isFilled;
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}
