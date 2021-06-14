using BeauUtil;
using BeauUtil.Tags;
using BeauUtil.Variants;
using Leaf;
using Leaf.Compiler;
using Leaf.Defaults;
using Leaf.Runtime;
using UnityEngine;

namespace Shipwreck {

	public sealed class GameMgr : Singleton<GameMgr> {
		private class Parser : LeafParser<LeafNode, LeafNodePackage<LeafNode>> {
			public override LeafNodePackage<LeafNode> CreatePackage(string inFileName) {
				return new LeafNodePackage<LeafNode>(inFileName);
			}
			protected override LeafNode CreateNode(string inFullId, StringSlice inExtraData, LeafNodePackage<LeafNode> inPackage) {
				return new LeafNode(inFullId, inPackage);
			}
		}


		[SerializeField]
		private LeafAsset m_testAsset = null;
		[SerializeField]
		private string m_testNodeName = string.Empty;
		[SerializeField]
		private DialogScreen m_dialogScreen = null;


		private DefaultLeafManager<LeafNode> m_scriptMgr;

		private LeafNodePackage<LeafNode> m_package;
		private Parser m_parser;


		protected override void OnAssigned() {
			m_scriptMgr = new DefaultLeafManager<LeafNode>(this, null, null);
			m_scriptMgr.ConfigureDisplay(m_dialogScreen, null);

			// configure tag parser
			CustomTagParserConfig parseConfig = new CustomTagParserConfig();
			parseConfig.AddEvent("@*", ScriptEvents.Dialog.Target).ProcessWith(ParseTargetArgs);
			parseConfig.AddEvent("conversation", ScriptEvents.Dialog.Conversation).WithStringData();

			m_scriptMgr.ConfigureTagStringHandling(parseConfig, new TagStringEventHandler());

			// parse/compile LeafTest asset
			m_parser = new Parser();

			m_package = LeafAsset.Compile(m_testAsset, m_parser);
			if (!m_package.TryGetNode(m_testNodeName, out LeafNode node)) {
				throw new System.Exception();
			}
			ShowDialogScreen(node);
			
		}


		public static void ShowDialogScreen(LeafNode node) {
			I.ShowDialogScreenInternal(node);
		}

		private void ShowDialogScreenInternal(LeafNode node) {
			m_scriptMgr.Run(node);
		}

		private static void ParseTargetArgs(TagData inTag, object inContext, ref TagEventData ioEvent) {
			ioEvent.Argument0 = inTag.Id.Substring(1).Hash32();
			if (inTag.Data.StartsWith('#')) {
				ioEvent.Argument1 = inTag.Data.Substring(1).Hash32();
			}
		}



	}

}