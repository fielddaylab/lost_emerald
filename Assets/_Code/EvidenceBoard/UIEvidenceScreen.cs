using BeauRoutine;
using BeauUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
		private List<EvidencePin> m_pins;
		private GraphicRaycaster m_raycaster;

		protected override void OnShowStart() {
			base.OnShowStart();

			m_raycaster = GetComponentInParent<GraphicRaycaster>();

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
					if (m_nodes.ContainsKey(node.NodeID)) {
						throw new System.Exception(node.NodeID.ToDebugString());
					}
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
				pin.OnPointerDown += HandlePinPointerDown;
				pin.OnPointerUp += HandlePinPointerUp;
				//pin.SetLayerParent(m_pinGroup, raycaster);
				pin.transform.position = m_nodes[current].PinPosition;
				line.Setup(m_nodes[current].RectTransform, (RectTransform)pin.transform);
			}
			
		}

		private EvidencePin m_selectedPin;
		private bool m_dragging = false;
		private Vector2 m_pressPosition;
		private Vector2 m_offset;

		private void Update() {
			if (m_selectedPin != null) {
				RectTransformUtility.ScreenPointToLocalPointInRectangle(
					(RectTransform)m_selectedPin.RectTransform.parent, InputMgr.Position, Camera.main, out Vector2 point
				);
				m_selectedPin.RectTransform.localPosition = point - m_offset;
				if (!m_dragging) {
					m_dragging = Vector2.Distance(m_pressPosition, InputMgr.Position) > 4f;
				}
			}
		}

		private void HandlePinPointerDown(EvidencePin pin) {
			if (m_selectedPin == pin) {
				DropPin();
			} else if (m_selectedPin == null) {
				m_selectedPin = pin;
				m_pressPosition = InputMgr.Position;
				m_selectedPin.RectTransform.SetAsLastSibling();
				RectTransformUtility.ScreenPointToLocalPointInRectangle(
					m_selectedPin.RectTransform, InputMgr.Position, Camera.main, out m_offset
				);
				if (!m_dragging) {
					m_dragging = Vector2.Distance(m_pressPosition, InputMgr.Position) > 4f;
				}
			}
		}
		private void HandlePinPointerUp(EvidencePin pin) {
			if (m_selectedPin == pin && m_dragging) {
				DropPin();
			}
		}

		private void DropPin() {
			List<RaycastResult> results = new List<RaycastResult>();
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			eventData.position = InputMgr.Position;
			m_raycaster.Raycast(eventData, results);
			bool foundNode = false;
			foreach (RaycastResult result in results) {
				EvidenceNode node = result.gameObject.GetComponent<EvidenceNode>();
				if (node != null) {
					if (m_selectedPin.Link != null) {
						m_selectedPin.Link.SetDefault();
					}
					m_selectedPin.SetLink(node);
					node.SetLinked();
					m_selectedPin.RectTransform.SetParent(node.transform);
					m_selectedPin.RectTransform.position = node.PinPosition;
					foundNode = true;
					break;
				}
			}
			if (!foundNode) {
				if (m_selectedPin.Link != null) {
					m_selectedPin.Link.SetDefault();
				}
				m_selectedPin.SetLink(null);
				m_selectedPin.RectTransform.SetParent(m_pinGroup);
			}

			m_selectedPin = null;
			m_dragging = false;
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}
	}
}