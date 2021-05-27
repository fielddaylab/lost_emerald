using BeauRoutine;
using BeauUtil;
using UnityEngine;

namespace Shipwreck {


	public class EvidenceBoard : Singleton<EvidenceBoard> {

		public static YarnNode DroppableNodePrefab {
			get { return I.m_droppableNodePrefab; }
		}

		[SerializeField]
		private float m_raycastDistance = 50f;
		[SerializeField]
		private LayerMask m_dragLayer = 0;
		[SerializeField]
		private LayerMask m_dropLayer = 0;
		[SerializeField]
		private float m_dragIncreaseZ = 4;
		[SerializeField]
		private TweenSettings m_dragTweenSettings = new TweenSettings(0.2f, Curve.QuadOut);
		[SerializeField]
		private YarnNode m_droppableNodePrefab = null;

		private Draggable m_selected;
		private Vector3 m_selectionOffset;
		private Vector3 m_originalPosition;

		private Routine m_routine;


		private void OnEnable() {
			Routine.Settings.DebugMode = false;
			InputMgr.Register(InputMgr.OnInteractPressed, HandleInteractPressed);
			InputMgr.Register(InputMgr.OnInteractReleased, HandleInteractReleased);
		}
		private void OnDisable() {
			InputMgr.Deregister(InputMgr.OnInteractPressed, HandleInteractPressed);
			InputMgr.Deregister(InputMgr.OnInteractReleased, HandleInteractReleased);
		}


		private void HandleInteractPressed() {
			if (m_selected != null) {
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(InputMgr.Position);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, m_raycastDistance, m_dragLayer)) {
				Draggable draggable = hitInfo.collider.GetComponent<Draggable>();
				if (draggable != null) {
					m_selected = draggable;
					m_originalPosition = m_selected.transform.position;
					m_selectionOffset = hitInfo.point - m_selected.transform.position;
					m_routine.Replace(this, Tween.ZeroToOne(SetDragPosition, m_dragTweenSettings));
					
					if (draggable.transform.parent != null && draggable.transform.parent.parent != null) {
						DropZone zone = draggable.transform.parent.parent.GetComponent<DropZone>();
						if (zone != null) {
							zone.Remove(draggable.transform);
						}
					}
					draggable.transform.SetParent(null);
					draggable.OnPickup();
				}
			}
		}
		private void HandleInteractReleased() {
			if (m_selected == null) {
				return;
			}
			
			bool didDrop = false;
			if (m_selected.IsDroppable) {
				Ray ray = Camera.main.ScreenPointToRay(InputMgr.Position);
				if (Physics.Raycast(ray, out RaycastHit hitinfo, m_raycastDistance, m_dropLayer)) {
					DropZone drop = hitinfo.collider.GetComponent<DropZone>();
					if (drop != null) {
						Vector3 position = drop.Attach(m_selected.transform);
						m_routine.Replace(this, Tween.OneToZero(SetDragPosition, m_dragTweenSettings))
							.OnComplete(OnSetDropComplete).OnStop(OnSetDropComplete);
						m_originalPosition = position;
						didDrop = true;
					}
				}
			}
			if (!didDrop) {
				m_routine.Replace(this, Tween.OneToZero(SetDragPosition, m_dragTweenSettings))
					.OnComplete(OnSetDropComplete).OnStop(OnSetDropComplete);
				// need to place object based on offset and mouse position
				// to avoid distortion based on camera
				Vector3 world = MouseToWorldPos(InputMgr.Position, -Camera.main.transform.position.z) - m_selectionOffset;
				world.z = m_originalPosition.z;
				m_originalPosition = world;
			}
		}
		private void Update() {
			if (m_selected == null || m_routine) {
				return; // nothing to do right now
			}
			float distance = -Camera.main.transform.position.z - m_dragIncreaseZ;
			m_selected.transform.position = MouseToWorldPos(InputMgr.Position, distance) - m_selectionOffset;
		}

		private void SetDragPosition(float value) {
			float distance = -Camera.main.transform.position.z - m_dragIncreaseZ;
			m_selected.transform.position = Vector3.Lerp(m_originalPosition, MouseToWorldPos(InputMgr.Position, distance) - m_selectionOffset, value);
		}
		private void OnSetDropComplete() {
			m_selected.transform.position = m_originalPosition;
			m_selected = null;
			m_selectionOffset = Vector2.zero;
		}

		private Vector3 MouseToWorldPos(Vector2 mouse, float distance) {
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(
				mouse.x, mouse.y, distance
			));
			
			return worldPos;
		}

	}


}


