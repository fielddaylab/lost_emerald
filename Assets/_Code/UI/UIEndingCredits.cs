using BeauRoutine;
using PotatoLocalization;
using System.Collections;
using UnityEngine;

namespace Shipwreck {

	public class UIEndingCredits : UITitleCredits {

		[SerializeField]
		private RectTransform m_newspaperGroup = null;
		[SerializeField]
		private LocalizedTextUGUI m_headlineText = null;
		[SerializeField]
		private LocalizedTextUGUI m_sublineText = null;

		[SerializeField]
		private LocalizationKey[] m_headlines = null;
		[SerializeField]
		private LocalizationKey[] m_sublines = null;


		private Routine m_routine;

		protected override void OnShowStart() {
			base.OnShowStart();
			m_newspaperGroup.localScale = Vector3.forward;
		}

		protected override void OnShowCompleted() {
			m_routine.Replace(SequenceRoutine());
		}

		private IEnumerator SequenceRoutine() {
			yield return Routine.Combine(
				CreditsRoutine(),
				NewspaperRoutine()
			);
			UIMgr.CloseThenOpen<UIEndingCredits, UITitleScreen>();
		}
		private IEnumerator NewspaperRoutine() {
			int index = -1;
			while (++index < m_headlines.Length) {
				m_headlineText.Key = m_headlines[index];
				m_sublineText.Key = m_sublines[index];
				m_newspaperGroup.localRotation = Quaternion.Euler(0f, 0f, 0f);
				yield return Routine.Combine(
					m_newspaperGroup.RotateTo(360f * 3f, 1f, Axis.Z, Space.Self, AngleMode.Absolute).Ease(Curve.QuintOut),
					m_newspaperGroup.ScaleTo(1f, 1f, Axis.XY).Ease(Curve.BackOut)
				);
				yield return 16f;
				yield return m_newspaperGroup.ScaleTo(0f, 1f, Axis.XY).Ease(Curve.BackIn);
			}
		}


	}
}