using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class UITitleCredits : UIBase {

		[SerializeField]
		private TextAsset m_creditsFile = null;
		[SerializeField]
		private TextMeshProUGUI m_slideHeading;
		[SerializeField]
		private TextMeshProUGUI m_slideBody;
		[SerializeField]
		private Color m_colorBody = Color.white;
		[SerializeField]
		private Color m_colorHeading = Color.white;
		[SerializeField]
		private Button m_backButton = null;

		private const string TAG_HEADING1 = "[H1]";
		private const string TAG_HEADING2 = "[H2]";
		private const string TAG_END_SLIDE = "[/slide]";

		private List<MainGroup> m_mainGroups = new List<MainGroup>();

		private Routine m_routine;

		private class MainGroup : IEnumerable<SubGroup> {
			public string Heading {
				get { return m_heading; }
			}
			private string m_heading;
			private List<SubGroup> m_groups;
			public MainGroup(string heading) {
				m_heading = heading;
				m_groups = new List<SubGroup>();
			}
			public void AddGroup(SubGroup group) {
				m_groups.Add(group);
			}
			public void AddName(string credit) {
				if (m_groups.Count <= 0) {
					m_groups.Add(new SubGroup(string.Empty));
				}
				m_groups[m_groups.Count - 1].AddName(credit);
			}

			public IEnumerator<SubGroup> GetEnumerator() {
				return m_groups.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return m_groups.GetEnumerator();
			}
		}
		private class SubGroup : IEnumerable<string> {
			public string Heading {
				get { return m_heading; }
			}
			private string m_heading;
			private List<string> m_names;
			public SubGroup(string heading) {
				m_heading = heading;
				m_names = new List<string>();
			}
			public void AddName(string name) {
				m_names.Add(name);
			}
			public IEnumerator<string> GetEnumerator() {
				return ((IEnumerable<string>)m_names).GetEnumerator();
			}
			IEnumerator IEnumerable.GetEnumerator() {
				return ((IEnumerable)m_names).GetEnumerator();
			}
		}
		private void ParseCredits() {
			m_mainGroups.Clear();
			string[] text = m_creditsFile.text.Split('\n');
			for (int ix = 0; ix < text.Length; ix++) {
				string trimmed = text[ix].Trim('\n', '\r','\t', ' ');
				if (trimmed.Length <= 0) {
					continue;
				}
				if (trimmed.StartsWith(TAG_END_SLIDE)) {
					m_mainGroups.Add(new MainGroup(m_mainGroups[m_mainGroups.Count - 1].Heading));
				} else if (trimmed.StartsWith(TAG_HEADING1)) {
					MainGroup group = new MainGroup(trimmed.Substring(TAG_HEADING1.Length, trimmed.Length - TAG_HEADING1.Length).Trim());
					m_mainGroups.Add(group);
				} else if (trimmed.StartsWith(TAG_HEADING2)) {
					SubGroup group = new SubGroup(trimmed.Substring(TAG_HEADING2.Length, trimmed.Length - TAG_HEADING2.Length).Trim());
					m_mainGroups[m_mainGroups.Count - 1].AddGroup(group);
				} else {
					m_mainGroups[m_mainGroups.Count - 1].AddName(text[ix].Trim());
				}
			}
		}

		protected override void OnShowStart() {
			m_slideHeading.color = new Color(0, 0, 0, 0);
			m_slideBody.color = new Color(0, 0, 0, 0);
			ParseCredits();
			if (m_backButton != null) {
				m_backButton.onClick.AddListener(HandleBackButton);
			}
			
		}
		protected override void OnShowCompleted() {
			m_routine.Replace(Routine.StartLoopRoutine(CreditsRoutine));
		}
		protected override void OnHideStart() {
			if (m_backButton != null) {
				m_backButton.onClick.RemoveListener(HandleBackButton);
			}
		}

		private void HandleBackButton() {
			UIMgr.CloseThenOpen<UITitleCredits, UITitleScreen>();
		}

		protected IEnumerator CreditsRoutine() {
			int index = 0;
			foreach (MainGroup group in m_mainGroups) {
				m_slideHeading.text = group.Heading;
				StringBuilder builder = new StringBuilder();
				foreach (SubGroup sub in group) {
					builder.Append("<b>").Append(sub.Heading).Append('\n').Append("</b>");
					foreach (string name in sub) {
						builder.Append(name).Append('\n');
					}
					builder.Append('\n');
				}
				m_slideBody.text = builder.ToString();
				yield return Routine.Combine(
					m_slideHeading.ColorTo(m_colorHeading, 1f, ColorUpdate.FullColor),
					m_slideBody.ColorTo(m_colorBody, 1f, ColorUpdate.FullColor)
				);
				yield return 6f;
				if (index + 1 < m_mainGroups.Count && m_mainGroups[index + 1].Heading == group.Heading) {
					yield return m_slideBody.ColorTo(new Color(0, 0, 0, 0), 1f, ColorUpdate.FullColor);
				} else {
					yield return Routine.Combine(
						m_slideHeading.ColorTo(new Color(0, 0, 0, 0), 1f, ColorUpdate.FullColor),
						m_slideBody.ColorTo(new Color(0, 0, 0, 0), 1f, ColorUpdate.FullColor)
					);
				}
				index++;
			}
		}


		protected override IEnumerator ShowRoutine() {
			yield return CanvasGroup.FadeTo(1f, 0.5f).Ease(Curve.QuadOut);
		}

		protected override IEnumerator HideRoutine() {
			yield return CanvasGroup.FadeTo(0f, 0.5f).Ease(Curve.QuadIn);
		}

		protected override IEnumerator HideImmediateRoutine() {
			throw new System.NotImplementedException();
		}
	}
}


