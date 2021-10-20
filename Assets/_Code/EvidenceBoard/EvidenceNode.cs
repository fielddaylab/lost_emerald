using BeauRoutine;
using BeauUtil;
using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {


	[RequireComponent(typeof(RectTransform))]
	public class EvidenceNode : MonoBehaviour {

		public StringHash32 NodeID { 
			get { return m_nodeId; } 
		}
		public LocalizationKey Label {
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
		public Vector2 PinPosition {
			get {
				if (m_pinPosition == null) {
					if (m_rectTransform == null) {
						m_rectTransform = GetComponent<RectTransform>();
					}
					return m_rectTransform.position;
				} else {
					return m_pinPosition.position;
				}
			}
		}
		public Vector2 SubPinPosition {
			get {
				if (m_subPinPosition == null) {
					if (m_rectTransform == null) {
						m_rectTransform = GetComponent<RectTransform>();
					}
					return m_rectTransform.position;
				} else {
					return m_subPinPosition.position;
				}
			}
		}
		public bool Pinned
		{
			get { return m_pinned; }
		}

		[SerializeField]
		private SerializedHash32 m_nodeId = string.Empty;
		[SerializeField]
		private LocalizationKey m_label = LocalizationKey.Empty;
		[SerializeField]
		private RectTransform m_pinPosition = null;
		[SerializeField]
		private RectTransform m_subPinPosition = null;
		[SerializeField]
		private Image m_image = null;
		[SerializeField]
		private bool m_pinned = false; // whether a pin is currently dropped on this node

		private RectTransform m_rectTransform;
		private Routine m_colorRoutine;
		private Routine m_pulseRoutine;
		private ChainStatus m_currStatus;
		private Color m_cachedColor;

		private void Awake() {
			if (m_image != null) {
				m_cachedColor = m_image.color;
			}
		}

		public void SetColor(Color color) {
			m_cachedColor = color;
			if (!m_pulseRoutine) {
				m_colorRoutine.Replace(this, ColorTo(color));
			}
		}

		private IEnumerator ColorTo(Color color) {
			yield return m_image.ColorTo(color, 0.1f).Ease(Curve.QuadOut);
		}

		public void SetCurrStatus(ChainStatus status) {
			m_currStatus = status;
		}

		public ChainStatus GetCurrStatus() {
			return m_currStatus;
		}

		public void SetPinned(bool pinned)
		{
			m_pinned = pinned;
		}

		public void SetPulsing(bool isPulsing) {
			if (isPulsing) {
				m_colorRoutine.Stop();
				m_pulseRoutine.Replace(this, PulseRoutine());
			} else {
				m_pulseRoutine.Stop();
				transform.localScale = Vector3.one;
			}
		}

		private IEnumerator PulseRoutine() {
			while (true) {
				yield return Routine.Combine(
					transform.ScaleTo(1.25f, 0.5f, Axis.XY),
					m_image.ColorTo(Color.cyan, 1.0f)
				);
				yield return Routine.Combine(
					transform.ScaleTo(1.0f, 0.5f, Axis.XY),
					m_image.ColorTo(m_cachedColor, 1.0f)
				);
			}
		}
	}

}