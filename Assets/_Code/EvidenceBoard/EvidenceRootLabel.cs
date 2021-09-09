using BeauRoutine;
using BeauUtil;
using PotatoLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Shipwreck {

	
	public class EvidenceRootLabel : MonoBehaviour {

		[SerializeField]
		private LocalizedTextUGUI m_label = null;


		private void OnEnable() {
			GameMgr.Events.Register<StringHash32>(GameEvents.ChainSolved, HandleChainComplete);
			RefreshLabel();
		}
		private void OnDisable() {
			GameMgr.Events.Deregister<StringHash32>(GameEvents.ChainSolved, HandleChainComplete);
		}

		private void HandleChainComplete(StringHash32 root) {
			RefreshLabel();
		}

		private void RefreshLabel() {
			m_label.Key = GameMgr.State.CurrentLevel.Name;
		}

	}



}

