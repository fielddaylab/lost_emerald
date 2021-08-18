using BeauUtil;
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
		public StringHash32 TopDownPhotoID {
			get { return m_topDownPhotoID; }
		}

		[SerializeField]
		private int m_levelIndex = 0;
		[SerializeField]
		private SerializedHash32 m_locationRoot = StringHash32.Null;
		[SerializeField]
		private SerializedHash32 m_topDownPhotoID = StringHash32.Null;

	}

}