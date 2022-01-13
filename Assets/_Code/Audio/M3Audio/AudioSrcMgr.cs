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
			source.mute = instance.m_audioSrc.mute;
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

		public void MuteAudio(bool isMute) {
			m_audioSrc.mute = isMute;
			m_dialogAudioMgr.Mute(isMute);
			m_ambianceMgr.MuteAudio(isMute);
		}
		public bool IsMute() {
			return m_audioSrc.mute;
		}

		public void CrossFadeAudio(string clipID, float time, bool loop = false) {
			StartCoroutine(CrossFadeRoutine(clipID, time, loop));
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

		public void CrossFadeAmbiance(string clipID, float time, bool loop = false) {
			m_ambianceMgr.CrossFadeAudio(clipID, time, loop);
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
		public void PlayOneShot(string clipID) {
			AudioClip clip = GameDb.GetAudioData(clipID).Clip;
			m_audioSrc.PlayOneShot(clip);
		}

		/// <summary>
		/// CrossFadeAudio + OneShot clip
		/// </summary>
		/// <param name="clipID"></param>
		/// <param name="time"></param>
		/// <param name="oneShotClipID"></param>
		/// <param name="loop"></param>
		public void CrossFadeAudio(string clipID, float time, string oneShotClipID, bool loop = false) {
			StartCoroutine(CrossFadeRoutineWithOneShot(clipID, time, oneShotClipID, loop));
		}

		#endregion

		#region Helper Methods

		private IEnumerator CrossFadeRoutine(string clipID, float time, bool loop) {
			float maxVolume = m_audioSrc.volume;
			float midTime = time / 2f;
			bool transitioned = false;

			float volumeStep = maxVolume / midTime;

			for (float t = 0; t < time; t += Time.deltaTime) {
				if (t >= midTime && !transitioned) {
					// load new audio
					m_audioSrc.Stop();
					AudioData newData = GameDb.GetAudioData(clipID);
					m_currData = newData;
					LoadAudio(m_audioSrc, newData);
					m_audioSrc.volume = volumeStep * Time.deltaTime;
					m_audioSrc.loop = loop;
					m_audioSrc.Play();
					transitioned = true;
				}
				if (!transitioned) {
					m_audioSrc.volume -= volumeStep * Time.deltaTime;
				}
				else {
					m_audioSrc.volume += volumeStep * Time.deltaTime;
				}

				yield return null;
			}

			m_audioSrc.volume = maxVolume;
		}

		private IEnumerator CrossFadeRoutineWithOneShot(string clipID, float time, string oneShotClipID, bool loop) {
			float maxVolume = m_audioSrc.volume;
			float midTime = time / 2f;
			bool transitioned = false;

			float volumeStep = maxVolume / midTime;

			for (float t = 0; t < time; t += Time.deltaTime) {
				if (t >= midTime && !transitioned) {
					// load new audio
					m_audioSrc.Stop();
					AudioData newData = GameDb.GetAudioData(clipID);
					m_currData = newData;
					LoadAudio(m_audioSrc, newData);
					m_audioSrc.volume = volumeStep * Time.deltaTime;
					m_audioSrc.loop = loop;
					m_audioSrc.Play();
					m_audioSrc.PlayOneShot(GameDb.GetAudioData(oneShotClipID).Clip);
					transitioned = true;
				}
				if (!transitioned) {
					m_audioSrc.volume -= volumeStep * Time.deltaTime;
				}
				else {
					m_audioSrc.volume += volumeStep * Time.deltaTime;
				}

				yield return null;
			}

			m_audioSrc.volume = maxVolume;
		}

		#endregion
	}
}
