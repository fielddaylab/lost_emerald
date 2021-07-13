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
		public bool IsRoot {
			get { return m_isRoot; }
		}

		[SerializeField]
		private SerializedHash32 m_nodeId = string.Empty;
		[SerializeField]
		private string m_label = string.Empty;
		[SerializeField, Tooltip("Root nodes will start with a string and pin and cannot have pins dropped on them.")]
		private bool m_isRoot;

	}

}