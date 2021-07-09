using BeauPools;
using BeauUtil;
using BeauUtil.Blocks;
using BeauUtil.Debugger;
using BeauUtil.Variants;
using Leaf;
using System;
using UnityEngine;

namespace Shipwreck {

	public class ScriptNode : LeafNode {

		public enum NodeType {
			Unassigned,
			TextMessage,
			InPerson,
			Radio,
			Function
		}

		public StringHash32 ContactId {
			get { return m_contactId; }
		}
		public NodeType Type {
			get { return m_type; }
		}
		public string FullName {
			get { return m_fullName; }
		}
		public StringHash32 TriggerId {
			get { return m_trigger; }
		}
		public int TriggerPriority {
			get { return m_triggerPriority; }
		}
		public VariantComparison[] TriggerConditions {
			get { return m_conditions; }
		}
		public bool RunOnce {
			get { return m_once; }
		}
		public bool IsNotification {
			get { return m_isNotification; }
		}
		public bool IsConversation {
			get { return m_type > NodeType.Unassigned && m_type < NodeType.Function; }
		}

		private StringHash32 m_contactId = default(StringHash32);
		private NodeType m_type = NodeType.Unassigned;
		private string m_fullName;
		private StringHash32 m_trigger;
		private int m_triggerPriority;
		private VariantComparison[] m_conditions;
		private bool m_once;
		private bool m_isNotification;

		public ScriptNode(string fullName, ILeafModule inModule) : base(fullName, inModule) {
			m_fullName = fullName;
		}

		#region Meta

		[BlockMeta("contact")]
		private void SetContact(StringHash32 contactId, string type) {
			m_contactId = contactId;

			string toParse = type.Replace("-", "").ToLower();
			if (Enum.TryParse(toParse, true, out NodeType result)) {
				m_type = result;
			} else {
				Debug.LogWarningFormat("Could not set node to type `{0}'. Did you mispell it?",type);
			}
		}

		[BlockMeta("trigger")]
		private void SetTrigger(StringHash32 triggerId) {
			m_trigger = triggerId;
		}

		[BlockMeta("when")]
		private void SetConditions(StringSlice text) {
			using(PooledList<StringSlice> conditions = PooledList<StringSlice>.Create()) {
				int conditionsCount = text.Split(ArgsSplitter, StringSplitOptions.RemoveEmptyEntries, conditions);
				if (conditionsCount > 0) {
					m_conditions = new VariantComparison[conditionsCount];
					for(int i = 0; i < conditionsCount; ++i) {
						if (!VariantComparison.TryParse(conditions[i], out m_conditions[i])) {
							Log.Error("[ScriptNode] Unable to parse condition '{0}'", conditions[i]);
						}
					}
					m_triggerPriority += conditionsCount;
				}
			}
		}

		[BlockMeta("priority")]
		private void SetPriority(int priority) {
			m_triggerPriority += priority;
		}

		[BlockMeta("once")]
		private void SetOnce() {
			m_once = true;
		}

		[BlockMeta("notification")]
		private void SetNotification() {
			m_isNotification = true;
		}

		[BlockMeta("function")]
		private void SetFunction() {
			m_type = NodeType.Function;
			m_once = false;
		}

		#endregion // Meta

		static public readonly StringSlice.ISplitter ArgsSplitter = new StringUtils.ArgsList.Splitter();
	}

}