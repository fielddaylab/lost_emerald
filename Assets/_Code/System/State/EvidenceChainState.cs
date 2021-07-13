using BeauData;
using BeauUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public sealed partial class GameMgr { // EvidenceChainState.cs
		private sealed partial class GameState { // EvidenceChainState.cs


			private class EvidenceChainState : ISerializedObject {

				private StringHash32 m_rootNode;
				private Stack<StringHash32> m_chain;
				private Vector2 m_danglePos;
				private bool m_isLocked;

				public EvidenceChainState() {
					// empty constructor for deserialization
				}
				public EvidenceChainState(StringHash32 root, Vector2 danglingPos) {
					m_rootNode = root;
					m_chain = new Stack<StringHash32>();
					m_danglePos = danglingPos;
					m_isLocked = false;
				}

				public StringHash32 Root() {
					return m_rootNode;
				}

				public void Lock() {
					m_isLocked = true;
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
				public void Lift(StringHash32 node) {
					if (m_isLocked) {
						throw new InvalidOperationException("Cannot Lift pins when the Chain is locked!");
					}
					if (!m_chain.Contains(node) && node != m_rootNode) {
						throw new InvalidOperationException(string.Format("Cannot Lift node `{0}' " +
							"because it is not part of this chain!", node.ToDebugString()
						));
					}
					while (m_chain.Count > 0 && m_chain.Pop() != node) { 
						// intentionally removing all nodes in the chain
						// (including the lifted one) until the desired
						// node is found or chain is empty
					}
				}

				/// <summary>
				/// Drops a pin at the end of the chain at the given node and moves 
				/// the dangling pin to danglePos. If the root node is provided, 
				/// no pin will be added. 
				/// </summary>
				public void Drop(StringHash32 node, Vector2 danglePos) {
					if (m_isLocked) {
						throw new InvalidOperationException("Cannot Drop pins when the Chain is locked!");
					}
					if (node != m_rootNode) {
						if (Contains(node)) {
							throw new InvalidOperationException("Cannot Drop " +
								"pins on nodes that are already within the chain."
							);
						}
						m_chain.Push(node);
					}
					m_danglePos = danglePos;
				}


				public void Serialize(Serializer ioSerializer) {
					StringHash32[] chain;
					if (ioSerializer.IsReading) {
						chain = new StringHash32[0];
					} else {
						chain = new StringHash32[m_chain.Count];
						m_chain.CopyTo(chain, 0);
					}

					ioSerializer.UInt32Proxy("root", ref m_rootNode);
					ioSerializer.UInt32ProxyArray("chain", ref chain);
					ioSerializer.Serialize("danglePos", ref m_danglePos);
					ioSerializer.Serialize("isLocked", ref m_isLocked);

					if (ioSerializer.IsReading) {
						m_chain.Clear();
						for (int ix = 0; ix < chain.Length; ix++) {
							m_chain.Push(chain[ix]);
						}
					}
				}


			}
		}
	}
}