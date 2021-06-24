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

		protected override void OnAssigned() {
			base.OnAssigned();
			m_mapByType = new Dictionary<Type, IUIScreen>();
			foreach (UIBase screen in m_screens) {
				m_mapByType.Add(screen.GetType(), screen.GetScreen());
			}
			if (m_defaultScreen != null) {
				Open(m_defaultScreen.GetType());
			}
		}

		/// <summary>
		/// Opens the given screen
		/// </summary>
		public static void Open<T>() where T : UIBase {
			I.Open(typeof(T));
		}
		public static void Close(UIBase screen) {
			I.Close(screen.GetType());
		}
		public static void Close<T>() where T : UIBase {
			I.Close(typeof(T));
		}
		public static void CloseThenCall<T>(Action callback, bool invokeIfAlreadyClosed = true) where T : UIBase {
			I.CloseThenCall(typeof(T), callback, invokeIfAlreadyClosed);
		}
		public static void CloseThenOpen<T, U>() where T : UIBase where U : UIBase {
			I.CloseThenOpen(typeof(T), typeof(U));
		}

		private void Open(Type type) {
			IUIScreen screen = m_mapByType[type];
			if (m_opened.Add(screen)) {
				screen.Show();
			}
		}
		private void Close(Type type) {
			IUIScreen screen = m_mapByType[type];
			if (m_opened.Remove(screen)) {
				screen.Hide();
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
		private void CloseThenOpen(Type toClose, Type toOpen) {
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
		}

	}

}
