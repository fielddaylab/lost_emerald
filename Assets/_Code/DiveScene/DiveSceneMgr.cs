using BeauPools;
using BeauRoutine;
using PotatoLocalization;
using System.Collections;
using System.Collections.Generic;
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

		[SerializeField]
		private LocalizationKey m_nothingOfInterestKey = LocalizationKey.Empty;
		[SerializeField]
		private LocalizationKey m_pleaseZoomInKey = LocalizationKey.Empty;
		[SerializeField]
		private LocalizationKey m_pleaseZoomOutKey = LocalizationKey.Empty;
		[SerializeField]
		private LocalizationKey m_alreadyTakenKey = LocalizationKey.Empty;

		private DiveNode[] m_diveNodes;
		private DiveNode m_currentNode;
		private Routine m_startRoutine;
		private Routine m_transitionRoutine;
		private bool m_isNavActive = true;
		private float m_zoomLevel = 0f;

		private void Awake() {
			m_diveNodes = GetComponentsInChildren<DiveNode>();
			m_startRoutine.Replace(this, Routine.Delay(() => { SetNode(m_startNode); }, m_startDelay));
			HandleNavDeactivated(); // default to off
			InputMgr.Register(InputMgr.OnInteractPressed, HandleInteractPressed);
			GameMgr.Events.Register(GameEvents.Dive.AttemptPhoto, HandleAttemptPhoto, this);
			GameMgr.Events.Register<float>(GameEvents.Dive.CameraZoomChanged, HandleCameraZoomChanged, this);
			GameMgr.Events.Register(GameEvents.Dive.NavigationActivated, HandleNavActivated, this);
			GameMgr.Events.Register(GameEvents.Dive.NavigationDeactivated, HandleNavDeactivated, this);
			GameMgr.Events.Register(GameEvents.Dive.RequestPhotoList, HandlePhotoListRequested, this);
			GameMgr.Events.Register(GameEvents.Dive.NavigateToAscendNode, HandleNavToAscendNode, this);
		}
		private void OnDestroy() {
			if (!GameMgr.Exists) {
				return;
			}
			InputMgr.Deregister(InputMgr.OnInteractPressed, HandleInteractPressed);
			GameMgr.Events.DeregisterAll(this);
		}

		private void HandleAttemptPhoto() {
			// Reset any previous custom messages
			GameMgr.State.CurrentLevel.SetCurrentMessage("");

			// there must be a point of interest or custom message at the location
			// or the request will be ignored
			DivePointOfInterest poi = m_currentNode.GetPointOfInterest();
			if (poi == null) {
				// check for custom message
				DivePointCustomMessage pcm = m_currentNode.GetPointCustomMessage();
				if (pcm == null) {
                    GameMgr.Events.Dispatch(GameEvents.Dive.NoPhotoAvailable);
					GameMgr.RunTrigger(GameTriggers.Dive.OnNothingOfInterest);
					GameMgr.Events.Dispatch(GameEvents.Dive.ShowMessage);
					AudioSrcMgr.instance.PlayOneShot("photo_fail");
				}
				else {
                    GameMgr.Events.Dispatch(GameEvents.Dive.NoPhotoAvailable);
					GameMgr.State.CurrentLevel.SetCurrentMessage(pcm.CustomMessageKey);
					GameMgr.RunTrigger(GameTriggers.Dive.OnNothingOfInterest);
					GameMgr.Events.Dispatch(GameEvents.Dive.ShowMessage, pcm.CustomMessageKey);
					AudioSrcMgr.instance.PlayOneShot("photo_fail");
				}
			}
			else if (GameMgr.State.CurrentLevel.IsEvidenceUnlocked(poi.EvidenceUnlock)) {
                GameMgr.Events.Dispatch(GameEvents.Dive.PhotoAlreadyTaken);
				GameMgr.RunTrigger(GameTriggers.Dive.OnPhotoAlreadyTaken);
				GameMgr.Events.Dispatch(GameEvents.Dive.ShowMessage);
				AudioSrcMgr.instance.PlayOneShot("photo_fail");
			}
			else if (m_zoomLevel < poi.ZoomMin) {
                GameMgr.Events.Dispatch(GameEvents.Dive.PhotoFail);
				GameMgr.RunTrigger(GameTriggers.Dive.OnZoomIn);
				GameMgr.Events.Dispatch(GameEvents.Dive.ShowMessage);
				AudioSrcMgr.instance.PlayOneShot("photo_fail");
			}
			else if (m_zoomLevel > poi.ZoomMax) {
                GameMgr.Events.Dispatch(GameEvents.Dive.PhotoFail);
				GameMgr.RunTrigger(GameTriggers.Dive.OnZoomOut);
				GameMgr.Events.Dispatch(GameEvents.Dive.ShowMessage);
				AudioSrcMgr.instance.PlayOneShot("photo_fail");
			}
			else {
				GameMgr.Events.Dispatch(GameEvents.Dive.ConfirmPhoto, poi.EvidenceUnlock);
				GameMgr.Events.Dispatch(GameEvents.Dive.ShowMessage, poi.UnlockMessage);
				AudioSrcMgr.instance.PlayOneShot("take_photo");

                if (HasAllPhotos()) {
                    GameMgr.Events.Dispatch(GameEvents.Dive.AllPhotosTaken);
                }
			}
		}

		private void HandleCameraZoomChanged(float zoom) {
			m_zoomLevel = zoom;
			if (m_currentNode != null) {
				m_currentNode.SetZoom(m_zoomLevel);
			}
		}
		private void HandleNavActivated() {
			foreach (DiveNode node in m_diveNodes) {
				node.SetActive(true);
			}
			if (m_currentNode != null) {
				m_currentNode.SetActive(false);
			}
			HandleCameraZoomChanged(0f);
			m_isNavActive = true;
		}
		private void HandleNavDeactivated() {
			foreach (DiveNode node in m_diveNodes) {
				node.SetActive(false);
			}
			m_isNavActive = false;
		}

		private void HandleNavToAscendNode() {
			SetNode(m_startNode);
		}

		private void HandleInteractPressed() {
			if (m_isNavActive && Physics.Raycast(Camera.main.ScreenPointToRay(InputMgr.Position), out RaycastHit hit)) {
				foreach (DiveNode node in m_diveNodes) {
					if (node.MatchesCollider(hit.collider)) {
						SetNode(node);
						break;
					}
				}
			}
		}

		private void HandlePhotoListRequested() {
			// find all of the points of interest
			List<DivePointOfInterest> list = new List<DivePointOfInterest>();
			foreach (DiveNode node in m_diveNodes) {
				DivePointOfInterest poi = node.GetPointOfInterest();
				if (poi != null) {
					list.Add(poi);
				}
			}
			GameMgr.Events.Dispatch(GameEvents.Dive.SendPhotoList, list);
			GameMgr.Events.Dispatch(GameEvents.Dive.PhotoListSent);
		}

        private bool HasAllPhotos() {
            foreach(DiveNode node in m_diveNodes) {
                DivePointOfInterest poi = node.GetPointOfInterest();
				if (poi != null) {
					if (!poi.EvidenceUnlock.IsEmpty && !GameMgr.State.IsEvidenceUnlocked(GameMgr.State.CurrentLevel.Index, poi.EvidenceUnlock)) {
                        return false;
                    }
				}
            }

            return true;
        }

		private void SetNode(DiveNode node) {
			if (m_currentNode != null) {
				m_currentNode.SetActive(true);
				m_currentNode.Deprioritize();
			}
			m_currentNode = node;
			m_currentNode.Prioritize();
			m_currentNode.SetActive(false);
			m_transitionRoutine.Replace(this, WaitForTransitionRoutine());
			GameMgr.Events.Dispatch(GameEvents.Dive.LocationChanging, m_currentNode == m_startNode);
			if (m_currentNode != m_startNode) {
				AudioSrcMgr.instance.PlayOneShot("click_node");

                GameMgr.Events.Dispatch(GameEvents.Dive.NavigateToNode, node.name);

				//reset previous observations
				GameMgr.State.CurrentLevel.SetCurrentObservation("");

				// display observation upon arrival at node
				DiveObservationPoint op = m_currentNode.GetObservationPoint();
				if (op != null) {
					GameMgr.State.CurrentLevel.SetCurrentObservation(op.ObservationKey);
					GameMgr.RunTrigger(GameTriggers.Dive.OnObservationMade);
					GameMgr.Events.Dispatch(GameEvents.Dive.ShowMessage, op.ObservationKey);
				}
			}
			AudioSrcMgr.instance.PlayOneShot("swim_to_node");
		}

		private IEnumerator WaitForTransitionRoutine() {
			yield return 4f; // same amount of time as on Cinemachine 'default blend'
			GameMgr.Events.Dispatch(GameEvents.Dive.CameraTransitionComplete);
		}
	}

}