using BeauUtil;
using PotatoLocalization;
using UnityEngine;

namespace Shipwreck {


	[CreateAssetMenu(fileName = "NewDisplayObj", menuName = "Shipwrecks/Display Object")]
	public class DisplayObjData : ScriptableObject {

		public StringHash32 Hash {
			get { return name; }
		}
		public GameObject Prefab {
			get {
				if (LocalizationMgr.CurrentLanguage == new LanguageCode("en")) {
					return m_englishPrefab;
				} else {
					return m_spanishPrefab;
				}
			}
		}

		[SerializeField]
		private GameObject m_englishPrefab = null;
		[SerializeField]
		private GameObject m_spanishPrefab = null;

	}

}