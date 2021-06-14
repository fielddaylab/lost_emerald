﻿using BeauUtil;
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


	public class DialogScreen : MonoBehaviour, ITextDisplayer {

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
		private StringUtils.ArgsList.Splitter m_argsListSplitter;


		public void RegisterEvents() {
			if (m_tagEvents != null) {
				return;
			}
			m_tagEvents = new TagStringEventHandler();
			m_tagEvents.Register(ScriptEvents.Dialog.Target, HandleSetTarget);
			m_tagEvents.Register(ScriptEvents.Dialog.Conversation, HandleSetConversation);
		}

		private TempList8<StringSlice> ExtractArgs(StringSlice inString) {
			TempList8<StringSlice> args = new TempList8<StringSlice>();
			inString.Split(m_argsListSplitter, StringSplitOptions.None, ref args);
			return args;
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
			if (args.Count != 2) {
				throw new ArgumentException(string.Format("Recieved `{0}' " +
					"arguments for conversation when expecting 2", args.Count
				));
			}
			switch (args[1].ToString()) {
				case "text-message": SetTextMessageMode(); break;
				case "dialogue": SetDialogMessageMode(); break;
			}
			SetConversationPartner(GameDb.GetCharacterData(args[0].ToString()));
		}

		private void SetTextMessageMode() {
			if (m_current != m_textMessagePanel) {
				m_current.Hide();
			}
			m_current = m_textMessagePanel;
			m_current.Show();
		}
		private void SetDialogMessageMode() {
			if (m_current != m_dialogMessagePanel) {
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

	}

}