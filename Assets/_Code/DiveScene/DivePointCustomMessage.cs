using UnityEngine;
using BeauUtil;
using PotatoLocalization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Shipwreck {

	public class DivePointCustomMessage : MonoBehaviour {

		public SerializedHash32 CustomMessageKey {
			get { return m_customMessageKey; }
		}

		[SerializeField]
		private SerializedHash32 m_customMessageKey = "";
	}

}