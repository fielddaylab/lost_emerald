using BeauRoutine;
using BeauUtil;
using BeauUtil.Tags;
using Leaf;
using Leaf.Compiler;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public sealed partial class GameMgr : Singleton<GameMgr> {

		private class Parser : LeafParser<ScriptNode, LeafNodePackage<ScriptNode>> {
			public override LeafNodePackage<ScriptNode> CreatePackage(string inFileName) {
				return new LeafNodePackage<ScriptNode>(inFileName);
			}
			protected override ScriptNode CreateNode(string inFullId, StringSlice inExtraData, LeafNodePackage<ScriptNode> inPackage) {
				return new ScriptNode(inFullId, inPackage);
			}
		}

		public static IGameState State {
			get { return I.m_state; }
		}

		[SerializeField]
		private UIDialogScreen m_dialogScreen = null;
		[SerializeField]
		private UITextMessage m_textMessageScreen = null;

		private ScriptMgr m_scriptMgr;
		private Dictionary<LeafAsset,LeafNodePackage<ScriptNode>> m_packages;
		private Parser m_parser;
		private GameState m_state;


		protected override void OnAssigned() {
			Routine.Settings.DebugMode = false;

			m_state = new GameState();
			m_packages = new Dictionary<LeafAsset, LeafNodePackage<ScriptNode>>();

			m_scriptMgr = new ScriptMgr(this, null, null);
			m_scriptMgr.AssignUIReferences(m_textMessageScreen, m_dialogScreen);

			// configure tag parser
			CustomTagParserConfig parseConfig = new CustomTagParserConfig();
			parseConfig.AddEvent("@*", ScriptEvents.Dialog.Target).ProcessWith(ParseTargetArgs);
			parseConfig.AddEvent("conversation", ScriptEvents.Dialog.Conversation).WithStringData();

			m_scriptMgr.ConfigureTagStringHandling(parseConfig, new TagStringEventHandler());
			m_parser = new Parser();
		}

		public static bool TryFindNode(LeafAsset asset, string name, out ScriptNode node) {
			LeafNodePackage<ScriptNode> package;
			if (I.m_packages.ContainsKey(asset)) {
				package = I.m_packages[asset];
			} else {
				package = LeafAsset.Compile(asset, I.m_parser);
				I.m_packages.Add(asset, package);
			}
			return package.TryGetNode(string.Concat(package.RootPath(),name), out node);
		}

		public static void RunScriptNode(ScriptNode node) {
			I.m_scriptMgr.Run(node, I.m_state.VariableTable);
		}


		private static void ParseTargetArgs(TagData inTag, object inContext, ref TagEventData ioEvent) {
			ioEvent.StringArgument = inTag.Id.Substring(1);
		}



	}

}