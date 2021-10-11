using BeauData;
using BeauUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public interface IEvidenceChainState {

		bool IsCorrect { get; }
		StickyInfo StickyInfo { get; }
		int Depth { get; }

		StringHash32 Root();

		StringHash32 GetNodeInChain(int index);

		bool Contains(StringHash32 node);
		bool ContainsSet(IEnumerable<StringHash32> node);
		void Lift(int depth);
		void Drop(StringHash32 node);

		List<StringHash32> Chain();
		void SetEChain(EvidenceChain eChain);
	}

	public sealed partial class GameMgr { // EvidenceChainState.cs

		private class EvidenceChainState : IEvidenceChainState, ISerializedObject, ISerializedCallbacks {

			// serialized
			private StringHash32 m_rootNode;
			private List<StringHash32> m_chain;

			// non-serialized
			private StickyInfo m_stickyData;
			private StickyEvaluator.RootSolvedPredicate m_levelRootEvaluator;

			private EvidenceChain m_eChain;

			public EvidenceChainState() {
				// empty constructor for deserialization
			}
			public EvidenceChainState(StringHash32 root) {
				m_rootNode = root;
				m_chain = new List<StringHash32>();
			}

			public void SetEChain(EvidenceChain eChain) {
				m_eChain = eChain;
			}

			public StickyInfo StickyInfo { 
				get { return m_stickyData; } 
			}
			public bool IsCorrect {
				get { return m_stickyData != null && m_stickyData.Response == StickyInfo.ResponseType.Correct; }
			}
			public int Depth {
				get { return m_chain.Count + 1; }
			}

			public StringHash32 Root() {
				return m_rootNode;
			}
			public List<StringHash32> Chain() {
				return m_chain;
			}

			public void SetRootEvaluator(StickyEvaluator.RootSolvedPredicate predicate) {
				m_levelRootEvaluator = predicate;
			}

			public StringHash32 GetNodeInChain(int index) {
				if (index < 1 || index > m_chain.Count) {
					throw new IndexOutOfRangeException();
				}
				return m_chain[index-1];
			}

			public void ReevaluateStickyInfo() {
				bool wasCorrect = IsCorrect;
				m_stickyData = I.m_stickyEvaluator.Evaluate(m_rootNode, m_chain, m_levelRootEvaluator);
				if (!wasCorrect && IsCorrect) {
					Events.Dispatch(GameEvents.ChainSolved, m_rootNode);
					using (var table = TempVarTable.Alloc()) {
						table.Set("root", m_rootNode);
						RunTrigger(GameTriggers.OnChainSolved, table);
					}
				}
			}

			public bool Contains(StringHash32 node) {
				return node == m_rootNode || m_chain.Contains(node);
			}
			public bool ContainsSet(IEnumerable<StringHash32> set) {
				foreach (StringHash32 node in set) {
					if (!Contains(node)) {
						return false;
					}
				}
				return true;
			}

			/// <summary>
			/// Lifts the pin currently attached to the given node. If the root node is
			/// provided, the current dangling pin will be lifted.
			/// </summary>
			public void Lift(int depth) {
			/*
				if (IsCorrect) {
					throw new InvalidOperationException("Cannot Lift pins when the Chain is locked!");
				}
			*/
				while (m_chain.Count > depth) {
					EvidencePin ePin = m_eChain.GetPin(m_chain.Count - 1);
					if (GraphicsRaycasterMgr.instance.RaycastForNode(ePin.transform.position, out EvidenceNode node)) {
						node.SetPinned(false);
						node.SetColor(GameDb.GetPinColor(ChainStatus.Normal));
					}
					m_chain.RemoveAt(m_chain.Count - 1);

					// intentionally removing all nodes in the chain
					// (including the lifted one) until the desired
					// node is found or chain is empty
				}
				ReevaluateStickyInfo();
			}

			/// <summary>
			/// Drops a pin at the end of the chain at the given node and moves 
			/// the dangling pin to danglePos. If the root node is provided, 
			/// no pin will be added. 
			/// </summary>
			public void Drop(StringHash32 node) {
			/*
				if (IsCorrect) {
					throw new InvalidOperationException("Cannot Drop pins when the Chain is locked!");
				}
			*/
				if (node != m_rootNode) {
					if (Contains(node)) {
						/*
						throw new InvalidOperationException("Cannot Drop " +
							"pins on nodes that are already within the chain."
						);
						*/
					}
					m_chain.Add(node);
				}
				
				ReevaluateStickyInfo();
			}


			public void Serialize(Serializer ioSerializer) {
				ioSerializer.UInt32Proxy("root", ref m_rootNode);
				ioSerializer.UInt32ProxyArray("chain", ref m_chain);
			}

			public void PostSerialize(Serializer.Mode inMode, ISerializerContext inContext) {
				if (inMode == Serializer.Mode.Read) {
					ReevaluateStickyInfo();
				}
			}
		}
	}
}