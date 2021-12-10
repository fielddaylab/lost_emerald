using BeauRoutine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shipwreck {

	public class LogoScreen : MonoBehaviour, IPointerClickHandler {

		[SerializeField]
		private Button m_playButton = null;
		[SerializeField]
		private Image m_background = null;
		[SerializeField]
		private Image[] m_logos = null;
		[SerializeField]
		private Transform m_spinner = null;
		[SerializeField]
		private TweenSettings m_tween = new TweenSettings(1f,Curve.QuadInOut);
		[SerializeField]
		private float m_fadeDelay = 3f;

		private Routine m_logoRoutine;
		private Routine m_spinRoutine;
		private bool m_skip = false;
		private static readonly Color INVISIBLE = new Color(1f, 1f, 1f, 0f);

		public void OnPointerClick(PointerEventData eventData) {
			if (!m_playButton.isActiveAndEnabled) {
				m_skip = true;
			}
		}

		private void Awake() {
			foreach (Image logo in m_logos) {
				logo.gameObject.SetActive(false);
			}
			m_playButton.onClick.AddListener(HandlePlayButton);
		}
		private void HandlePlayButton() {
			m_logoRoutine.Replace(this, LogoRoutine());
			m_playButton.onClick.RemoveListener(HandlePlayButton);
			m_playButton.gameObject.SetActive(false);
		}

		private IEnumerator LogoRoutine() {
			yield return m_background.ColorTo(Color.white, 1f, ColorUpdate.FullColor);
			foreach (Image logo in m_logos) {
				logo.color = INVISIBLE;
				logo.gameObject.SetActive(true);
				yield return logo.ColorTo(Color.white, m_tween.Time * (m_skip ? 0.5f : 1f), ColorUpdate.FullColor).Ease(m_tween.Curve);
				float delay = m_fadeDelay;
				while (delay > 0f) {
					delay -= Time.deltaTime * (m_skip ? 20f : 1f);
					yield return null;
				}
				yield return logo.ColorTo(INVISIBLE, m_tween.Time * (m_skip ? 0.5f : 1f), ColorUpdate.FullColor).Ease(m_tween.Curve);

				m_skip = false;
			}
			m_spinner.gameObject.SetActive(true);
			m_spinRoutine.Replace(this, SpinRoutine());
			yield return m_background.ColorTo(INVISIBLE, 1f, ColorUpdate.FullColor);
			AsyncOperation operation = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
			operation.allowSceneActivation = true;
		}

		private IEnumerator SpinRoutine() {
			while (true) {
				m_spinner.Rotate(Vector3.forward, -360f * Time.deltaTime);
				yield return null;
			}
		}


	}

}


