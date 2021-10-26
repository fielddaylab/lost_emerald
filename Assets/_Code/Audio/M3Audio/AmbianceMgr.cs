using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Manages Ambiance audio in a scene
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AmbianceMgr : MonoBehaviour, IAudioPlayer
	{
		private AudioSource m_ambianceSrc;

		private AudioSrcMgr.AudioLoopPair m_stashedAudio;
		private AudioData m_currData;
		private Queue<AudioSrcMgr.AudioLoopPair> m_audioQueue;

		#region Unity Callbacks

		private void Awake()
		{
			m_ambianceSrc = this.GetComponent<AudioSource>();
		}

		private void Start()
		{
			CutscenePlayer.OnVideoComplete += ResumeAudio;
		}

		#endregion

		#region IAudioPlayer

		/// <summary>
		/// For longer sounds
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayAudio(string clipID, bool loop = false)
		{
			LoadAmbianceAudio(clipID);
			m_ambianceSrc.loop = loop;
			m_ambianceSrc.Play();
		}

		public void PlayAudioWhen(string clipIDToPlay, string clipIDPlayWhen, bool loop = false) {
			// todo: implement this
		}

		public bool IsPlayingAudio()
		{
			return m_ambianceSrc.isPlaying;
		}

		public void StopAudio()
		{
			m_ambianceSrc.Stop();
		}

		public void ClearAudio() {
			m_currData = null;
			m_ambianceSrc.clip = null;
		}

		public void ResumeAudio()
		{
			if (m_ambianceSrc.clip == null) {
				return;
			}
			m_ambianceSrc.Play();
		}

		// Saves the current audio for later
		public void StashAudio()
		{
			m_stashedAudio = new AudioSrcMgr.AudioLoopPair(m_currData, m_ambianceSrc.loop);
		}

		// Saves the current audio for later
		public void ResumeStashedAudio()
		{
			if (m_stashedAudio.Data == null) {
				ClearAudio();
				return;
			}

			m_currData = m_stashedAudio.Data;
			AudioSrcMgr.LoadAudio(m_ambianceSrc, m_stashedAudio.Data);
			m_ambianceSrc.loop = m_stashedAudio.Loop;
			m_ambianceSrc.Play();
		}

		#endregion

		#region Helper Methods

		private void LoadAmbianceAudio(string clipID)
		{
			var data = GameDb.GetAudioData(clipID);
			m_currData = data;
			AudioSrcMgr.LoadAudio(m_ambianceSrc, data);
		}

		#endregion
	}
}
