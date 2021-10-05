using BeauRoutine;
using BeauUtil;
using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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

		public void SetColor(Color color) {
			m_colorRoutine.Replace(this, ColorTo(color));
		}

		private IEnumerator ColorTo(Color color) {
			yield return m_image.ColorTo(color, 0.1f).Ease(Curve.QuadOut);
		}

		public void SetPinned(bool pinned)
		{
			m_pinned = pinned;
		}
	}

}