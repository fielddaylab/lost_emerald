﻿using BeauUtil;
using PotatoLocalization;
using UnityEngine;

namespace Shipwreck {

	/// <summary>
	/// variable state related to an individual level, 
	/// such as the hash code used for the Location chain
	/// </summary>
	[CreateAssetMenu(fileName = "NewLevelData", menuName = "Shipwrecks/Level")]
	public class LevelData : ScriptableObject {

		public int LevelIndex {
			get { return m_levelIndex; }
		}
		public StringHash32 LocationRoot {
			get { return m_locationRoot; }
		}
		public StringHash32 NameRoot {
			get { return m_nameRoot; }
		}
		public StringHash32 TopDownPhotoID {
			get { return m_topDownPhotoID; }
		}
		public LocalizationKey UnnamedKey {
			get { return m_unnamedKey; }
		}
		public LocalizationKey NamedKey {
			get { return m_namedKey; }
		}
		public LocalizationKey LockedKey {
			get { return m_lockedKey; }
		}

		[SerializeField]
		private int m_levelIndex = 0;
		[SerializeField]
		private SerializedHash32 m_locationRoot = StringHash32.Null;
		[SerializeField]
		private SerializedHash32 m_nameRoot = StringHash32.Null;
		[SerializeField]
		private SerializedHash32 m_topDownPhotoID = StringHash32.Null;
		[SerializeField]
		private LocalizationKey m_unnamedKey = LocalizationKey.Empty;
		[SerializeField]
		private LocalizationKey m_namedKey = LocalizationKey.Empty;
		[SerializeField]
		private LocalizationKey m_lockedKey = LocalizationKey.Empty;
	}

}