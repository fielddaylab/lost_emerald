using BeauUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Shipwreck {

	public class CutscenePlayer : Singleton<CutscenePlayer> {

		public static event Action OnVideoComplete;

		[SerializeField]
		private VideoPlayer m_videoPlayer = null;
		[SerializeField]
		private Camera m_videoCamera = null;
		[SerializeField]
		private Camera m_gameCamera;

#if UNITY_EDITOR
		private const string VIDEO_PATH = "file://{0}/cutscene{1:00}.mp4";
#else
		private const string VIDEO_PATH = "{0}/cutscene{1:00}.mp4";
#endif

		public static void Play() {
			I.m_videoPlayer.url = string.Format(VIDEO_PATH, Application.streamingAssetsPath, GameMgr.State.CurrentLevel.Index + 1);
			I.m_videoPlayer.Play();
			I.m_videoCamera.gameObject.SetActive(true);
			I.m_gameCamera.GetComponent<AudioListener>().enabled = false;
		}

		private void OnEnable() {
			m_videoPlayer.loopPointReached += HandleVideoComplete;
		}
		private void OnDisable() {
			m_videoPlayer.loopPointReached -= HandleVideoComplete;
		}
		private void HandleVideoComplete(VideoPlayer player) {
			m_videoCamera.gameObject.SetActive(false);
			I.m_gameCamera.GetComponent<AudioListener>().enabled = true;
			OnVideoComplete?.Invoke();
		}
	}
}


