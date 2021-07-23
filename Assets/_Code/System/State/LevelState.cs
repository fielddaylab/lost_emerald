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

				public IEnumerable<IEvidenceGroupState> Evidence {
					get {
						foreach (EvidenceGroupState evidence in m_evidence) {
							yield return evidence;
						}
					}
				}
				public IEnumerable<IEvidenceChainState> Chains {
					get {
						foreach (EvidenceChainState chain in m_chains) {
							yield return chain;
						}
					}
				}

				// serialized
				private bool m_isUnlocked = false;
				private List<EvidenceGroupState> m_evidence;
				private List<EvidenceChainState> m_chains;

				public LevelState() {
					m_evidence = new List<EvidenceGroupState>();
					m_chains = new List<EvidenceChainState>();
					UnlockEvidence(GameDb.GetEvidenceData("LV1-Root"));
					//UnlockEvidence(GameDb.GetEvidenceData("LV1-Table-Wrecks"));
					//UnlockEvidence(GameDb.GetEvidenceData("LV1-Photo-Above"));
					//UnlockEvidence(GameDb.GetEvidenceData("LV1-Photo-Name"));
					//UnlockEvidence(GameDb.GetEvidenceData("LV1-Photo-Artifact"));
					//UnlockEvidence(GameDb.GetEvidenceData("LV1-Card-Types"));
					//UnlockEvidence(GameDb.GetEvidenceData("LV1-Transcript-Lou"));
					//UnlockEvidence(GameDb.GetEvidenceData("LV1-Article-Sinking"));
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
							m_chains.Add(new EvidenceChainState(root, Vector2.zero));
						}
						return true;
					}
				}
				public bool IsEvidenceUnlocked(StringHash32 group) {
					return m_evidence.Find((item) => {
						return item.Identity == group;
					}) != null;
				}
				

				public void Serialize(Serializer ioSerializer) {
					ioSerializer.Serialize("isUnlocked", ref m_isUnlocked);
					ioSerializer.ObjectArray("evidence", ref m_evidence);
					ioSerializer.ObjectArray("chains", ref m_chains);
				}
			}
		}
	}
}