using BeauUtil;
using BeauUtil.Debugger;
using BeauUtil.Variants;
using Leaf.Runtime;
using System;
using System.Collections.Generic;

namespace Shipwreck {

	/// <summary>
	/// Set of scripting nodes that can be refreshed and polled to 
	/// </summary>
	public class ScriptNodeSet {

		private readonly List<ScriptNode> m_nodeSet;
		private bool m_setNeedsReorder;

		public ScriptNodeSet() {
			m_nodeSet = new List<ScriptNode>();
		}

		public void Optimize() {
			if (!m_setNeedsReorder)
				return;

			m_nodeSet.Sort(PrioritySorter.Instance);
			m_setNeedsReorder = false;
		}

		public void Add(ScriptNode node) {
			Assert.True(!m_nodeSet.Contains(node), "Node {0} is already registered", node.Id());
			m_nodeSet.Add(node);
			m_setNeedsReorder = true;
		}

		public void Remove(ScriptNode node) {
			Assert.True(m_nodeSet.Contains(node), "Node {0} is already deregistered", node.Id());
			m_nodeSet.FastRemove(node);
			m_setNeedsReorder = true;
		}

		public int GetPrioritizedValid(in EvaluateParams parameters, ICollection<ScriptNode> outNodes) {
			Optimize();

			ScriptNode node;
			int count = 0;
			int minPriority = int.MinValue;

			for(int nodeIdx = 0, nodeCount = m_nodeSet.Count; nodeIdx < nodeCount; nodeIdx++) {
				node = m_nodeSet[nodeIdx];

				if (node.TriggerPriority < minPriority)
					break;

				if (!EvaluateNode(parameters, node))
					continue;

				minPriority = node.TriggerPriority;
				outNodes.Add(node);
				count++;
			}

			return count;
		}

		public int GetAllValid(in EvaluateParams parameters, ICollection<ScriptNode> outNodes) {
			
			ScriptNode node;
			int count = 0;

			for(int nodeIdx = 0, nodeCount = m_nodeSet.Count; nodeIdx < nodeCount; nodeIdx++) {
				node = m_nodeSet[nodeIdx];

				if (!EvaluateNode(parameters, node))
					continue;

				outNodes.Add(node);
				count++;
			}

			return count;
		}

		private bool EvaluateNode(in EvaluateParams parameters, ScriptNode node) {
			if (!GameMgr.State.IsContactUnlocked(node.ContactId)) {
				return false;
			}
			if (!parameters.Target.IsEmpty && parameters.Target != node.ContactId)
				return false;

			if (node.RunOnce && parameters.GameState.HasVisitedNode(node))
				return false;

			VariantComparison[] conditions = node.TriggerConditions;
			if (conditions != null) {
				for(int conditionIdx = 0, conditionCount = conditions.Length; conditionIdx < conditionCount; conditionIdx++) {
					ref var comp = ref conditions[conditionIdx];
					if (!comp.Evaluate(parameters.Resolver, parameters.Context, parameters.Invoker)) {
						return false;
					}
				}
			}

			return true;
		}

		private class PrioritySorter : IComparer<ScriptNode> {

			static internal readonly PrioritySorter Instance = new PrioritySorter();

			public int Compare(ScriptNode x, ScriptNode y)
			{
				return Math.Sign(y.TriggerPriority - x.TriggerPriority);
			}
		}

		public struct EvaluateParams
		{
			public IVariantResolver Resolver;
			public IMethodCache Invoker;
			public IGameState GameState;
			public StringHash32 Target;
			public ILeafActor Context;
		}

		public delegate bool NodePredicate(ScriptNode node);
	}


}