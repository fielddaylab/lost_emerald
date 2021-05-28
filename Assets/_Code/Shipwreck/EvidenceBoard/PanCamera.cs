using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {

	public class PanCamera : MonoBehaviour {

		[SerializeField]
		private Vector2 m_boundsSize = new Vector2(10, 10);
		[SerializeField]
		private float m_panAmount = 2.5f;
		[SerializeField]
		private TweenSettings m_tweenSettings = new TweenSettings(0.3f, Curve.QuadInOut);

		[SerializeField]
		private Button m_buttonUp = null;
		[SerializeField]
		private Button m_buttonDown = null;
		[SerializeField]
		private Button m_buttonLeft = null;
		[SerializeField]
		private Button m_buttonRight = null;

		private Vector3 m_initialPosition;
		private Vector3 m_goalPosition;
		private Routine m_routine;

		private void Awake() {
			m_initialPosition = transform.position;
			m_goalPosition = m_initialPosition;
			m_buttonDown.onClick.AddListener(HandleButtonDown);
			m_buttonLeft.onClick.AddListener(HandleButtonLeft);
			m_buttonRight.onClick.AddListener(HandleButtonRight);
			m_buttonUp.onClick.AddListener(HandleButtonUp);
		}

		private void HandleButtonUp() {
			m_goalPosition += Vector3.up * m_panAmount;
			MoveToGoal();
		}
		private void HandleButtonDown() {
			m_goalPosition += Vector3.down * m_panAmount;
			MoveToGoal();
		}
		private void HandleButtonRight() {
			m_goalPosition += Vector3.right * m_panAmount;
			MoveToGoal();
		}
		private void HandleButtonLeft() {
			m_goalPosition += Vector3.left * m_panAmount;
			MoveToGoal();
		}

		private void MoveToGoal() {
			m_goalPosition = new Vector3(
				Mathf.Clamp(m_goalPosition.x, m_initialPosition.x - m_boundsSize.x, m_initialPosition.x + m_boundsSize.x),
				Mathf.Clamp(m_goalPosition.y, m_initialPosition.y - m_boundsSize.y, m_initialPosition.y + m_boundsSize.y),
				m_initialPosition.z
			) ;
			m_routine.Replace(this, transform.MoveTo(m_goalPosition, m_tweenSettings));
		}

	}


}


