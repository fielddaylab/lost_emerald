using System;
using System.Collections;
using BeauRoutine;
using UnityEngine;

namespace ShipAudio
{
    public class AudioBGMTrigger : MonoBehaviour
    {
        [SerializeField] private string m_EventId = null;
        [SerializeField] private float m_Crossfade = 0;

        private Routine m_WaitRoutine;
        private AudioHandle m_BGM;

        private void OnEnable()
        {
            m_BGM = AudioMgr.Instance.SetMusic(m_EventId, m_Crossfade);
        }

        private void OnDisable()
        {
            if (AudioMgr.Instance)
            {
                m_WaitRoutine.Stop();
                if (m_BGM.Exists() && AudioMgr.Instance.CurrentMusic().EventId() == m_EventId)
                {
                    m_BGM = default(AudioHandle);
                    AudioMgr.Instance.SetMusic(null, m_Crossfade);
                }
            }
        }
    }
}