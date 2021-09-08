using BeauUtil;
using UnityEngine;

namespace Shipwreck
{
	[CreateAssetMenu(fileName = "NewAudioData", menuName = "Shipwrecks/Audio/AudioData")]
	public class AudioData : ScriptableObject
	{
		public string ID
		{
			get { return m_id; }
		}
		public AudioClip Clip
		{
			get { return m_clip; }
		}

		[SerializeField]
		private string m_id;
		[SerializeField]
		private AudioClip m_clip;

	}

}