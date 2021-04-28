using UnityEngine;
using System;
using Shipwreck;
using Shipwreck.Document;
using System.Collections.Generic;
using Shipwreck.Scene;

public class DocumentReloader : MonoBehaviour {

    [SerializeField] private string levelID;
    [SerializeField] private InfoDropTarget[] targets;
    [SerializeField] private PhotoSlot[] photos;
    [SerializeField] private DocumentButton[] documents;

    public void Awake() {
        if(SceneSwitch.SceneHelper.IsReloaded()) {
            PlayerProgress.instance.ReloadLevel(levelID);
        }
    }

    public void ReloadDocument() {
        foreach(var target in targets) {
            PlayerProgress.instance.FillInfo(target);
        }
        foreach(var photo in photos) {
            PlayerProgress.instance.UpdatePhoto(photo);
        }
        foreach(var document in documents) {
            PlayerProgress.instance.UpdateDocumentPresence(document);
        }

    }
}