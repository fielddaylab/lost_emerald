using BeauData;
using BeauUtil;

namespace Shipwreck {

	public interface ILevelState {
		bool IsUnlocked { get; }
	}


	public sealed partial class GameMgr : Singleton<GameMgr> { // LevelState.cs

		private sealed partial class GameState : IGameState, ISerializedObject, ISerializedVersion { // LevelState.cs

			private class LevelState : ILevelState, ISerializedObject, ISerializedVersion {
				public ushort Version { 
					get { return 1; } 
				}

				public bool IsUnlocked {
					get { return m_isUnlocked; }
				}

				// serialized
				private bool m_isUnlocked = false;

				public void Unlock() {
					m_isUnlocked = true;
				}

				public void Serialize(Serializer ioSerializer) {
					ioSerializer.Serialize("isUnlocked", ref m_isUnlocked);
				}
			}
		}

	}

}