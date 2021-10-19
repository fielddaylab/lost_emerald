﻿using BeauUtil;
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

#if UNITY_EDITOR
		private const string VIDEO_PATH = "file://{0}/cutscene{1:00}.mp4";
		private const string VIDEO_PATH_ALT = "file://{0}/cutscene{1:00}.mov"; // temporary hack until movie formats are consistent
#else
		private const string VIDEO_PATH = "{0}/cutscene{1:00}.mp4";
		private const string VIDEO_PATH_ALT = "{0}/cutscene{1:00}.mov"; // temporary hack until movie formats are consistent
#endif

		public static void Play() {
			AudioSrcMgr.instance.StopAudio();
			// temporary hack until movie formats are consistent
			if (GameMgr.State.CurrentLevel.Index == 0) {
				I.m_videoPlayer.url = string.Format(
					VIDEO_PATH_ALT,
					Application.streamingAssetsPath,
					GameMgr.State.CurrentLevel.Index + 1
					);
			}
			else {
				I.m_videoPlayer.url = string.Format(
					VIDEO_PATH,
					Application.streamingAssetsPath,
					GameMgr.State.CurrentLevel.Index + 1
					);
			}
			I.m_videoPlayer.Play();
			I.m_videoCamera.gameObject.SetActive(true);
			GameCamera gameCamera = GameCamera.Find();
			if (gameCamera != null) {
				gameCamera.DisableAudio();
			}
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


