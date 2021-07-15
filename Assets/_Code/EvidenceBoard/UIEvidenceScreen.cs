using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
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

		private List<EvidenceGroup> m_evidence;


		protected override void OnShowStart() {
			base.OnShowStart();

			if (m_evidence == null) {
				m_evidence = new List<EvidenceGroup>();
			}

			// get all of the unlocked evidence
			foreach (IEvidenceGroupState state in GameMgr.State.GetEvidence(0)) {
				EvidenceGroup obj = Instantiate(GameDb.GetEvidenceGroup(state.Identity));
				m_evidence.Add(obj);
				obj.RectTransform.SetParent(m_nodeBackGroup);
				obj.RectTransform.localScale = Vector3.one;
				obj.RectTransform.anchoredPosition = state.Position;
			}
		}


		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
	}
}