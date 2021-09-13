using BeauRoutine;
using BeauUtil;
using PotatoLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace Shipwreck {

	public enum ChainStatus {
		Normal,
		Incorrect,
		Complete
	}
	public class EvidenceChain : MonoBehaviour {

		public int PinCount {
			get { return m_evidencePins.Length; }
		}

		[SerializeField]
		private EvidenceLabel m_rootLabel = null;
		[SerializeField]
		private UILineRenderer m_lineRenderer = null;
		[SerializeField]
		private StickyNote m_stickyNote = null;
		[SerializeField]
		private EvidencePin[] m_evidencePins = null;

		private Vector2 m_rootPos;
		private Routine m_lineColorRoutine;
		private Vector2[] m_points;
		private float m_labelDistance;


		public EvidencePin GetPin(int index) {
			if (index < 0 || index >= m_evidencePins.Length) {
				throw new IndexOutOfRangeException();
			}
			return m_evidencePins[index];
		}

		public void Setup(LocalizationKey label, UIEvidenceScreen.Layers layers, float labelDistance) {
			m_rootLabel.Key = label;
			int num = 0;
			foreach (EvidencePin pin in m_evidencePins) {
				if (num++ > 0) {
					pin.gameObject.SetActive(false); // hack
				}	
				pin.RectTransform.SetParent(layers.Pin);
				pin.RectTransform.localPosition += Vector3.down * 100f;
				pin.RectTransform.localScale = Vector3.one;
				pin.OnPositionSet += HandlePinPositionChanged;
			}
			m_stickyNote.transform.SetParent(layers.Label);
			m_lineRenderer.transform.SetParent(layers.Line);
			m_rootLabel.transform.SetParent(layers.Label);
			m_rootPos = Vector2.zero;
			m_labelDistance = labelDistance;
			m_stickyNote.transform.localScale = Vector3.one;
			m_lineRenderer.transform.localScale = Vector3.one;
			m_rootLabel.transform.localScale = Vector3.one;

			SetChainDepth(1);
		}
		public void MoveToFront() {
			foreach (EvidencePin pin in m_evidencePins) {
				pin.RectTransform.SetAsLastSibling();
			}
			m_stickyNote.transform.SetAsLastSibling();
			m_lineRenderer.transform.SetAsLastSibling();
			m_rootLabel.transform.SetAsLastSibling();
		}

		public void SetChainDepth(int depth) {
			m_points = new Vector2[depth + 1];
			m_points[0] = m_rootPos;
			m_lineRenderer.Points = m_points;
			SetLabelDistance();
			int index = 0;
			while (index < depth) {
				m_evidencePins[index].gameObject.SetActive(true);
				m_points[index+1] = PinToLinePoint(m_evidencePins[index++]);
			}
			while (index < m_evidencePins.Length) {
				m_evidencePins[index++].gameObject.SetActive(false);
			}
			m_lineRenderer.SetAllDirty();
		}

		public void SetState(ChainStatus state) {
			m_stickyNote.SetColor(GameDb.GetStickyColor(state));
			m_rootLabel.SetColor(GameDb.GetLineColor(state));
			foreach (EvidencePin pin in m_evidencePins) {
				pin.SetColor(GameDb.GetPinColor(state));
			}
			m_lineColorRoutine.Replace(this, Tween.Color(m_lineRenderer.color, GameDb.GetLineColor(state), SetLineColor, 0.2f));
		}

		public void ShowStickyNote(string text) {
			m_stickyNote.SetText(text);
			m_stickyNote.gameObject.SetActive(true);
			for (int index = 0; index < m_evidencePins.Length; index++) {
				// place the note on the last active index
				if (!m_evidencePins[index].gameObject.activeInHierarchy) {
					RectTransformUtility.ScreenPointToLocalPointInRectangle(
						(RectTransform)m_stickyNote.RectTransform.parent,
						RectTransformUtility.WorldToScreenPoint(Camera.main, m_evidencePins[index-1].RectTransform.position),
						Camera.main, out Vector2 point
					);
					m_stickyNote.RectTransform.localPosition = point;
					break;
				}
			}
		}
		public void HideStickyNote() {
			m_stickyNote.gameObject.SetActive(false);
		}


		private void OnDestroy() {
			Destroy(m_rootLabel.gameObject);
			Destroy(m_stickyNote.gameObject);
			Destroy(m_lineRenderer.gameObject);
			foreach (EvidencePin pin in m_evidencePins) {
				Destroy(pin.gameObject);
			}
		}

		private void SetLabelDistance() {
			Vector2 segement1 =  m_points[1] - m_rootPos;
			Vector2 basePos = m_lineRenderer.rectTransform.position;
			((RectTransform)m_rootLabel.transform).position =  basePos + (segement1.normalized * Mathf.Min(m_labelDistance, segement1.magnitude* 0.75f));
		}

		private void HandlePinPositionChanged(EvidencePin pin) {
			int index = Array.IndexOf(m_evidencePins, pin);
			m_points[index + 1] = PinToLinePoint(pin);
			m_lineRenderer.SetAllDirty();
			SetLabelDistance();
		}

		private void SetLineColor(Color color) {
			m_lineRenderer.color = color;
		}

		private Vector2 PinToLinePoint(EvidencePin pin) {
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				m_lineRenderer.rectTransform, 
				RectTransformUtility.WorldToScreenPoint(Camera.main, pin.transform.position), 
				Camera.main, out Vector2 point
			);
			return point;
		}

	}

}