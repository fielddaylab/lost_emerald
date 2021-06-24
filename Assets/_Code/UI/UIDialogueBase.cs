using BeauUtil.Tags;
using Leaf.Defaults;
using System.Collections;

namespace Shipwreck {


	public abstract class UIDialogueBase : UIBase, ITextDisplayer {


		private TagStringEventHandler m_tagEvents;

		protected string CachedText { get; private set; }

		private void Awake() {
			if (m_tagEvents != null) {
				return;
			}
			m_tagEvents = new TagStringEventHandler();
			m_tagEvents.Register(ScriptEvents.Dialog.Target, HandleSetTarget);
		}
		private void HandleSetTarget(TagEventData inEvent, object inContext) {
			OnSetSpeaker(GameDb.GetCharacterData(inEvent.StringArgument.ToString()));
		}

		public TagStringEventHandler PrepareLine(TagString inString, TagStringEventHandler inBaseHandler) {
			CachedText = inString.RichText;
			return m_tagEvents;
		}

		public abstract IEnumerator TypeLine(TagString inString, TagTextData inType);
		public abstract IEnumerator CompleteLine();

		protected abstract void OnPrepareLine(TagString inString);

		protected abstract void OnSetSpeaker(CharacterData speaker);
	}

}