using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// Manages sounds for dialogue (i.e. vocal underlay during phone call)
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class DialogAudioMgr : MonoBehaviour
	{
		#region Inspector

		[SerializeField]
		private AudioData m_phoneData;
		[SerializeField]
		private AudioData m_textData;
		[SerializeField]
		private AudioData m_radioData;

		private AudioSource m_audioSrc;

		private bool m_lineInProgress;

		#endregion

		public enum Type
		{
			phone,
			text,
			radio
		}

		public void Mute(bool isMute) {
			m_audioSrc.mute = isMute;
		}

		#region Unity Callbacks

		private void Awake()
		{
			m_audioSrc = this.GetComponent<AudioSource>();
			m_lineInProgress = false;
		}

		private void Update()
		{
			// todo: if sound ends but text is still being generated, loop back over

		}

		#endregion

		#region Communication Channels

		public void StartLineAudio(Type type)
		{
			AudioData dataToUse;

			switch (type)
			{
				case (Type.phone):
					dataToUse = m_phoneData;
					break;
				case (Type.text):
					dataToUse = m_textData;
					break;
				case (Type.radio):
					dataToUse = m_radioData;
					break;
				default:
					dataToUse = null;
					Debug.Log("Error: tried to start a line that was not phone, text, or radio");
					return;
			}

			AudioSrcMgr.LoadAudio(m_audioSrc, dataToUse);

			m_audioSrc.Play();
			m_lineInProgress = true;
		}

		public void EndLineAudio()
		{
			m_audioSrc.Stop();
			m_lineInProgress = false;
		}

		#endregion
	}
}
