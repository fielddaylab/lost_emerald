using BeauUtil;
using UnityEngine;

namespace Shipwreck {


	[RequireComponent(typeof(RectTransform))]
	public class EvidenceNode : MonoBehaviour {

		public StringHash32 NodeID { 
			get { return m_nodeId; } 
		}
		public string Label {
			get { return m_label; }
		}
		public RectTransform RectTransform {
			get {
				if (m_rectTransform == null) {
					m_rectTransform = GetComponent<RectTransform>();
				}
				return m_rectTransform; 
			}
		}
		public RectTransform PinPosition {
			get { return m_pinPosition; }
		}

		[SerializeField]
		private SerializedHash32 m_nodeId = string.Empty;
		[SerializeField]
		private string m_label = string.Empty;
		[SerializeField]
		private RectTransform m_pinPosition = null;

		private RectTransform m_rectTransform;

	}

}