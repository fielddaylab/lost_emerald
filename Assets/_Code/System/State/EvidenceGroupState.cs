using BeauData;
using BeauUtil;
using UnityEngine;

namespace Shipwreck {


	public sealed partial class GameMgr { // EvidenceGroupState.cs
		private sealed partial class GameState { // EvidencesGroupState.cs

			/// <summary>
			/// Holds a set of evidence nodes that have been unlocked by the player
			/// that are all moved together when the player clicks and drags them
			/// </summary>
			private class EvidenceGroupState : ISerializedObject {

				public StringHash32 Identity {
					get { return m_identity; }
				}
				public Vector2 Position {
					get { return m_position; }
				}
				public bool IsRevealed {
					get { return m_isRevealed; }
				}

				// serialized
				private StringHash32 m_identity;
				private Vector2 m_position;
				private bool m_isRevealed;

				public EvidenceGroupState() {
					// empty constructor for deserialization
				}
				public EvidenceGroupState(StringHash32 id, Vector2 position) {
					m_identity = id;
					m_position = position;
					m_isRevealed = false;
				}

				public void SetPosition(Vector2 position) {
					m_position = position;
				}
				public void MarkAsRevealed() {
					m_isRevealed = true;
				}
				public void Serialize(Serializer ioSerializer) {
					ioSerializer.UInt32Proxy("id", ref m_identity);
					ioSerializer.Serialize("position", ref m_position);
					ioSerializer.Serialize("revealed", ref m_isRevealed);
				}
			}

		}

	}

}