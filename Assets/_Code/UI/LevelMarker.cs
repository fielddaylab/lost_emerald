using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {
	public class LevelMarker : MonoBehaviour {

		[SerializeField]
		private Button m_button;

		public Button Button {
			get { return m_button; }
		}
	}
}

