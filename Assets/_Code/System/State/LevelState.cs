using BeauData;
using BeauUtil;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	public interface ILevelState {
		bool IsUnlocked { get; }
	}


	public sealed partial class GameMgr { // LevelState.cs

		private sealed partial class GameState { // LevelState.cs

			private class LevelState : ILevelState, ISerializedObject, ISerializedVersion {
				public ushort Version { 
					get { return 1; }
				}

				public bool IsUnlocked {
					get { return m_isUnlocked; }
				}

				// serialized
				private bool m_isUnlocked = false;
				private List<EvidenceGroupState> m_evidence;
				private List<EvidenceChainState> m_connections;

				public void Unlock() {
					m_isUnlocked = true;
				}
				public void UnlockEvidence(StringHash32 group) {
					// todo: determine position
					m_evidence.Add(new EvidenceGroupState(group,Vector2.zero));
				}
				

				public void Serialize(Serializer ioSerializer) {
					ioSerializer.Serialize("isUnlocked", ref m_isUnlocked);
					ioSerializer.ObjectArray("evidence", ref m_evidence);
					ioSerializer.ObjectArray("connections", ref m_connections);
				}
			}
		}

	}

}