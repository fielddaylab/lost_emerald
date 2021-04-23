using System;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;

[CreateAssetMenu(menuName = "Shipwrecks/Ship/Ship Database", fileName = "NewShipDB")]
public class ShipDB : DBObjectCollection<ShipData>
{
    #if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(ShipDB))]
    private class Inspector : BaseInspector
    {}

    #endif // UNITY_EDITOR
}
