using BeauUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck
{
	/// <summary>
	/// For modifying evidence according to the progress of other chains
	/// </summary>
	public class ModifiableEvidence : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_state1;
		[SerializeField]
		private GameObject m_state2;
		[SerializeField]
		private string m_triggerRoot;

		private void Awake()
		{
			UIEvidenceScreen.ChainCompleted.AddListener(ModState);
		}

		private void ModState()
		{
			if (!GameMgr.State.CurrentLevel.IsChainComplete(m_triggerRoot))
			{
				return;
			}

			if (m_state1.activeSelf)
			{
				m_state1.SetActive(false);
				m_state2.SetActive(true);
				UIEvidenceScreen.ChainCompleted.RemoveListener(ModState);
			}
			else
			{
				Debug.Log("Error: tried to modify the state of evidence that is already modified");
			}
		}
	}
}
