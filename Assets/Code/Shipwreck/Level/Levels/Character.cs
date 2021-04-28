using System;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using Shipwreck;
using Shipwreck.Scene;

[CreateAssetMenu(menuName = "Shipwrecks/Character/Character", fileName = "NewCharacter")]

public class Character : ScriptableObject {
    #region Inspector

    [Header("Character")]
    [SerializeField] private string m_CharacterName = null;
    [SerializeField] private string m_FullName = null;

    [Header("Dialogs")]
    [SerializeField] private TextAsset[] m_Dialogs = null;

    [Header("Assets")]
    [SerializeField] private Sprite m_Image = null;

    #endregion // Inspector
}