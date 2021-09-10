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

		private struct AudioLoopPair
		{
			public AudioLoopPair(AudioClip clip, bool loop)
			{
				Clip = clip;
				Loop = loop;
			}

			public AudioClip Clip { get; set; }
			public bool Loop { get; set; }
		}

		private Queue<AudioLoopPair> m_audioQueue;

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
			m_audioQueue = new Queue<AudioLoopPair>();
		}

		public void Update()
		{
			if (!m_audioSrc.isPlaying && m_audioQueue.Count > 0)
			{
				PlayNextInQueue();
			}
		}

		public void QueueAudio(string clipID, bool loop = false)
		{
			AudioClip clip = GameDb.GetAudioClip(clipID);
			m_audioQueue.Enqueue(new AudioLoopPair(clip, loop));
		}

		public void PlayNextInQueue()
		{
			if (m_audioQueue.Count > 0)
			{
				AudioLoopPair pair = m_audioQueue.Dequeue();
				m_audioSrc.clip = pair.Clip;
				m_audioSrc.loop = pair.Loop;
				m_audioSrc.Play();
			}
		}

		public void ClearAudioQueue()
		{
			m_audioQueue.Clear();
		}

		/// <summary>
		/// For short sounds
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayOneShot(string clipID)
		{
			AudioClip clip = GameDb.GetAudioClip(clipID);
			m_audioSrc.PlayOneShot(clip);
		}

		/// <summary>
		/// For longer sounds
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayAudio(string clipID, bool loop = false)
		{
			m_audioSrc.clip = GameDb.GetAudioClip(clipID);
			m_audioSrc.loop = loop;
			m_audioSrc.Play();
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
