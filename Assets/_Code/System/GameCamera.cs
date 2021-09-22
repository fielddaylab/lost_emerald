using UnityEngine;

namespace Shipwreck {

	public class GameCamera : MonoBehaviour {

		[SerializeField]
		private AudioListener m_audioListener = null;


		public static GameCamera Find() {
			return GameObject.FindObjectOfType<GameCamera>();
		}

		public void EnableAudio() {
			m_audioListener.enabled = true;
		}
		public void DisableAudio() {
			m_audioListener.enabled = false;
		}

	}

}