using BeauUtil;
using BeauUtil.Blocks;
using Leaf;
using System;
using UnityEngine;

namespace Shipwreck {


	public class ScriptNode : LeafNode {

		public enum NodeType {
			Unassigned,
			TextMessage,
			InPerson
		}

		public NodeType Type {
			get { return m_type; }
		}
		public string FullName {
			get { return m_fullName; }
		}

		private NodeType m_type = NodeType.Unassigned;
		private string m_fullName;

		public ScriptNode(string fullName, ILeafModule inModule) : base(fullName, inModule) {
			m_fullName = fullName;
		}

		[BlockMeta("type")]
		private void SetNodeType(string type) {
			string toParse = type.Replace("-", "").ToLower();
			if (Enum.TryParse(toParse, true, out NodeType result)) {
				m_type = result;
			} else {
				Debug.LogWarningFormat("Could not set node to type `{0}'. Did you mispell it?",type);
			}
		}

	}

}