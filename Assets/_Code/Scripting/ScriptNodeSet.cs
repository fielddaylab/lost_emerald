using BeauUtil;
using BeauUtil.Variants;
using System.Collections.Generic;

namespace Shipwreck {

	/// <summary>
	/// Set of scripting nodes that can be refreshed and polled to 
	/// </summary>
	public class ScriptNodeSet {

		private HashSet<StringHash32> m_ids;
		private List<ScriptNode> m_set;

		public ScriptNodeSet() {
			m_ids = new HashSet<StringHash32>();
			m_set = new List<ScriptNode>();
		}

		
		public void RefreshPriority() {
			
		}

		public void Add(ScriptNode node) {
			if (!m_ids.Contains(node.Id())) {
				m_ids.Add(node.Id());
				m_set.Add(node);
			}
		}
		public void Remove(ScriptNode node) {
			if (m_set.Contains(node) && m_ids.Contains(node.Id())) {
				m_set.Remove(node);
				m_ids.Remove(node.Id());
			}
		}

	}


}