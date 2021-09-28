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

		[SerializeField]
		private AmbianceMgr m_ambianceMgr;

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

		private AudioLoopPair m_stashedAudio;
		private AudioData m_currData;
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

		public void Start()
		{
			CutscenePlayer.OnVideoComplete += ResumeAudio;
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

		/// <summary>
		/// For short sounds
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayOneShot(string clipID)
		{
			AudioClip clip = GameDb.GetAudioData(clipID).Clip;
			m_audioSrc.PlayOneShot(clip);
		}

		/// <summary>
		/// For longer sounds
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayAudio(string clipID, bool loop = false)
		{
			InitializeAudio(m_audioSrc, GameDb.GetAudioData(clipID));
			m_audioSrc.loop = loop;
			m_audioSrc.Play();
		}

		/// <summary>
		/// Delegates call to Ambiance Mgr
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayAmbiance(string clipID, bool loop = false)
		{
			m_ambianceMgr.PlayAudio(clipID, loop);
		}

		public bool IsPlayingAudio()
		{
			return m_audioSrc.isPlaying;
		}

		public void StopAudio()
		{
			m_audioSrc.Stop();
		}

		public void StopAmbiance()
		{
			m_ambianceMgr.StopAudio();
		}

		public void ResumeAudio()
		{
			m_audioSrc.Play();
		}

		// Saves the current audio for later
		public void StashAudio()
		{
			m_stashedAudio = new AudioLoopPair(m_currData, m_audioSrc.loop);
			StashAmbiance();
		}

		public void StashAmbiance()
		{
			m_ambianceMgr.StashAudio();
		}

		// Saves the current audio for later
		public void ResumeStashedAudio()
		{
			if (m_stashedAudio.Data == null) { return; }

			InitializeAudio(m_audioSrc, m_stashedAudio.Data);
			m_audioSrc.loop = m_stashedAudio.Loop;
			m_audioSrc.Play();

			ResumeStashedAmbiance();
		}

		public void ResumeStashedAmbiance()
		{
			m_ambianceMgr.ResumeAudio();
		}

		public void InitializeAudio(AudioSource source, AudioData data)
		{ 
			source.clip = data.Clip;
			source.volume = data.Volume;
			source.panStereo = data.Pan;

			if (source == m_audioSrc)
			{
				m_currData = data;
			}
		}
	}
}
