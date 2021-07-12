using UnityEngine;
using PotatoLocalization;
using BeauUtil;

namespace Shipwreck {

	[CreateAssetMenu(fileName = "NewEvidenceData", menuName = "Shipwrecks/Evidence")]
	public class EvidenceData : ScriptableObject {

		
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

		[SerializeField, Tooltip("Does this use a different prefab based on language?")]
		private bool m_isLocalized = false;
		[SerializeField]
		private EvidenceGroup m_englishPrefab = null;
		[SerializeField]
		private EvidenceGroup m_spanishPrefab = null;

	}

}


