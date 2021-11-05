using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shipwreck {

	

	public class UIMgr : BeauUtil.Singleton<UIMgr> {

		[SerializeField]
		private UIBase m_defaultScreen = null;
		[SerializeField]
		private UIBase[] m_screens = null;


		private HashSet<IUIScreen> m_opened = new HashSet<IUIScreen>();

		private Dictionary<Type, IUIScreen> m_mapByType;

		private HashSet<IUIScreen> m_recorded = new HashSet<IUIScreen>();

		protected override void OnAssigned() {
			base.OnAssigned();
			m_mapByType = new Dictionary<Type, IUIScreen>();
			foreach (UIBase screen in m_screens) {
				m_mapByType.Add(screen.GetType(), screen.GetScreen());
			}
		}

		private void Start() {
			foreach(var screen in m_screens) {
				screen.gameObject.SetActive(false);
			}
			
			if (m_defaultScreen != null) {
				Open(m_defaultScreen.GetType());
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			Destroy(gameObject);
		}

		/// <summary>
		/// Opens the given screen
		/// </summary>
		public static T Open<T>() where T : UIBase {
			return (T) I.Open(typeof(T)).Component;
		}
		public static void Close(UIBase screen) {
			I.Close(screen.GetType());
		}
		public static void Close<T>() where T : UIBase {
			I.Close(typeof(T));
		}
		public static void CloseImmediately<T>() where T : UIBase {
			I.CloseImmediately(typeof(T));
		}
		public static void CloseThenCall<T>(Action callback, bool invokeIfAlreadyClosed = true) where T : UIBase {
			I.CloseThenCall(typeof(T), callback, invokeIfAlreadyClosed);
		}
		public static U CloseThenOpen<T, U>() where T : UIBase where U : UIBase {
			return (U) I.CloseThenOpen(typeof(T), typeof(U)).Component;
		}
		public static void CloseAll() {
			foreach (IUIScreen screen in I.m_opened) {
				screen.Hide();
			}
			I.m_opened.Clear();
		}
		public static bool IsOpen<T>() {
			return I.IsOpen(typeof(T));
		}
		public static void RecordState() {
			I.m_recorded = new HashSet<IUIScreen>(I.m_opened);
		}
		public static void RestoreRecordedState() {
			foreach (IUIScreen screen in I.m_recorded) {
				if (I.m_opened.Add(screen)) {
					screen.Show();
				}
			}
			foreach (IUIScreen screen in I.m_opened) {
				if (!I.m_recorded.Contains(screen)) {
					I.m_opened.Remove(screen);
					screen.Hide();
				}
			}
		}

		private IUIScreen Open(Type type) {
			IUIScreen screen = m_mapByType[type];
			if (m_opened.Add(screen)) {
				screen.Show();
			}
			return screen;
		}
		private bool IsOpen(Type type) {
			IUIScreen screen = m_mapByType[type];
			return m_opened.Contains(screen);
		}
		private void Close(Type type) {
			IUIScreen screen = m_mapByType[type];
			if (m_opened.Remove(screen)) {
				screen.Hide();
			}
		}
		private void CloseImmediately(Type type) {
			IUIScreen screen = m_mapByType[type];
			if (m_opened.Remove(screen)) {
				screen.HideImmediate();
			}
		}
		private void CloseThenCall(Type type, Action callback, bool invokeIfAlreadyClosed) {
			IUIScreen screen = m_mapByType[type];
			if (m_opened.Remove(screen)) {
				Action wrapped = null;
				wrapped = () => {
					callback();
					screen.OnCloseComplete -= wrapped;
				};
				screen.OnCloseComplete += wrapped;
				screen.Hide();
			} else if (invokeIfAlreadyClosed) {
				callback();
			}
		}
		private IUIScreen CloseThenOpen(Type toClose, Type toOpen) {
			IUIScreen close = m_mapByType[toClose];
			IUIScreen open = m_mapByType[toOpen];
			if (m_opened.Remove(close)) {
				Action wrapped = null;
				wrapped = () => {
					if (m_opened.Add(open)) {
						open.Show();
					}
					close.OnCloseComplete -= wrapped;
				};
				close.OnCloseComplete += wrapped;
				close.Hide();
			} else if(m_opened.Add(open)) {
				open.Show();
			}

			return open;
		}


		#if UNITY_EDITOR

		[UnityEditor.CustomEditor(typeof(UIMgr))]
		private class Inspector : UnityEditor.Editor {
			public override void OnInspectorGUI() {
				base.OnInspectorGUI();
				UnityEditor.EditorGUILayout.Space();
				if (GUILayout.Button("Refresh Screens List")) {
					UIMgr mgr = (UIMgr) target;
					mgr.m_screens = mgr.GetComponentsInChildren<UIBase>(true);
					UnityEditor.EditorUtility.SetDirty(mgr);
				}
			}
		}

		#endif // UNITY_EDITOR

	}

}
