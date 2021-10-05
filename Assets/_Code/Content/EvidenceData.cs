using UnityEngine;
using PotatoLocalization;
using BeauUtil;
using System.Collections.Generic;

namespace Shipwreck {

	[CreateAssetMenu(fileName = "NewEvidenceData", menuName = "Shipwrecks/Evidence")]
	public class EvidenceData : ScriptableObject {

		public StringHash32 GroupID {
			get { return m_groupID; }
		}
		public Vector2 Position {
			get { return m_position; }
		}
		public EvidenceGroup NodeGroup {
			get {
				if (m_isLocalized) {
					if (LocalizationMgr.CurrentLanguage == new LanguageCode("en")) {
						return m_englishPrefab;
					} else {
						return m_spanishPrefab;
					}
				} else {
					return m_englishPrefab;
				}
			}
		}
		public IEnumerable<StringHash32> RootNodes {
			get {
				foreach (SerializedHash32 node in m_rootNodes) {
					yield return node;
				}
			}
		}

		public void SetPosition(Vector2 pos)
		{
			m_position = pos;
		}

		[SerializeField]
		private SerializedHash32 m_groupID;
		[SerializeField]
		private Vector2 m_position = Vector2.zero;
		[SerializeField, Tooltip("Does this use a different prefab based on language?")]
		private bool m_isLocalized = false;
		[SerializeField]
		private EvidenceGroup m_englishPrefab = null;
		[SerializeField]
		private EvidenceGroup m_spanishPrefab = null;
		[SerializeField]
		private SerializedHash32[] m_rootNodes = null;

	}

}


