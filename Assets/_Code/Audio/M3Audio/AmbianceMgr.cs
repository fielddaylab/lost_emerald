﻿using System.Collections;
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

		public void MuteAudio(bool isMute) {
			m_ambianceSrc.mute = isMute;
		}
		public bool IsMute() {
			return m_ambianceSrc.mute;
		}

		public void CrossFadeAudio(string clipID, float time, bool loop = false) {
			StartCoroutine(CrossFadeRoutine(clipID, time, loop));
		}

		#endregion

		#region Helper Methods

		private void LoadAmbianceAudio(string clipID)
		{
			var data = GameDb.GetAudioData(clipID);
			m_currData = data;
			AudioSrcMgr.LoadAudio(m_ambianceSrc, data);
		}

		private IEnumerator CrossFadeRoutine(string clipID, float time, bool loop) {
			float maxVolume = m_ambianceSrc.volume;
			float midTime = time / 2f;
			bool transitioned = false;

			float volumeStep = maxVolume / midTime;

			for (float t = 0; t < time; t += Time.deltaTime) {
				if (t >= midTime && !transitioned) {
					// load new audio
					m_ambianceSrc.Stop();
					AudioData newData = GameDb.GetAudioData(clipID);
					m_currData = newData;
					AudioSrcMgr.LoadAudio(m_ambianceSrc, newData);
					m_ambianceSrc.volume = volumeStep * Time.deltaTime;
					m_ambianceSrc.loop = loop;
					m_ambianceSrc.Play();
					transitioned = true;
				}
				if (!transitioned) {
					m_ambianceSrc.volume -= volumeStep * Time.deltaTime;
				}
				else {
					m_ambianceSrc.volume += volumeStep * Time.deltaTime;
				}

				yield return null;
			}

			m_ambianceSrc.volume = maxVolume;
		}

		#endregion
	}
}
