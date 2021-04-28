using System;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Shipwrecks/Level/Document", fileName = "NewDocument")]

public class Document : ScriptableObject {
    #region Inspector

    [Header("Document Info")]
    [SerializeField] private string m_DocumentName = null;
    [SerializeField] private string[] m_LinkIds = null;
    [SerializeField] private string m_Description = null;

    [Header("Notification")]
    [SerializeField] private string m_Notification = null;

    [Header("Document")]
    [SerializeField] private GameObject m_Document = null;

    #endregion // Inspector
}