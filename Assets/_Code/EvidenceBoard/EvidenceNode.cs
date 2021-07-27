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

		private RectTransform m_rectTransform;

		private Routine m_colorRoutine;

		private static readonly Color DEFAULT = new Color(0.6039216f, 0.9058824f, 0.8980393f);
		private static readonly Color LINKED = new Color(0.8f, 1f, 1f);
		private static readonly Color SOLVED = new Color(1f, 0.77f, 0.23f);

		public void SetDefault() {
			m_colorRoutine.Replace(this, ColorTo(DEFAULT));
		}
		public void SetLinked() {
			m_colorRoutine.Replace(this, ColorTo(LINKED));
		}
		public void SetSolved() {
			m_colorRoutine.Replace(this, ColorTo(SOLVED));
		}
		private IEnumerator ColorTo(Color color) {
			yield return m_image.ColorTo(color, 0.1f).Ease(Curve.QuadOut);
		}
	}

}