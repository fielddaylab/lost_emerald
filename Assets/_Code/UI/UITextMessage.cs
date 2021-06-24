using PotatoLocalization;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BeauRoutine;
using Leaf.Defaults;
using BeauUtil.Tags;

namespace Shipwreck {

	public class UITextMessage : UIDialogueBase {

		[SerializeField]
		private TweenSettings m_tweenSettings = new TweenSettings(0.35f, Curve.QuadInOut);

		[SerializeField]
		private TextMessage m_theirPrefab = null;
		[SerializeField]
		private TextMessage m_yourPrefab = null;
		[SerializeField]
		private RectTransform m_content = null;
		[SerializeField]
		private LocalizedTextUGUI m_conversationPartner = null;
		[SerializeField]
		private ScrollRect m_scrollRect = null;
		[SerializeField]
		private Button m_continueButton = null;

		private Sprite m_textingIcon = null;

		public void SetConversationPartner(CharacterData partner) {
			m_conversationPartner.Key = partner.DisplayName;
		}
		protected override void OnSetSpeaker(CharacterData speaker) {
			m_textingIcon = speaker.TextingIcon;
		}

		protected override void OnPrepareLine(TagString inString) {
			// do nothing
		}

		public override IEnumerator TypeLine(TagString inString, TagTextData inType) {
			TextMessage prefab = m_theirPrefab;
			if (m_textingIcon == null) {
				prefab = m_yourPrefab;
			}
			TextMessage obj = Instantiate(prefab, m_content);
			obj.Icon = m_textingIcon;
			obj.Text = CachedText;
			yield return m_scrollRect.verticalScrollbar.ValueTo(0f, 0.1f);
			yield return 0.15f;
		}

		public override IEnumerator CompleteLine() {
			bool isComplete = false;
			UnityAction handler = () => {
				isComplete = true;
			};
			m_continueButton.onClick.AddListener(handler);
			while (!isComplete) {
				yield return null;
			}
			m_continueButton.onClick.RemoveListener(handler);
		}

		

		protected override IEnumerator ShowRoutine() {
			yield return transform.MoveTo(45f, m_tweenSettings, Axis.Y);
		}
		protected override IEnumerator HideRoutine() {
			yield return transform.MoveTo(-660f, m_tweenSettings, Axis.Y);
		}
		protected override void OnHideCompleted() {
			base.OnHideCompleted();
			for (int ix = m_content.childCount - 1; ix >= 0; ix--) {
				Destroy(m_content.GetChild(ix).gameObject);
			}
		}
	}

}