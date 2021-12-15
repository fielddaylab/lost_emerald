using BeauUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {
	public class Inspectable : MonoBehaviour {
		[SerializeField]
		private string m_imageID;
		[SerializeField]
		private Button m_button;

		private Sprite m_sprite; // used when an inspectable's id is not known ahead of time
							   // i.e. during text messages

		public string ID {
			get { return m_imageID; }
		}
		public Button Button {
			get { return m_button; }
		}
		public Sprite Sprite {
			get { return m_sprite; }
		}

		public void SetSprite(Sprite sprite) {
			m_sprite = sprite;
		}
	}
}
