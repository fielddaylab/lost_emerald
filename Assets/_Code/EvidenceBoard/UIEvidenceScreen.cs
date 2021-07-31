using BeauRoutine;
using BeauUtil;
using PotatoLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {

	

	public sealed partial class UIEvidenceScreen : UIBase {

		public class Layers {
			public Transform NodeBack { get; }
			public Transform Line { get; }
			public Transform NodeFront { get; }
			public Transform Label { get; }
			public Transform Pin { get; }

			public Layers(Transform back, Transform line, Transform front, Transform label, Transform pin) {
				NodeBack = back;
				NodeFront = front;
				Line = line;
				Label = label;
				Pin = pin;
			}
		}

		[SerializeField]
		private Transform m_nodeBackGroup = null;
		[SerializeField]
		private Transform m_lineGroup = null;
		[SerializeField]
		private Transform m_nodeFrontGroup = null;
		[SerializeField]
		private Transform m_labelGroup = null;
		[SerializeField]
		private Transform m_pinGroup = null;

		[SerializeField]
		private EvidenceLabel m_labelPrefab = null;
		[SerializeField]
		private EvidenceChain m_chainPrefab = null;

		[SerializeField]
		private Button m_buttonBack = null;
		[SerializeField]
		private Button m_buttonShipOut = null;


		private Dictionary<StringHash32, EvidenceGroup> m_groups;
		private Dictionary<StringHash32, EvidenceNode> m_nodes;
		private Dictionary<StringHash32, EvidenceChain> m_chains;
		private Dictionary<StringHash32, List<EvidencePin>> m_pinsByRoot;
		private Dictionary<EvidencePin, StringHash32> m_rootsByPin;

		private GraphicRaycaster m_raycaster;
		private Layers m_layers;

		private StringHash32 m_selectedRoot = StringHash32.Null;
		private int m_selectedPin = -1;
		private bool m_dragging;
		private Vector2 m_pressPosition;
		private Vector2 m_offset;

		private void Awake() {
			m_layers = new Layers(m_nodeBackGroup, m_lineGroup, m_nodeFrontGroup, m_labelGroup, m_pinGroup);
			m_raycaster = GetComponentInParent<GraphicRaycaster>();
			m_groups = new Dictionary<StringHash32, EvidenceGroup>();
			m_nodes = new Dictionary<StringHash32, EvidenceNode>();
			m_chains = new Dictionary<StringHash32, EvidenceChain>();
			m_rootsByPin = new Dictionary<EvidencePin, StringHash32>();
			m_pinsByRoot = new Dictionary<StringHash32, List<EvidencePin>>();
		}

		protected override void OnShowStart() {
			base.OnShowStart();

			// get all of the unlocked evidence
			foreach (IEvidenceGroupState state in GameMgr.State.GetEvidence()) {
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
			int index = 0;
			foreach (IEvidenceChainState chain in GameMgr.State.GetChains()) {
				EvidenceNode root = m_nodes[chain.Root()];
				EvidenceChain obj = Instantiate(m_chainPrefab);
				obj.transform.SetParent(root.RectTransform);
				obj.transform.position = root.RectTransform.position;
				obj.Setup(GameDb.GetNodeLocalizationKey(root.NodeID), m_layers, 20f + 40f * (index++ % 2 == 0 ? 0 : 1f));
				foreach (EvidencePin pin in obj.Pins) {
					if (!m_pinsByRoot.ContainsKey(root.NodeID)) {
						m_pinsByRoot.Add(root.NodeID, new List<EvidencePin>());
					}
					m_pinsByRoot[root.NodeID].Add(pin);
					m_rootsByPin.Add(pin, root.NodeID);
					pin.OnPointerDown += HandlePinPressed;
					pin.OnPointerUp += HandlePinReleased;
				}
				m_chains.Add(chain.Root(), obj);
				obj.SetChainDepth(1); //hack?
			}
		}

		protected override void OnHideCompleted() {
			base.OnHideCompleted();
			foreach (EvidenceGroup group in m_groups.Values) {
				Destroy(group.gameObject);
			}
			foreach (EvidencePin pin in m_rootsByPin.Keys) {
				Destroy(pin.gameObject);
			}
			foreach (EvidenceChain chain in m_chains.Values) {
				Destroy(chain.gameObject);
			}
			m_groups.Clear();
			m_chains.Clear();
			m_pinsByRoot.Clear();
			m_rootsByPin.Clear();
			m_nodes.Clear();
		}

		protected override void OnShowCompleted() {
			base.OnShowCompleted();
			m_buttonBack.onClick.AddListener(HandleBackButton);
			m_buttonShipOut.onClick.AddListener(HandleShipOutButton);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonBack.onClick.RemoveListener(HandleBackButton);
			m_buttonShipOut.onClick.RemoveListener(HandleShipOutButton);
		}

		private EvidencePin Selected {
			get {
				if (m_selectedRoot == StringHash32.Null || m_selectedPin == -1) {
					return null;
				} else {
					return m_pinsByRoot[m_selectedRoot][m_selectedPin];
				}
			}
		}
		private void Update() {
			if (Selected != null) {
				Selected.SetPosition(InputMgr.Position);
				if (!m_dragging) {
					m_dragging = Vector2.Distance(m_pressPosition, InputMgr.Position) > 4f;
				}
			}
		}
		private void HandlePinPressed(EvidencePin pin) {
			if (Selected == pin) {
				Drop();
			} else if (Selected == null) {
				m_selectedRoot = m_rootsByPin[pin];
				m_selectedPin = m_pinsByRoot[m_selectedRoot].IndexOf(pin);
				m_pressPosition = InputMgr.Position;
				Selected.RectTransform.SetAsLastSibling();
				Lift();
			}
		}
		private void HandlePinReleased(EvidencePin pin) {
			if (Selected == pin && m_dragging) {
				Drop();
			}
		}
		private void HandleBackButton() {
			UIMgr.CloseThenOpen<UIEvidenceScreen,UIOfficeScreen>();
		}
		private void HandleShipOutButton() {
			UIMgr.Close<UIEvidenceScreen>();
			UIMgr.Close<UIPhoneNotif>();
			SceneManager.LoadScene("Dive_Ship01"); // hack
		}

		private void Lift() {
			GameMgr.State.GetChain(m_selectedRoot).Lift(m_selectedPin);
			m_chains[m_selectedRoot].SetChainDepth(m_selectedPin+1);
			m_chains[m_selectedRoot].MoveToFront();
		}

		private void Drop() {
			List<RaycastResult> results = new List<RaycastResult>();
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			eventData.position = InputMgr.Position;
			m_raycaster.Raycast(eventData, results);
			foreach (RaycastResult result in results) {
				EvidenceNode node = result.gameObject.GetComponent<EvidenceNode>();
				if (node != null) {
					Selected.SetPosition(RectTransformUtility.WorldToScreenPoint(Camera.main,node.PinPosition));
					GameMgr.State.GetChain(m_selectedRoot).Drop(node.NodeID);
					break;
				}
			}
			// determine what we do with the chain
			EvidenceChain chain = m_chains[m_selectedRoot];
			PostItData data = GameMgr.EvaluateChain(m_selectedRoot);
			if (data == null) {
				chain.SetState(ChainStatus.Normal);
			} else {
				switch (data.Response) {
					case PostItData.ResponseType.Correct:
						chain.SetState(ChainStatus.Complete);
						break;
					case PostItData.ResponseType.Hint:
						chain.SetState(ChainStatus.Normal);
						break;
					case PostItData.ResponseType.Incorrect:
						chain.SetState(ChainStatus.Incorrect);
						break;
				}
			}
			m_selectedPin = -1;
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