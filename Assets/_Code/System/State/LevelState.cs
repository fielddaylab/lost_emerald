using BeauData;
using BeauUtil;
using System;
using System.Collections.Generic;

namespace Shipwreck {

	public sealed partial class GameMgr { // LevelState.cs

		private sealed partial class GameState { // LevelState.cs

			private class LevelState : ISerializedObject, ISerializedVersion {
				public ushort Version {
					get { return 1; }
				}

				public bool IsUnlocked {
					get { return m_isUnlocked; }
				}

				public IEnumerable<IEvidenceGroupState> Evidence {
					get {
						foreach (EvidenceGroupState evidence in m_evidence) {
							yield return evidence;
						}
					}
				}
				public int ChainCount {
					get { return m_chains.Count; }
				}

				// serialized
				private bool m_isUnlocked = false;
				private bool m_hasSeenCutscene = false;
				private List<EvidenceGroupState> m_evidence;
				private List<EvidenceChainState> m_chains;

				// non-serialized
				private LevelData m_levelData;

				public LevelState() {
					m_evidence = new List<EvidenceGroupState>();
					m_chains = new List<EvidenceChainState>();
				}

				public void AssignLevelData(LevelData data) {
					m_levelData = data;
				}

				public IEvidenceChainState GetChain(StringHash32 root) {
					return m_chains.Find((item) => {
						return item.Root() == root;
					});
				}
				public IEvidenceChainState GetChain(int index) {
					if (index < 0 || index >= m_chains.Count) {
						throw new IndexOutOfRangeException();
					}
					return m_chains[index];
				}

				public bool Unlock() {
					if (m_isUnlocked) {
						return false;
					} else {
						m_isUnlocked = true;
						return true;
					}
				}
				public bool UnlockEvidence(EvidenceData group) {
					if (IsEvidenceUnlocked(group.GroupID)) {
						return false;
					} else {
						// todo: determine position
						m_evidence.Add(new EvidenceGroupState(group.GroupID, group.Position));
						foreach (StringHash32 root in group.RootNodes) {
							m_chains.Add(new EvidenceChainState(root));
						}
						return true;
					}
				}
				public bool IsEvidenceUnlocked(StringHash32 group) {
					return m_evidence.Find((item) => {
						return item.Identity == group;
					}) != null;
				}
				public bool IsLocationChainComplete() {
					return GetChain(m_levelData.LocationRoot).IsCorrect;
				}
				public bool HasTakenTopDownPhoto() {
					return IsEvidenceUnlocked(m_levelData.TopDownPhotoID);
				}
				public bool IsChainComplete(StringHash32 root) {
					return m_chains.Find((item) => {
						return item.Root() == root;
					})?.IsCorrect ?? false;
				}
				

				public void Serialize(Serializer ioSerializer) {
					ioSerializer.Serialize("isUnlocked", ref m_isUnlocked);
					ioSerializer.ObjectArray("evidence", ref m_evidence);
					ioSerializer.ObjectArray("chains", ref m_chains);
					ioSerializer.Serialize("hasSeenCutscene", ref m_hasSeenCutscene);
				}

				public void SetCutsceneSeen() {
					m_hasSeenCutscene = true;
				}
			}
		}
	}
}