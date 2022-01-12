using BeauRoutine;
using BeauUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
		private EvidenceChain[] m_chainObjects = null;

		[SerializeField]
		private Button m_buttonBack = null;
		[SerializeField]
		private Button m_buttonShipOut = null;

		[SerializeField]
		private Color m_colorDefault;
		[SerializeField]
		private Color m_colorHover;

		private StringHash32[] m_roots = new StringHash32[6] {
			"Location", "Type", "Name", "Cargo", "Cause", "Artifact"
		};


		private Dictionary<StringHash32, EvidenceGroup> m_groups;
		private Dictionary<StringHash32, EvidenceNode> m_nodes;
		private Dictionary<StringHash32, EvidenceChain> m_chains;
		private Dictionary<StringHash32, List<EvidencePin>> m_pinsByRoot;
		private Dictionary<EvidencePin, StringHash32> m_rootsByPin;

		//private GraphicRaycaster m_raycaster;
		private Layers m_layers;
		private Routine m_routineShipOut;

		private StringHash32 m_selectedRoot = StringHash32.Null;
		private int m_selectedPin = -1;
		private bool m_dragging;
		private Vector2 m_pressPosition;
		private EvidenceNode m_hoverNode;
		private bool m_wasLocationComplete = false;

		//private List<RaycastResult> m_raycastResults;

		private void Awake() {
			m_layers = new Layers(m_nodeBackGroup, m_lineGroup, m_nodeFrontGroup, m_labelGroup, m_pinGroup);
			m_groups = new Dictionary<StringHash32, EvidenceGroup>();
			m_nodes = new Dictionary<StringHash32, EvidenceNode>();
			m_chains = new Dictionary<StringHash32, EvidenceChain>();
			m_rootsByPin = new Dictionary<EvidencePin, StringHash32>();
			m_pinsByRoot = new Dictionary<StringHash32, List<EvidencePin>>();
		}

		protected override void OnShowStart() {
			base.OnShowStart();
			SetupBoard();
			m_routineShipOut.Stop();
			m_buttonShipOut.transform.localScale = Vector3.one;
			m_wasLocationComplete = GameMgr.State.CurrentLevel.IsLocationChainComplete();

            GameMgr.Events.Dispatch(GameEvents.BoardOpened);
			GameMgr.RunTrigger(GameTriggers.OnEnterEvidenceBoard);
		}

		protected override void OnHideCompleted() {
			base.OnHideCompleted();
			ClearBoard();
		}

		private void SetupBoard() {
			// get all of the unlocked evidence
			foreach (IEvidenceGroupState state in GameMgr.State.CurrentLevel.Evidence) {
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
			for (int chainIndex = 0; chainIndex < GameMgr.State.CurrentLevel.ChainCount; chainIndex++) {
				StringHash32 root = m_roots[chainIndex];
				IEvidenceChainState chain = GameMgr.State.CurrentLevel.GetChain(root);
				GameMgr.Events.Register<StringHash32>(GameEvents.ChainSolved, chain.HandleChainCorrect); // for @requires chains
				
				EvidenceChain obj = m_chainObjects[chainIndex];
				obj.Setup(m_layers);

				bool addDangler = chain.Depth < obj.PinCount && (chain.StickyInfo == null || (chain.StickyInfo.Response == StickyInfo.ResponseType.Hint && !chain.StickyInfo.NoDangler));
				obj.SetChainDepth(chain.Depth - 1 + (addDangler ? 1 : 0));

				// tracks which nodes have been completed.
				// if all nodes have been completed, set their color to gold
				// (workaround to bug in SetState raycasting)
				List<EvidenceNode> pinnedNodes = new List<EvidenceNode>();

				for (int pinIndex = 0; pinIndex < obj.PinCount; pinIndex++) {
					EvidencePin pin = obj.GetPin(pinIndex);
					if (!m_pinsByRoot.ContainsKey(root)) {
						m_pinsByRoot.Add(root, new List<EvidencePin>());
					}
					m_pinsByRoot[root].Add(pin);
					m_rootsByPin.Add(pin, root);
					EvidenceNode node;

					if (pinIndex == 0) { // this is a root pin
						pin.SetHomePosition(WorldToScreenPoint(pin.RectTransform.position));
						pin.SetPosition(WorldToScreenPoint(pin.RectTransform.position));
						pin.MarkAsRoot();
					}

					if (pinIndex + 1 < chain.Depth && m_nodes.TryGetValue(chain.GetNodeInChain(pinIndex + 1), out node)) {
						pin.SetPosition(WorldToScreenPoint(node.PinPosition));
						node.SetPinned(true);
						pinnedNodes.Add(node);
						if (pinIndex != 0) {
							pin.SetHomePosition(WorldToScreenPoint(node.SubPinPosition));
						}
					}
					else if (addDangler && pinIndex > 0 && pinIndex < chain.Depth && m_nodes.TryGetValue(chain.GetNodeInChain(pinIndex), out node)) {
						pin.SetPosition(WorldToScreenPoint(node.SubPinPosition));
						if (pinIndex != 0) {
							pin.SetHomePosition(WorldToScreenPoint(node.SubPinPosition));
						}
					}
					pin.OnPointerDown += HandlePinPressed;
					pin.OnPointerUp += HandlePinReleased;
				}
				chain.SetEvidenceChain(obj);
				m_chains.Add(chain.Root(), obj);
				
				if (chain.IsCorrect) {
					foreach (EvidenceNode node in pinnedNodes) {
						node.SetStatus(ChainStatus.Complete);
					}
				}

				RefreshChainState(chain.StickyInfo, obj, null);

				// if we are tutorializing location, relevant items need to pulse
				if (!GameMgr.State.HasTutorialPinDisplayed() && GameMgr.State.CurrentLevel.IsLocationRoot(chain.Root())) {
					obj.GetPin(0).SetPulsing(true);
				}
			}

			// ship out button is only available if the location root is solved
			m_buttonShipOut.gameObject.SetActive(GameMgr.State.CurrentLevel.IsLocationChainComplete());

			
		}

		private void ClearBoard() {
			foreach (EvidenceGroup group in m_groups.Values) {
				Destroy(group.gameObject);
			}
			foreach (EvidencePin pin in m_rootsByPin.Keys) {
				pin.OnPointerDown -= HandlePinPressed;
				pin.OnPointerUp -= HandlePinReleased;
			}
			for (int chainIndex = 0; chainIndex < GameMgr.State.CurrentLevel.ChainCount; chainIndex++) {
				IEvidenceChainState chain = GameMgr.State.CurrentLevel.GetChain(chainIndex);
				GameMgr.Events.Deregister<StringHash32>(GameEvents.ChainSolved, chain.HandleChainCorrect);
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
			GameMgr.Events.Register<StringHash32>(GameEvents.ChainSolved, HandleChainCorrect);
			GameMgr.Events.Register<StringHash32>(GameEvents.EvidenceUnlocked, HandleEvidenceUnlocked);
			GameMgr.Events.Register<StringHash32>(GameEvents.EvidenceRemoved, HandleEvidenceRemoved);
		}
		protected override void OnHideStart() {
			base.OnHideStart();
			m_buttonBack.onClick.RemoveListener(HandleBackButton);
			m_buttonShipOut.onClick.RemoveListener(HandleShipOutButton);
			GameMgr.Events.Deregister<StringHash32>(GameEvents.ChainSolved, HandleChainCorrect);
			GameMgr.Events.Deregister<StringHash32>(GameEvents.EvidenceUnlocked, HandleEvidenceUnlocked);
			GameMgr.Events.Deregister<StringHash32>(GameEvents.EvidenceRemoved, HandleEvidenceRemoved);
		}

		private EvidencePin Selected {
			get {
				if (m_selectedRoot == StringHash32.Null || m_selectedPin == -1) {
					return null;
				}
				else {
					return m_pinsByRoot[m_selectedRoot][m_selectedPin];
				}
			}
		}
		private void Update() {
			if (Selected != null) {
				Selected.SetPosition(InputMgr.Position);
				if (m_dragging) {
					EvidenceNode result;
					GraphicsRaycasterMgr.instance.RaycastForNode(InputMgr.Position, out result);
					if (result != m_hoverNode) {
						if (m_hoverNode != null && !m_hoverNode.IsPinned) {
							m_hoverNode.SetHover(false, m_colorHover);
						}
						m_hoverNode = result;
						if (m_hoverNode != null && !m_hoverNode.IsPinned) {
							m_hoverNode = result;
							m_hoverNode.SetHover(true, m_colorHover);
						}
					}
				} else {
					m_dragging = Vector2.Distance(m_pressPosition, InputMgr.Position) > 4f;
				}
			}
		}
		private void HandlePinPressed(EvidencePin pin) {
			if (Selected == pin) {
				Drop();
			}
			else if (Selected == null && !GameMgr.State.CurrentLevel.GetChain(m_rootsByPin[pin]).IsCorrect) {
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
			AudioSrcMgr.instance.PlayOneShot("click_evidence_back");
			UIMgr.CloseThenOpen<UIEvidenceScreen, UIOfficeScreen>();
		}
		private void HandleShipOutButton() {
			AudioSrcMgr.instance.PlayOneShot("click_evidence_ship_out");
			UIMgr.Open<UIEnRouteScreen>();
			/*
			UIMgr.Close<UIEvidenceScreen>();
			AudioSrcMgr.instance.PlayAudio("ship_out");
			AudioSrcMgr.instance.PlayAmbiance("ship_out_ambiance", true);
			SceneManager.LoadScene("ShipOut");
			*/
		}
		private void HandleChainCorrect(StringHash32 root) {
			if (GameMgr.State.CurrentLevel.IsLocationChainComplete() && !m_wasLocationComplete) {
				m_buttonShipOut.gameObject.SetActive(true);
				// this indicates that the button should also pulse
				m_routineShipOut.Replace(this, PulseShipOutButton());
				m_wasLocationComplete = true;
			}
		}
		private void HandleEvidenceUnlocked(StringHash32 evidence) {
			ClearBoard();
			SetupBoard();
		}
		private void HandleEvidenceRemoved(StringHash32 evidence) {
			ClearBoard();
			SetupBoard();
		}

		private IEnumerator PulseShipOutButton() {
			while (true) {
				yield return m_buttonShipOut.transform.ScaleTo(1.1f, 0.5f, Axis.XY).Ease(Curve.QuadIn);
				yield return m_buttonShipOut.transform.ScaleTo(1f, 0.5f, Axis.XY).Ease(Curve.QuadOut);
			}
		}

		private void Lift() {
			IEvidenceChainState chain = GameMgr.State.CurrentLevel.GetChain(m_selectedRoot);
			EvidenceChain obj = m_chains[m_selectedRoot];


			if (m_selectedPin < obj.Depth - 1 || (chain.StickyInfo != null && (chain.StickyInfo.NoDangler || chain.StickyInfo.Response != StickyInfo.ResponseType.Hint))) {
				// if you lift a non dangler pin, the sticky note hides
				obj.HideStickyNote();
			}
			GameMgr.State.CurrentLevel.GetChain(m_selectedRoot).Lift(m_selectedPin);
			
			obj.SetChainDepth(m_selectedPin + 1);
			obj.MoveToFront();
			AudioSrcMgr.instance.PlayOneShot("pick_up_pin");

			if (GraphicsRaycasterMgr.instance.RaycastForNode(InputMgr.Position, out EvidenceNode node)) {
				node.SetPinned(false);
			}

			// if we are tutorializing location, relevant items need to pulse
			if (!GameMgr.State.HasTutorialPinDisplayed() && GameMgr.State.CurrentLevel.IsLocationRoot(m_selectedRoot)) {
				m_chains[m_selectedRoot].GetPin(0).SetPulsing(false);
				// the location node needs to pulse
				m_nodes["location-coordinates"].SetPulsing(true); // gross
			}
		}

		private void Drop() {
			IEvidenceChainState chainState = GameMgr.State.CurrentLevel.GetChain(m_selectedRoot);
			EvidenceChain chainObj = m_chains[m_selectedRoot];
			bool alreadyPinned = false;

			if (m_hoverNode != null) {
				alreadyPinned = m_hoverNode.IsPinned;
				// check that evidence node does not already have a pin on it
				if (alreadyPinned) {
					SendSelectionHome();
				} else {
					Selected.SetPosition(WorldToScreenPoint(m_hoverNode.PinPosition));
					/*if (!Selected.IsRoot) {
						Selected.SetHomePosition(WorldToScreenPoint(m_hoverNode.SubPinPosition));
					}*/
					if (!GameMgr.State.HasTutorialPinDisplayed() && GameMgr.State.CurrentLevel.IsLocationRoot(m_selectedRoot)) {
						m_nodes["location-coordinates"].SetPulsing(false); // gross
					}
					chainState.Drop(m_hoverNode.NodeID);
					m_hoverNode.SetPinned(true);
				}
				m_hoverNode.SetHover(false, m_colorHover);
			} else {
				// if we didn't find a node, we need to return the pin home
				SendSelectionHome();
			}
			if (!alreadyPinned) {
				RefreshChainState(chainState.StickyInfo, chainObj, m_hoverNode);
				RefreshAllChains();
			}
			m_hoverNode = null;
			m_selectedPin = -1;
			m_dragging = false;
		}

		private void SendSelectionHome() {
			Selected.FlyHome();
			AudioSrcMgr.instance.PlayOneShot("evidence_miss");

			// if we are tutorializing location, relevant items need to pulse
			if (!GameMgr.State.HasTutorialPinDisplayed() && GameMgr.State.CurrentLevel.IsLocationRoot(m_selectedRoot)) {
				m_chains[m_selectedRoot].GetPin(0).SetPulsing(true);
				// the location node needs to pulse
				m_nodes["location-coordinates"].SetPulsing(false); // gross
			}
		}

		private void RefreshChainState(StickyInfo info, EvidenceChain chainObj, EvidenceNode node = null) {
			if (info == null) {
				chainObj.SetStatus(ChainStatus.Normal);
				if (node != null) {
					TryExtendingChain(chainObj, node);
				}
			}
			else {
				switch (info.Response) {
					case StickyInfo.ResponseType.Correct:
						chainObj.SetStatus(ChainStatus.Complete);
						break;
					case StickyInfo.ResponseType.Hint:
						chainObj.SetStatus(ChainStatus.Normal);
						if (node != null) {
							TryExtendingChain(chainObj, node, info);
						}
						break;
					case StickyInfo.ResponseType.Incorrect:
						chainObj.SetStatus(ChainStatus.Incorrect);
						break;
				}
			}

			if (string.IsNullOrEmpty(info?.Text ?? null)) {
				chainObj.HideStickyNote();
			} else {
				bool useDangler = !info.NoDangler;
				chainObj.ShowStickyNote(info.Text, useDangler);
			}
		}

		private void RefreshAllChains() {
			// NOTE:
			// this will only really work if there is only one
			// layer of dependency of chains

			int index = 0;
			foreach (EvidenceChain chainObj in m_chains.Values) {
				IEvidenceChainState state = GameMgr.State.CurrentLevel.GetChain(index++);
				state.ReevaluateStickyInfo();
				RefreshChainState(state.StickyInfo, chainObj);
			}
		}

		private void TryExtendingChain(EvidenceChain chainObj, EvidenceNode node, StickyInfo info = null) {
			bool useDangler = info == null || !info.NoDangler;
			if (node != null && m_selectedPin + 1 < chainObj.PinCount && useDangler) {
				chainObj.SetChainDepth(m_selectedPin + 2);
				EvidencePin newPin = chainObj.GetPin(m_selectedPin + 1);
				newPin.SetPosition(WorldToScreenPoint(node.SubPinPosition));
				newPin.SetHomePosition(WorldToScreenPoint(node.SubPinPosition));
			}
		}

		private static Vector2 WorldToScreenPoint(Vector3 worldPoint) {
			return RectTransformUtility.WorldToScreenPoint(Camera.main, worldPoint);
		}

		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.3f);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.3f);
		}

		protected override IEnumerator HideImmediateRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.0f);
		}
	}
}
