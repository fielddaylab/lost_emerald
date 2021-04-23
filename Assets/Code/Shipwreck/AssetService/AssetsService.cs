using BeauData;
using BeauUtil;
using UnityEngine;

public class AssetsService : MonoBehaviour
{
    #region Inspector

    [SerializeField, Required] private ShipDB m_Ships = null;

    #endregion // Inspector

    public ShipDB Ships { get { return m_Ships; } }

    protected void Initialize()
    {
        m_Ships.Initialize();
    }
}
