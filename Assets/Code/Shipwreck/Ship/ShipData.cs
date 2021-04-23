using System;
using System.Collections.Generic;
using BeauUtil;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Shipwrecks/Ship/Ship", fileName = "NewShip")]
public class ShipData : DBObject
{
    #region Inspector

    [SerializeField] private string m_Id = null;

    [Header("Text")]
    [SerializeField] private string m_ShipName = null;
    [SerializeField] private string m_Description = null;

    [Header("Assets")]
    [SerializeField] private Sprite m_Image = null;

    #endregion

    public string SId() { return m_Id; }

    public string Name() { return m_ShipName; }
    public string Description() { return m_Description; }
    
    public Sprite Image() { return m_Image; }
}
