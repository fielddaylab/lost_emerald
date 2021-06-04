using System;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Shipwrecks/Level/Level", fileName = "NewLevel")]
public class LevelObject : DBObject
{
    #region Inspector

    [SerializeField] private string m_LevelID = null;

    //[Header("Characters")]
    //[SerializeField] private Character[] m_Characters = null;

    //[Header("Photos")]
    //[SerializeField] private PhotoSlot[] m_Photos = null;

    //[Header("Documents")]
    //[SerializeField] private Document[] m_Documents = null;

    //[Header("Case Info")]
    //[SerializeField] private CaseInfo m_CaseInfo = null;

    [Header("Assets")]
    [SerializeField] private Sprite m_Image = null;

    #endregion
}
