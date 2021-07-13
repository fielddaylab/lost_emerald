using BeauRoutine;
using System.Collections;
using UnityEngine;

namespace Shipwreck {

	public sealed partial class UIEvidenceScreen : UIBase {

		[SerializeField]
		private Transform m_nodeBackGroup = null;
		[SerializeField]
		private Transform m_lineGroup = null;
		[SerializeField]
		private Transform m_nodeFrontGroup = null;
		[SerializeField]
		private Transform m_pinGroup = null;


		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
	}
}