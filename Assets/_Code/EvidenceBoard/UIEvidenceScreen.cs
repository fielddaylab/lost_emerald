using BeauRoutine;
using BeauUtil;
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

		[SerializeField]
		private EvidenceLine m_linePrefab = null;
		[SerializeField]
		private EvidencePin m_pinPrefab = null;


		private Dictionary<StringHash32, EvidenceGroup> m_groups;
		private Dictionary<StringHash32, EvidenceNode> m_nodes;

		protected override void OnShowStart() {
			base.OnShowStart();

			if (m_groups == null) {
				m_groups = new Dictionary<StringHash32, EvidenceGroup>();
			}
			if (m_nodes == null) {
				m_nodes = new Dictionary<StringHash32, EvidenceNode>();
			}

			// get all of the unlocked evidence
			foreach (IEvidenceGroupState state in GameMgr.State.GetEvidence(0)) {
				EvidenceGroup obj = Instantiate(GameDb.GetEvidenceGroup(state.Identity));
				m_groups.Add(state.Identity, obj);
				obj.RectTransform.SetParent(m_nodeBackGroup);
				obj.RectTransform.localScale = Vector3.one;
				obj.RectTransform.anchoredPosition = state.Position;
				foreach (EvidenceNode node in obj.Nodes) {
					m_nodes.Add(node.NodeID, node);
				}
			}
			// get the state of all chains
			foreach (IEvidenceChainState chain in GameMgr.State.GetChains(0)) {
				
				StringHash32 current, next;
				current = chain.Root();
				next = chain.Next(chain.Root());
				EvidenceLine line;
				while (next != StringHash32.Null) {
					line = Instantiate(m_linePrefab, m_lineGroup);
					line.transform.localPosition = Vector3.zero;
					line.Setup(m_nodes[current].RectTransform, m_nodes[next].RectTransform);
					current = next;
					next = chain.Next(current);
				}
				line = Instantiate(m_linePrefab, m_lineGroup);
				line.transform.localPosition = Vector3.zero;
				EvidencePin pin = Instantiate(m_pinPrefab, m_pinGroup);
				pin.transform.position = m_nodes[current].PinPosition.position;
				line.Setup(m_nodes[current].RectTransform, (RectTransform)pin.transform);
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