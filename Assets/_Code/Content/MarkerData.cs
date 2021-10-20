using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {
	[CreateAssetMenu(fileName = "NewMarkerData", menuName = "Shipwrecks/Marker")]
	public class MarkerData : ScriptableObject {
		public Vector2 MarkerPos {
			get { return m_markerPos; }
		}
		public bool LocationKnown {
			get { return m_locationKnown; }
		}
		public string UnknownSpriteID {
			get { return m_unknownSpriteID; }
		}

		[SerializeField]
		private Vector2 m_markerPos;
		[SerializeField]
		private bool m_locationKnown = true;
		[SerializeField]
		private string m_unknownSpriteID = null;
	}

}