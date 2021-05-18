using BeauUtil;
using UnityEngine;

namespace ShipAudio
{
    public class AudioPackageLoader : MonoBehaviour
    {
        #region Inspector

        [SerializeField, Required] private AudioPackage[] m_Packages = null;

        #endregion // Inspector

        private void OnEnable()
        {
            AudioMgr mgr = AudioMgr.Instance;
            foreach(var package in m_Packages)
                mgr.Load(package);
        }

        private void OnDisable()
        {
            AudioMgr mgr = AudioMgr.Instance;
            if (!mgr)
                return;
            
            foreach(var package in m_Packages)
                mgr.Unload(package);
        }
    }
}