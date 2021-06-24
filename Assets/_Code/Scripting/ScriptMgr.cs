using BeauUtil;
using BeauUtil.Variants;
using Leaf.Defaults;
using Leaf.Runtime;
using UnityEngine;

namespace Shipwreck {

	public class ScriptMgr : DefaultLeafManager<ScriptNode> {

		private UITextMessage m_uiTextMessage;
		private UIDialogScreen m_uiDialog;
		private UIDialogueBase m_uiCurrent;


		public ScriptMgr(MonoBehaviour inHost, CustomVariantResolver inResolver, IMethodCache inCache = null) : base(inHost, inResolver, inCache) {
		}

		public void AssignUIReferences(UITextMessage textMessage, UIDialogScreen dialogue) {
			m_uiDialog = dialogue;
			m_uiTextMessage = textMessage;
		}

		public override void OnNodeEnter(ScriptNode inNode, LeafThreadState<ScriptNode> inThreadState) {
			if (inNode.Type == ScriptNode.NodeType.TextMessage) {
				m_uiCurrent = m_uiTextMessage;
				UIMgr.Open<UITextMessage>();
			} else if (inNode.Type == ScriptNode.NodeType.InPerson) {
				m_uiCurrent = m_uiDialog;
				UIMgr.Open<UIDialogScreen>();
			}
			ConfigureDisplay(m_uiCurrent, null);
		}
		public override void OnNodeExit(ScriptNode inNode, LeafThreadState<ScriptNode> inThreadState) {
			UIMgr.Close(m_uiCurrent);
		}

	}

}