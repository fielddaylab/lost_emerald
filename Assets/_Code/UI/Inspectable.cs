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

		public string ID {
			get { return m_imageID; }
		}
		public Button Button {
			get { return m_button; }
		}
	}
}
