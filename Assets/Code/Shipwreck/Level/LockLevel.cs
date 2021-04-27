using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Shipwreck;

public class LockLevel : MonoBehaviour
{
    public string levelID;

    private bool isUnlocked;

    void Start() {
        levelID = this.GetComponent<LoadLevel>().LevelID;

    }
}
