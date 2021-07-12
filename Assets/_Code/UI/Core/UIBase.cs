using BeauRoutine;
using System;
using System.Collections;
using UnityEngine;

namespace Shipwreck {
	public interface IUIScreen {
		event Action OnCloseComplete;
		event Action OnOpenComplete;
		void Show();
		void Hide();
		MonoBehaviour Component { get; }
	}
	public abstract class UIBase : MonoBehaviour {

		private class MgrLink : IUIScreen {

			public event Action OnOpenComplete {
				add { m_owner.OnShowComplete += value; }
				remove { m_owner.OnShowComplete -= value; }
			}
			public event Action OnCloseComplete {
				add { m_owner.OnHideComplete += value; }
				remove { m_owner.OnHideComplete -= value; }
			}

			private UIBase m_owner;

			public MgrLink(UIBase owner) {
				m_owner = owner;
			}
			public void Show() {
				m_owner.Show();
			}
			public void Hide() {
				m_owner.Hide();
			}

			public MonoBehaviour Component {
				get { return m_owner; }
			}
		}

		public event Action OnShowComplete;
		public event Action OnHideComplete;

		protected CanvasGroup CanvasGroup {
			get { return m_canvasGroup; }
		}

		[SerializeField]
		private CanvasGroup m_canvasGroup = null;

		private Routine m_showHideRoutine;
		private bool m_isShown = false;


		public IUIScreen GetScreen() {
			return new MgrLink(this);
		}

		private void Show() {
			if (m_isShown) {
				return;
			}
			m_isShown = true;
			gameObject.SetActive(true);
			OnShowStart();
			m_showHideRoutine.Replace(this, ShowRoutine())
				.ExecuteWhileDisabled()
				.OnComplete(OnShowCompleted)
				.OnStop(OnShowCompleted);
		}
		private void Hide() {
			if (!m_isShown) {
				return;
			}
			m_isShown = false;
			gameObject.SetActive(true);
			OnHideStart();
			m_showHideRoutine.Replace(this, HideRoutine())
				.ExecuteWhileDisabled()
				.OnComplete(OnHideCompleted)
				.OnStop(OnHideCompleted);
		}

		protected virtual void OnShowStart() {
			if (m_canvasGroup != null) {
				m_canvasGroup.interactable = false;
			}
		}
		protected virtual void OnHideStart() {
			if (m_canvasGroup != null) {
				m_canvasGroup.interactable = false;
			}
		}
		protected virtual void OnShowCompleted() {
			if (m_canvasGroup != null) {
				m_canvasGroup.interactable = true;
			}
			OnShowComplete?.Invoke();
		}
		protected virtual void OnHideCompleted() {
			gameObject.SetActive(false);
			OnHideComplete?.Invoke();
		}


		protected abstract IEnumerator ShowRoutine();

		protected abstract IEnumerator HideRoutine();

	}

}