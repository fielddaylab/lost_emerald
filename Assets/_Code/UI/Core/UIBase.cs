using BeauRoutine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Shipwreck {
	public interface IUIScreen {
		event Action OnCloseComplete;
		event Action OnOpenComplete;
		void Show();
		void Hide();
		void HideImmediate();
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
			public void HideImmediate() {
				m_owner.HideImmediate();
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
		public bool IsShowing {
			get { return m_isShown; }
		}
		public bool IsChangingState {
			get { return m_showHideRoutine; }
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
				.OnComplete(HandleShowComplete)
				.OnStop(HandleShowComplete);
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
				.OnComplete(HandlehideCmplete)
				.OnStop(HandlehideCmplete);
		}
		private void HideImmediate() {
			if (!m_isShown) {
				return;
			}
			m_isShown = false;
			gameObject.SetActive(true);
			OnHideStart();
			m_showHideRoutine.Replace(this, HideImmediateRoutine())
				.ExecuteWhileDisabled()
				.OnComplete(HandlehideCmplete)
				.OnStop(HandlehideCmplete);
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

		private void HandleShowComplete() {
			if (!m_isShown)
				return;

			OnShowCompleted();
		}

		private void HandlehideCmplete() {
			if (m_isShown)
				return;

			OnHideCompleted();
		}


		protected abstract IEnumerator ShowRoutine();

		protected abstract IEnumerator HideRoutine();

		protected abstract IEnumerator HideImmediateRoutine();

		static public void AssignSpritePreserveAspect(Image target, Sprite sprite, Axis preserveAspect) {
			target.sprite = sprite;
			if (sprite != null) {
				float aspect = sprite.rect.width / sprite.rect.height;
				if (preserveAspect == Axis.X) {
					target.rectTransform.SetSizeDelta(target.rectTransform.sizeDelta.x / aspect, Axis.Y);
				} else if (preserveAspect == Axis.Y) {
					target.rectTransform.SetSizeDelta(target.rectTransform.sizeDelta.y * aspect, Axis.X);
				} else {
					throw new ArgumentOutOfRangeException("Unaccepted axis value " + preserveAspect);
				}
			}
		}

		static public void AssignSpritePreserveAspect(Image target, LayoutElement layout, Sprite sprite, Axis preserveAspect) {
			target.sprite = sprite;
			if (sprite != null) {
				float aspect = sprite.rect.width / sprite.rect.height;
				if (preserveAspect == Axis.X) {
					layout.preferredHeight = layout.preferredWidth / aspect;
				} else if (preserveAspect == Axis.Y) {
					layout.preferredWidth = layout.preferredHeight * aspect;
				} else {
					throw new ArgumentOutOfRangeException("Unaccepted axis value " + preserveAspect);
				}
			}
		}
	}

}