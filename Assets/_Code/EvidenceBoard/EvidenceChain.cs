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
		Unassigned,
		Normal,
		Incorrect,
		Complete
	}
	public class EvidenceChain : MonoBehaviour {

		public int PinCount {
			get { return m_evidencePins.Length; }
		}
		public ChainStatus Status {
			get { return m_status; }
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
		private ChainStatus m_status;

		public EvidencePin GetPin(int index) {
			if (index < 0 || index >= m_evidencePins.Length) {
				throw new IndexOutOfRangeException();
			}
			return m_evidencePins[index];
		}

		public void Setup(UIEvidenceScreen.Layers layers) {
			int num = 0;
			foreach (EvidencePin pin in m_evidencePins) {
				if (num++ > 0) {
					pin.gameObject.SetActive(false); // hack
				}	
				pin.RectTransform.SetParent(layers.Pin);
				Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, m_rootLabel.RectTransform.position + Vector3.down * 100f);
				pin.SetPosition(screenPoint);
				pin.SetHomePosition(screenPoint);
				pin.RectTransform.localScale = Vector3.one;
			}
			m_stickyNote.transform.SetParent(layers.Label);
			m_lineRenderer.transform.SetParent(layers.Line);
			m_rootPos = Vector2.zero;
			m_stickyNote.transform.localScale = Vector3.one;
			m_lineRenderer.transform.localScale = Vector3.one;

			SetChainDepth(1);
		}
		public void MoveToFront() {
			foreach (EvidencePin pin in m_evidencePins) {
				pin.RectTransform.SetAsLastSibling();
			}
			m_stickyNote.transform.SetAsLastSibling();
			m_lineRenderer.transform.SetAsLastSibling();
		}
		

		public void SetChainDepth(int depth) {
			m_points = new Vector2[depth + 1];
			m_points[0] = m_rootPos;
			m_lineRenderer.Points = m_points;
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

		public void SetStatus(ChainStatus status) {
			// play a sound if our state changed
			if (status != m_status) {
				if (m_status == ChainStatus.Unassigned) {
					// do not play a sound
					m_status = status;
				} else {
					m_status = status;
					switch (m_status) {
						case ChainStatus.Complete:
							AudioSrcMgr.instance.PlayOneShot("evidence_complete");
							break;
						case ChainStatus.Normal:
							AudioSrcMgr.instance.PlayOneShot("evidence_right");
							break;
						case ChainStatus.Incorrect:
							AudioSrcMgr.instance.PlayOneShot("evidence_wrong");
							break;
					}
				}
			}

			m_stickyNote.SetColor(GameDb.GetStickyColor(status));
			m_rootLabel.SetColor(GameDb.GetLineColor(status));
			foreach (EvidencePin pin in m_evidencePins) {
				if (pin.gameObject.activeSelf) {
					pin.SetColor(GameDb.GetPinColor(status));
					if (GraphicsRaycasterMgr.instance.RaycastForNode(pin.transform.position, out EvidenceNode nodeUnderPin)) {
						nodeUnderPin.SetStatus(status);
						//nodeUnderPin.SetPinned(status == ChainStatus.Complete);
					}
				}
			}
			m_lineColorRoutine.Replace(this, Tween.Color(m_lineRenderer.color, GameDb.GetLineColor(status), SetLineColor, 0.2f));
		}

		public void ShowStickyNote(string text) {
			m_stickyNote.SetText(text);
			m_stickyNote.gameObject.SetActive(true);
			for (int index = 0; index < m_evidencePins.Length; index++) {
				// place the note on the last active index
				if (!m_evidencePins[index].gameObject.activeInHierarchy) {
					EvidencePin pin = m_evidencePins[index - 1];
					Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, pin.RectTransform.position);
					screenPoint.y = Mathf.Max(100f * pin.GetComponentInParent<Canvas>().scaleFactor, screenPoint.y);
					RectTransformUtility.ScreenPointToLocalPointInRectangle(
						(RectTransform)m_stickyNote.RectTransform.parent,
						screenPoint,
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

		private void OnEnable() {
			foreach (EvidencePin pin in m_evidencePins) {
				pin.OnPositionSet += HandlePinPositionChanged;
			}
		}
		private void OnDisable() {
			foreach (EvidencePin pin in m_evidencePins) {
				pin.OnPositionSet -= HandlePinPositionChanged;
			}
		}


		private void OnDestroy() {
			Destroy(m_rootLabel.gameObject);
			Destroy(m_stickyNote.gameObject);
			Destroy(m_lineRenderer.gameObject);
			foreach (EvidencePin pin in m_evidencePins) {
				Destroy(pin.gameObject);
			}
		}

		private void HandlePinPositionChanged(EvidencePin pin) {
			int index = Array.IndexOf(m_evidencePins, pin);
			if (m_points != null && m_points.Length > index + 1) {
				m_points[index + 1] = PinToLinePoint(pin);
				m_lineRenderer.SetAllDirty();
			}
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