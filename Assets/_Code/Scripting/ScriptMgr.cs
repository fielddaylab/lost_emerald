using System;
using System.Collections.Generic;
using BeauPools;
using BeauUtil;
using BeauUtil.Tags;
using BeauUtil.Variants;
using Leaf;
using Leaf.Compiler;
using Leaf.Defaults;
using Leaf.Runtime;
using UnityEngine;
using BeauRoutine;

namespace Shipwreck {

	public class ScriptMgr : DefaultLeafManager<ScriptNode> {

		private class Parser : LeafParser<ScriptNode, LeafNodePackage<ScriptNode>> {
			public override LeafNodePackage<ScriptNode> CreatePackage(string inFileName) {
				return new LeafNodePackage<ScriptNode>(inFileName);
			}
			protected override ScriptNode CreateNode(string inFullId, StringSlice inExtraData, LeafNodePackage<ScriptNode> inPackage) {
				return new ScriptNode(inFullId, inPackage);
			}
		}

		private enum TriggerType {
			Function,
			Response
		}

		static private TriggerType NodeTypeToTriggerType(ScriptNode.NodeType inNodeType) {
			switch(inNodeType) {
				case ScriptNode.NodeType.Function:
					return TriggerType.Function;
				default:
					return TriggerType.Response;
			}
		}

		private struct TriggerKey : IEquatable<TriggerKey> {
			public readonly StringHash32 TriggerId;
			public readonly TriggerType TriggerType;

			public TriggerKey(StringHash32 triggerId, TriggerType triggerType) {
				TriggerId = triggerId;
				TriggerType = triggerType;
			}

			public TriggerKey(StringHash32 triggerId, ScriptNode.NodeType nodeType) {
				TriggerId = triggerId;
				TriggerType = NodeTypeToTriggerType(nodeType);
			}

			public bool Equals(TriggerKey other) {
				return TriggerId == other.TriggerId && TriggerType == other.TriggerType;
			}

			public override int GetHashCode() {
				return (TriggerId.GetHashCode() << 4) ^ (TriggerType.GetHashCode());
			}

			public override bool Equals(object obj) {
				if (obj is TriggerKey) {
					return Equals((TriggerKey) obj);
				}

				return false;
			}
		}

		private UIDialogueBase m_uiCurrent;

		private readonly Dictionary<LeafAsset,LeafNodePackage<ScriptNode>> m_packages;
		private readonly Dictionary<TriggerKey,ScriptNodeSet> m_triggerSets;
		private readonly Dictionary<StringHash32,ScriptNode> m_allNodes;
		private readonly Parser m_parser;
		private readonly CustomVariantResolver m_triggerResolver;

		private LeafThreadHandle m_currentHandle;

		private IGameState m_cachedGameState;

		public ScriptMgr(MonoBehaviour inHost) : base(inHost, new CustomVariantResolver(), null) {
			m_packages = new Dictionary<LeafAsset, LeafNodePackage<ScriptNode>>();
			m_triggerSets = new Dictionary<TriggerKey, ScriptNodeSet>();
			m_allNodes = new Dictionary<StringHash32, ScriptNode>();
			m_parser = new Parser();

			m_triggerResolver = new CustomVariantResolver();
			m_triggerResolver.Base = Resolver;

			Resolver.SetVar(new TableKeyPair("scene", "name"), () => SceneHelper.ActiveScene().Name);
		}

		public void LoadGameState(IGameState gameState, VariantTable table) {
			m_cachedGameState = gameState;
			Resolver.SetDefaultTable(table);
		}

		public void ConfigureEvents() {
			CustomTagParserConfig parseConfig = new CustomTagParserConfig();
			parseConfig.AddEvent("wait", ScriptEvents.Global.Wait).WithFloatData(0.25f);
			
			parseConfig.AddEvent("@*", ScriptEvents.Dialog.Target).ProcessWith(ParseTargetArgs);
			parseConfig.AddEvent("img", ScriptEvents.Dialog.Image).WithStringHashData().CloseWith(ScriptEvents.Dialog.HideImage);
			parseConfig.AddEvent("obj", ScriptEvents.Dialog.Object).WithStringHashData().CloseWith(ScriptEvents.Dialog.HideObject);

			TagStringEventHandler eventConfig = new TagStringEventHandler();
			eventConfig.Register(ScriptEvents.Global.Wait, (e, o) => Routine.WaitSeconds(e.GetFloat()));

			ConfigureTagStringHandling(parseConfig, eventConfig);
		}

		public override LeafThreadHandle Run(ScriptNode inNode, ILeafActor inActor = null, VariantTable inLocals = null, string inName = null) {
			if (inNode.Type == ScriptNode.NodeType.Function) {
				return base.Run(inNode, inActor, inLocals, inName);
			}
			
			m_currentHandle.Kill();
			return m_currentHandle = base.Run(inNode, inActor, inLocals, inName);
		}

