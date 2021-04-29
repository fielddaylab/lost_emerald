using UnityEngine.UI;
using UnityEngine;
using System;
using Shipwreck;
using Shipwreck.Scene;

public class ReturnToShore : SceneSwitch 
{
    [SerializeField] public Button button = null;

    public void Awake() {
        button.onClick.AddListener(() => GotoDocuments(PlayerProgress.instance?.GetCurrentLevel()));
    }
}