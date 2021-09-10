using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Plays the audio
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AudioSrcMgr : MonoBehaviour
	{
		public static AudioSrcMgr instance;

		private AudioSource m_audioSrc;
		private bool m_looping;

		private void Awake()
		{
			// ensure there is only one AudioSrcMgr at any given time
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
			{
				Destroy(this.gameObject);
			}

			m_audioSrc = this.GetComponent<AudioSource>();
			m_looping = false;
		}

		private void Update()
		{
			if (m_looping)
			{
				if (!m_audioSrc.isPlaying)
				{
					m_audioSrc.Play();
				}
			}
		}

		public void PlayAudio(string clipID)
		{
			m_audioSrc.clip = GameDb.GetAudioClip(clipID);
			m_audioSrc.Play();
		}

		public void PlayAudioLoop(string clipID)
		{
			m_looping = true;
			PlayAudio(clipID);
		}

		public void PlayOneShot(string clipID)
		{
			AudioClip clip = GameDb.GetAudioClip(clipID);
			m_audioSrc.PlayOneShot(clip);
		}

		public bool IsPlayingAudio()
		{
			return m_audioSrc.isPlaying;
		}

		public void StopAudio()
		{
			m_audioSrc.Stop();
		}
	}
}
