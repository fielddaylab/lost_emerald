﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {
	[CreateAssetMenu(fileName = "NewMarkerData", menuName = "Shipwrecks/Marker")]
	public class MarkerData : ScriptableObject {
		public Vector2 MarkerPos {
			get { return m_markerPos; }
		}
		public Vector2 BannerPos {
			get { return m_bannerPos; }
		}
		public bool LocationKnown {
			get { return m_locationKnown; }
		}
		public string UnknownSpriteID {
			get { return m_unknownSpriteID; }
		}
		public Vector2 UnknownSpriteOffset {
			get { return m_unknownSpriteOffset; }
		}

		[SerializeField]
		private Vector2 m_markerPos;
		[SerializeField]
		private Vector2 m_bannerPos = new Vector2(0, 38.8f);
		[SerializeField]
		private bool m_locationKnown = true;
		[SerializeField]
		private string m_unknownSpriteID = null;
		[SerializeField]
		private Vector2 m_unknownSpriteOffset = new Vector2(0, 0f);
	}

}