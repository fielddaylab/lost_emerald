using System.Collections;
using UnityEngine;

namespace Shipwreck {

	public sealed partial class EvidenceMgr : UIBase {

		[SerializeField]
		private Transform m_lineGroup = null;
		[SerializeField]
		private Transform m_nodeGroup = null;
		[SerializeField]
		private Transform m_pinGroup = null;


		protected override IEnumerator ShowRoutine() {
			throw new System.NotImplementedException();
		}

		protected override IEnumerator HideRoutine() {
			throw new System.NotImplementedException();
		}
	}
}