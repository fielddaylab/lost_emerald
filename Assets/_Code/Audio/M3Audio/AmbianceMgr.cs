using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Manages Ambiance audio in a scene
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AmbianceMgr : MonoBehaviour
	{
		private struct AudioLoopPair
		{
			public AudioLoopPair(AudioData data, bool loop)
			{
				Data = data;
				Loop = loop;
			}

			public AudioData Data { get; set; }
			public bool Loop { get; set; }
		}

		private AudioSource m_ambianceSrc;

		private AudioLoopPair m_stashedAudio;
		private AudioData m_currData;
		private Queue<AudioLoopPair> m_audioQueue;

		private void Awake()
		{
			m_ambianceSrc = this.GetComponent<AudioSource>();
			// m_audioQueue = new Queue<AudioLoopPair>();
		}

		public void Start()
		{
			CutscenePlayer.OnVideoComplete += ResumeAudio;
		}

		public void Update()
		{
			/*
			if (!m_audioSrc.isPlaying && m_audioQueue.Count > 0)
			{
				PlayNextInQueue();
			}
			*/
		}

		/*
		public void QueueAudio(string clipID, bool loop = false)
		{
			AudioData data = GameDb.GetAudioData(clipID);
			m_audioQueue.Enqueue(new AudioLoopPair(data, loop));
		}

		public void PlayNextInQueue()
		{
			if (m_audioQueue.Count > 0)
			{
				AudioLoopPair pair = m_audioQueue.Dequeue();
				InitializeAudio(m_audioSrc, pair.Data);
				m_audioSrc.loop = pair.Loop;
				m_audioSrc.Play();
			}
		}
		

		public void ClearAudioQueue()
		{
			m_audioQueue.Clear();
		}
		*/

		/// <summary>
		/// For longer sounds
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayAudio(string clipID, bool loop = false)
		{
			AudioSrcMgr.instance.InitializeAudio(m_ambianceSrc, GameDb.GetAudioData(clipID));
			m_ambianceSrc.loop = loop;
			m_ambianceSrc.Play();
		}

		public bool IsPlayingAudio()
		{
			return m_ambianceSrc.isPlaying;
		}

		public void StopAudio()
		{
			m_ambianceSrc.Stop();
		}
		public void ResumeAudio()
		{
			m_ambianceSrc.Play();
		}

		// Saves the current audio for later
		public void StashAudio()
		{
			m_stashedAudio = new AudioLoopPair(m_currData, m_ambianceSrc.loop);
		}

		// Saves the current audio for later
		public void ResumeStashedAudio()
		{
			if (m_stashedAudio.Data == null) { return; }

			AudioSrcMgr.instance.InitializeAudio(m_ambianceSrc, m_stashedAudio.Data);
			m_ambianceSrc.loop = m_stashedAudio.Loop;
			m_ambianceSrc.Play();
		}
	}
}
