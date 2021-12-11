﻿using BeauUtil;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Video;

namespace Shipwreck {

	public class CutscenePlayer : Singleton<CutscenePlayer> {

#if !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern bool IsSafari();
#endif

		public static event Action OnVideoComplete;

		[SerializeField]
		private VideoPlayer m_videoPlayer = null;
		[SerializeField]
		private Camera m_videoCamera = null;

#if UNITY_EDITOR
		private const string VIDEO_PATH = "file://{0}/cutscene{1:00}.mp4";
#else
		private const string VIDEO_PATH = "{0}/cutscene{1:00}.mp4";
#endif

		public static void Play() {
			AudioSrcMgr.instance.StashAudio();
			AudioSrcMgr.instance.StopAudio();
			AudioSrcMgr.instance.StopAmbiance();
#if !UNITY_EDITOR
			if (IsSafari()) {
				I.HandleVideoComplete(I.m_videoPlayer);
			} else {
#endif
				I.m_videoPlayer.url = string.Format(
					VIDEO_PATH,
					Application.streamingAssetsPath,
					GameMgr.State.CurrentLevel.Index + 1
				);
				I.m_videoPlayer.Play();
				I.m_videoCamera.gameObject.SetActive(true);
				GameCamera gameCamera = GameCamera.Find();
				if (gameCamera != null) {
					gameCamera.DisableAudio();
				}
#if !UNITY_EDITOR
			}
#endif			
		}

		private void OnEnable() {
			m_videoPlayer.loopPointReached += HandleVideoComplete;
		}
		private void OnDisable() {
			m_videoPlayer.loopPointReached -= HandleVideoComplete;
		}
		private void HandleVideoComplete(VideoPlayer player) {
			m_videoCamera.gameObject.SetActive(false);
			GameCamera gameCamera = GameCamera.Find();
			if (gameCamera != null) {
				gameCamera.EnableAudio();
			}
			OnVideoComplete?.Invoke();
		}
	}
}


