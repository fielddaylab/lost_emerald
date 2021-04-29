using System;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Shipwrecks/Level/CaseInfo", fileName = "NewCase")]

public class CaseInfo : ScriptableObject {
    #region Inspector

    [Header("Case Info")]
    [SerializeField] private string m_Location = null;
    [SerializeField] private string m_Type = null;
    [SerializeField] private string m_ShipName = null;
    [SerializeField] private string m_Cargo = null;
    [SerializeField] private string m_Cause = null;
    [SerializeField] private string m_Secret = null;

    [Header("Document")]
    private GameObject m_Document = null;

    #endregion // Inspector
}