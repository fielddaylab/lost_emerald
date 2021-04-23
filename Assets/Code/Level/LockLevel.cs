using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LockLevel : MonoBehaviour
{
    public string[] LevelIDs;

    void OnEnable()
    {
        string currentLevel = PlayerProgress.instance.GetCurrentLevel();
        if (Array.Exists(LevelIDs, (x) => x == currentLevel))
        {
            this.gameObject.SetActive(false);
        }
    }
}
