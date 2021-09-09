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
		}

		public void PlayAudio(string clipID)
		{
			m_audioSrc.clip = GameDb.GetAudioClip(clipID);
			m_audioSrc.Play();
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
