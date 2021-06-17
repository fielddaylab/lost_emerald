using BeauUtil;
using BeauUtil.Tags;
using Leaf.Defaults;
using System;
using System.Collections;
using UnityEngine;

namespace Shipwreck {

	public interface IMessagePanel {
		void Show();
		void Hide();
		void SetSpeaker(CharacterData speaker);
		void SetConversationPartner(CharacterData partner);
		void PrepareLine(string text); //todo: set to key
		IEnumerator TypeLine(uint visibleCharacterCount);
		IEnumerator CompleteLine();
	}


	public class UIDialogScreen : UIBase, ITextDisplayer {

		private enum Mode {
			TextMessage,
			Dialog
		}


		[SerializeField]
		private TextMessagePanel m_textMessagePanel = null;
		[SerializeField]
		private DialogMessagePanel m_dialogMessagePanel = null;

		private IMessagePanel m_current;


		private TagStringEventHandler m_tagEvents;


		public void Awake() {
			if (m_tagEvents != null) {
				return;
			}
			m_tagEvents = new TagStringEventHandler();
			m_tagEvents.Register(ScriptEvents.Dialog.Target, HandleSetTarget);
			m_tagEvents.Register(ScriptEvents.Dialog.Conversation, HandleSetConversation);
		}


		public TagStringEventHandler PrepareLine(TagString inString, TagStringEventHandler inBaseHandler) {
			if (inString.RichText.Length > 0) {
				m_current.PrepareLine(inString.RichText);
			}
			return m_tagEvents;
		}

		public IEnumerator TypeLine(TagString inString, TagTextData inType) {
			yield return m_current.TypeLine(inType.VisibleCharacterCount);
		}

		public IEnumerator CompleteLine() {
			yield return m_current.CompleteLine();
		}


		private void HandleSetTarget(TagEventData inEvent, object inContext) {
			SetTarget(GameDb.GetCharacterData(inEvent.StringArgument.ToString()));
		}
		private void HandleSetConversation(TagEventData inEvent, object inContext) {
			var args = ExtractArgs(inEvent.StringArgument);
			if (args.Length != 2) {
				throw new ArgumentException(string.Format("Recieved `{0}' " +
					"arguments for conversation when expecting 2", args.Length
				));
			}
			switch (args[0].ToString()) {
				case "text-message": SetTextMessageMode(); break;
				case "dialogue": SetDialogMessageMode(); break;
			}


			SetConversationPartner(GameDb.GetCharacterData(args[1]));
		}

		private void SetTextMessageMode() {
			if (m_current != null && m_current != m_textMessagePanel) {
				m_current.Hide();
			}
			m_current = m_textMessagePanel;
			m_current.Show();
		}
		private void SetDialogMessageMode() {
			if (m_current != null && m_current != m_dialogMessagePanel) {
				m_current.Hide();
			}
			m_current = m_dialogMessagePanel;
			m_current.Show();
		}

		private void SetTarget(CharacterData data) {
			m_current.SetSpeaker(data);
		}
		private void SetConversationPartner(CharacterData data) {
			m_current.SetConversationPartner(data);
		}
		private string[] ExtractArgs(StringSlice inString) {
			string[] split = inString.ToString().Split(',');
			for (int ix = 0; ix < split.Length; ix++) {
				split[ix] = split[ix].Trim();
			}
			return split;
		}

		protected override IEnumerator ShowRoutine() {
			yield return null;
		}

		protected override IEnumerator HideRoutine() {
			yield return null;
		}

	}

}