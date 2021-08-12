using BeauRoutine;
using UnityEngine;

namespace Shipwreck {

	public class DiveSceneMgr : MonoBehaviour {

		[SerializeField]
		private DiveNode m_startNode = null;
		[SerializeField]
		private float m_startDelay = 1.0f;
		[SerializeField]
		private Color m_clickTargetBaseColor = Color.black;
		[SerializeField]
		private Color m_clickTargetSelectedColor = Color.black;

		private DiveNode[] m_diveNodes;
		private DiveNode m_currentNode;
		private Routine m_startRoutine;

		private void Awake() {
			m_diveNodes = GetComponentsInChildren<DiveNode>();
			m_startRoutine.Replace(this, Routine.Delay(()=> { SetNode(m_startNode); }, m_startDelay));
			InputMgr.Register(InputMgr.OnInteractPressed, HandleInteractPressed);
		}
		private void OnDestroy() {
			InputMgr.Deregister(InputMgr.OnInteractPressed, HandleInteractPressed);
		}

		private void HandleTryTakePhoto() {
			// there must be a point of interest at the location
			// or the request will be ignored
		}

		private void HandleInteractPressed() {
			if (Physics.Raycast(Camera.main.ScreenPointToRay(InputMgr.Position), out RaycastHit hit)) {
				foreach (DiveNode node in m_diveNodes) {
					if (node.MatchesCollider(hit.collider)) {
						SetNode(node);
						break;
					}
				}
			}
		}

		private void SetNode(DiveNode node) {
			if (m_currentNode != null) {
				//m_currentNode.SetColor(m_clickTargetBaseColor);
				m_currentNode.Deprioritize();
			}
			m_currentNode = node;
			//m_currentNode.SetColor(m_clickTargetSelectedColor);
			m_currentNode.Prioritize();
		}
	}

}