		public override void OnNodeEnter(ScriptNode inNode, LeafThreadState<ScriptNode> inThreadState) {
			GameMgr.RecordNodeVisited(inNode);

			if (inNode.Type != ScriptNode.NodeType.Function) {
				if (m_uiCurrent != null) {
					UIMgr.Close(m_uiCurrent);
				}

				if (inNode.Type == ScriptNode.NodeType.TextMessage) {
					m_uiCurrent = UIMgr.Open<UITextMessage>();
					UIMgr.Close<UIDialogScreen>();
				} else if (inNode.Type == ScriptNode.NodeType.PhoneCall) {
					m_uiCurrent = UIMgr.Open<UIDialogScreen>();
					UIMgr.Close<UIPhone>();
				} else if (inNode.Type == ScriptNode.NodeType.Radio) {
					m_uiCurrent = UIMgr.Open<UIRadioDialog>();
					UIMgr.Close<UIPhone>();
				}
				
				ConfigureDisplay(m_uiCurrent, null);
				m_uiCurrent.PrepareNode(inNode);
			}
		}
		public override void OnNodeExit(ScriptNode inNode, LeafThreadState<ScriptNode> inThreadState) {
			if (inThreadState.GetHandle() != m_currentHandle)
				return;

			switch(inNode.Type) {
				case ScriptNode.NodeType.PhoneCall:
					if (m_uiCurrent != null) {
						UIMgr.Close(m_uiCurrent);
					}
					break;
				case ScriptNode.NodeType.TextMessage:
					if (m_uiCurrent != null) {
						UIMgr.Close(m_uiCurrent);
					}
					UIMgr.Close<UIPhone>();
					break;
				case ScriptNode.NodeType.Radio:
					if (m_uiCurrent != null) {
						UIMgr.Close(m_uiCurrent);
					}
					break;
			}
		}
		public override void OnEnd(LeafThreadState<ScriptNode> inThreadState) {
			base.OnEnd(inThreadState);
		}

		#region Package Management

		public LeafNodePackage<ScriptNode> RegisterPackage(LeafAsset asset) {
			LeafNodePackage<ScriptNode> package;
			if (!m_packages.TryGetValue(asset, out package)) {
				package = LeafAsset.Compile(asset, m_parser);
				m_packages.Add(asset, package);
				RegisterNodes(package);
			}
			return package;
		}

		public void DeregisterPackage(LeafAsset asset) {
			LeafNodePackage<ScriptNode> package;
			if (m_packages.TryGetValue(asset, out package)) {
				DeregisterNodes(package);
				m_packages.Remove(asset);
			}
		}

		#endregion // Package Management

		#region Node Management

		public ScriptNode GetNotification(StringHash32 id) {
			ScriptNode node;
			m_allNodes.TryGetValue(id, out node);
			return node;
		}

		private void RegisterNodes(LeafNodePackage<ScriptNode> package) {
			StringHash32 triggerId;
			foreach(var node in package) {
				
				m_allNodes[node.Id()] = node;

				triggerId = node.TriggerId;
				
				if (triggerId.IsEmpty)
					continue;

				TriggerKey key = new TriggerKey(triggerId, node.Type);
				ScriptNodeSet set;
				if (!m_triggerSets.TryGetValue(key, out set)) {
					set = new ScriptNodeSet();
					m_triggerSets.Add(key, set);
				}

				set.Add(node);
			}
		}

		private void DeregisterNodes(LeafNodePackage<ScriptNode> package) {
			StringHash32 triggerId;
			foreach(var node in package) {
				
				m_allNodes.Remove(node.Id());

				triggerId = node.TriggerId;
				
				if (triggerId.IsEmpty)
					continue;

				TriggerKey key = new TriggerKey(triggerId, node.Type);
				ScriptNodeSet set;
				if (m_triggerSets.TryGetValue(key, out set)) {
					set.Remove(node);
				}
			}
		}

		#endregion // Node Management

		#region Triggers

		private bool CanRunNode(ScriptNode node) {
			
				return m_cachedGameState.GetContactNotificationId(node.ContactId) != node.Id();
		}

		public int GetResponsesForTrigger(StringHash32 triggerId, StringHash32 target, ILeafActor actor, VariantTable contextTable, ICollection<ScriptNode> outResponses) {
			ScriptNodeSet.EvaluateParams evalParams;
			evalParams.Resolver = GetResolverForTrigger(contextTable);
			evalParams.Invoker = MethodCache;
			evalParams.Context = actor;
			evalParams.GameState = m_cachedGameState;
			evalParams.Target = target;

			int count = 0;

			TriggerKey responseKey = new TriggerKey(triggerId, TriggerType.Response);
			if (m_triggerSets.TryGetValue(responseKey, out ScriptNodeSet responseSet)) {
				using(PooledList<ScriptNode> priorityNodes = PooledList<ScriptNode>.Create()) {
					int responseCount = responseSet.GetPrioritizedValid(evalParams, priorityNodes);
					if (responseCount > 0) {
						outResponses.Add(RNG.Instance.Choose(priorityNodes));
						count++;
					}
				}
			}

			TriggerKey functionKey = new TriggerKey(triggerId, TriggerType.Function);
			if (m_triggerSets.TryGetValue(functionKey, out ScriptNodeSet functionSet)) {
				count += functionSet.GetAllValid(evalParams, outResponses);
			}

			m_triggerResolver.Clear();
			return count;
		}

		private CustomVariantResolver GetResolverForTrigger(VariantTable contextTable) {
			if (contextTable != null && (contextTable.Count > 0 || contextTable.Base != null)) {
				m_triggerResolver.SetTable(LeafUtils.LocalIdentifier, contextTable);
				return m_triggerResolver;
			}

			return Resolver;
		}

		#endregion // Triggers

		#region Parsing

		private static void ParseTargetArgs(TagData inTag, object inContext, ref TagEventData ioEvent) {
			ioEvent.SetStringHash(inTag.Id.Substring(1));
		}

		#endregion // Parsing
	}

}