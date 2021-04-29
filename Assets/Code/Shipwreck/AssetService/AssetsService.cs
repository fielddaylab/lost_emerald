using System;
using BeauUtil;
using UnityEngine;

public class AssetsService : MonoBehaviour
{
    #region Inspector

    // [SerializeField, Required] private ShipDB m_Ships = null;
    [SerializeField, Required] private LevelDB m_Levels = null;

    #endregion // Inspector

    public LevelDB Levels { get { return m_Levels; } }

    protected void Initialize()
    {
        m_Levels.Initialize();
    }
}
