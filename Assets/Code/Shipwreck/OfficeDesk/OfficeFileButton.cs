using UnityEngine;
using Shipwreck;
using Shipwreck.Scene;
using System;
using UnityEngine.UI;

public class OfficeFileButton : SceneSwitch {
    [SerializeField] private Button button;

    void Start() {
        string levelID = PlayerProgress.instance.GetCurrentLevel();
        button.interactable = levelID != null;
        if (button.IsInteractable())
        {
            button.onClick.AddListener(() => GotoDocuments(levelID));
        }
    }
}