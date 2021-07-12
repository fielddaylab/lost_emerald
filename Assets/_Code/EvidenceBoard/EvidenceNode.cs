using BeauUtil;
using UnityEngine;

namespace Shipwreck {


	[RequireComponent(typeof(RectTransform))]
	public class EvidenceNode : MonoBehaviour {

		public StringHash32 NodeID { 
			get { return m_nodeId; } 
		}
		[SerializeField]
		private SerializedHash32 m_nodeId;

		[SerializeField]
		private string m_label = string.Empty;


	}

}