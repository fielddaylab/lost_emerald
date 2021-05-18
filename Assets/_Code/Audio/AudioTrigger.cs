using System;
using System.Collections;
using BeauRoutine;
using UnityEngine;

namespace ShipAudio
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] private string m_EventId = null;

        private Routine m_WaitRoutine;
        private AudioHandle m_Playback;

        private void OnEnable()
        {
            m_Playback = AudioMgr.Instance.PostEvent(m_EventId);
        }

        private void OnDisable()
        {
            m_WaitRoutine.Stop();
            m_Playback.Stop(0.1f);
        }
    }
}