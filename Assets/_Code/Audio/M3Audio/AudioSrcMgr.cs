using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Central audio player in the game.
	/// Delegates surrounding sounds to AmbianceMgr and DialogAudioMgr 
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AudioSrcMgr : MonoBehaviour, IAudioPlayer
	{
		public static AudioSrcMgr instance;

		public struct AudioLoopPair
		{
			public AudioLoopPair(AudioData data, bool loop)
			{
				Data = data;
				Loop = loop;
			}

			public AudioData Data { get; set; }
			public bool Loop { get; set; }
		}

		#region Inspector

		[SerializeField]
		private AmbianceMgr m_ambianceMgr;
		[SerializeField]
		private DialogAudioMgr m_dialogAudioMgr;

		#endregion

		private AudioSource m_audioSrc;
		private AudioLoopPair m_stashedAudio;
		private AudioData m_currData;
		private Queue<AudioLoopPair> m_audioQueue;

		#region Static Functions

		public static void LoadAudio(AudioSource source, AudioData data)
		{
			source.clip = data.Clip;
			source.volume = data.Volume;
			source.panStereo = data.Pan;
		}

		#endregion

		#region Unity Callbacks

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

		private void Start()
		{
			CutscenePlayer.OnVideoComplete += ResumeStashedAudio;
		}

		private void Update()
		{
			if (!m_audioSrc.isPlaying && m_audioQueue.Count > 0)
			{
				PlayNextInQueue();
			}
		}

		#endregion

		#region IAudioPlayer

		public void PlayAudio(string clipID, bool loop = false)
		{
			AudioData newData = GameDb.GetAudioData(clipID);
			m_currData = newData;
			LoadAudio(m_audioSrc, newData);
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
			//m_audioSrc.clip = null;
		}

		public void ResumeAudio()
		{
			if (m_audioSrc.clip == null) {
				return;
			}
			m_audioSrc.Play();
		}

		public void StashAudio()
		{
			m_stashedAudio = new AudioLoopPair(m_currData, m_audioSrc.loop);
			StashAmbiance();
		}

		public void ResumeStashedAudio()
		{
			if (m_stashedAudio.Data == null) {
				m_audioSrc.Stop();
				m_currData = null;
				ResumeStashedAmbiance();
				return;
			}

			m_currData = m_stashedAudio.Data;
			LoadAudio(m_audioSrc, m_stashedAudio.Data);
			m_audioSrc.loop = m_stashedAudio.Loop;
			m_audioSrc.Play();

			ResumeStashedAmbiance();
		}

		#endregion

		#region AmbianceAudioMgr

		/// <summary>
		/// Delegates call to Ambiance Mgr
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayAmbiance(string clipID, bool loop = false)
		{
			m_ambianceMgr.PlayAudio(clipID, loop);
		}

		/// <summary>
		/// Trigger ambiance when a certain audio clip starts
		/// </summary>
		/// <param name="clipIDToPlay"></param>
		/// <param name="clipIDPlayWhen"></param>
		/// <param name="loop"></param>
		public void PlayAmbianceWhenAudio(string clipIDToPlay, string clipIDPlayWhen, bool loop = false) {
			m_ambianceMgr.PlayAudioWhen(clipIDToPlay, clipIDPlayWhen, loop = false);
		}

		public void StopAmbiance()
		{
			m_ambianceMgr.StopAudio();
		}

		public void ClearAmbiance() {
			m_ambianceMgr.ClearAudio();
		}

		public void StashAmbiance()
		{
			m_ambianceMgr.StashAudio();
		}

		public void ResumeStashedAmbiance()
		{
			m_ambianceMgr.ResumeStashedAudio();
		}

		#endregion

		#region DialogAudioMgr

		public void StartLineAudio(DialogAudioMgr.Type type)
		{
			m_dialogAudioMgr.StartLineAudio(type);
		}

		public void EndLineAudio()
		{
			m_dialogAudioMgr.EndLineAudio();
		}

		#endregion

		#region Member Functionalities

		public void QueueAudio(string clipID, bool loop = false)
		{
			AudioData data = GameDb.GetAudioData(clipID);
			m_audioQueue.Enqueue(new AudioLoopPair(data, loop));
		}

		private void PlayNextInQueue()
		{
			if (m_audioQueue.Count > 0)
			{
				AudioLoopPair pair = m_audioQueue.Dequeue();
				m_currData = pair.Data;
				LoadAudio(m_audioSrc, pair.Data);
				m_audioSrc.loop = pair.Loop;
				m_audioSrc.Play();
			}
		}

		private void ClearAudioQueue()
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

		#endregion
	}
}
