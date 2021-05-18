using System;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;

[CreateAssetMenu(menuName = "Shipwrecks/Level/Level Database", fileName = "NewLevelDB")]
public class LevelDB : DBObjectCollection<LevelObject>
{
    #if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(LevelDB))]
    private class Inspector : BaseInspector
    {}

    #endif // UNITY_EDITOR
}
