using UnityEngine;
using BeauUtil;
using PotatoLocalization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Shipwreck {

	public class DiveObservationPoint : MonoBehaviour {

		public SerializedHash32 ObservationKey {
			get { return m_observationKey; }
		}

		[SerializeField]
		private SerializedHash32 m_observationKey = "";
	}

}