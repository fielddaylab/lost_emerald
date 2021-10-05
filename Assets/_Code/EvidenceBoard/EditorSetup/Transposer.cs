using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Shipwreck
{
	public class Transposer : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> m_evidencePrefabs;
		[SerializeField]
		private List<EvidenceData> m_evidenceData;

		[ContextMenu("Transpose Evidence")]
		private void TransposeEvidence()
		{
			if (m_evidencePrefabs.Count != m_evidenceData.Count)
			{
				Debug.Log("Data not transferred: Data Mismatch. Ensure you have the same number of prefabs as data.");
				return;
			}

			int numElements = m_evidencePrefabs.Count;

			for (int i = 0; i < numElements; ++i)
			{
				m_evidenceData[i].SetPosition(new Vector2(
					m_evidencePrefabs[i].transform.localPosition.x,
					m_evidencePrefabs[i].transform.localPosition.y
					));

#if UNITY_EDITOR
				EditorUtility.SetDirty(m_evidenceData[i]);
#endif
			}

#if UNITY_EDITOR
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log("Tranposition Successful");
#endif
		}
	}
}
