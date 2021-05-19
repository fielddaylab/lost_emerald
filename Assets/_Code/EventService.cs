﻿using System;
using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using BeauUtil;
using UnityEngine;


namespace Shipwreck {
	public class EventService {
		#region Types

		private class HandlerBlock {
			private RingBuffer<Handler> m_Actions = new RingBuffer<Handler>(8, RingBufferMode.Expand);
			private bool m_DeletesQueued;
			private int m_ExecutionDepth;
			private StringHash32 m_EventId;

			public HandlerBlock(StringHash32 inEventId) {
				m_EventId = inEventId;
			}

			public void Invoke(object inContext) {
				if (m_Actions.Count == 0)
					return;

				++m_ExecutionDepth;

				for (int i = m_Actions.Count - 1; i >= 0; --i) {
					ref Handler handler = ref m_Actions[i];
					if (!handler.Invoke(inContext))
						m_DeletesQueued = true;
				}

				if (--m_ExecutionDepth == 0) {
					if (m_DeletesQueued)
						Cleanup();
				}
			}

			public void Add(in Handler inHandler) {
				m_Actions.PushBack(inHandler);
			}

			public void Delete(UnityEngine.Object inObject) {
				for (int i = m_Actions.Count - 1; i >= 0; --i) {
					ref Handler handler = ref m_Actions[i];
					if (handler.Match(inObject)) {
						if (m_ExecutionDepth > 0) {
							handler.Delete = true;
							m_DeletesQueued = true;
						} else {
							m_Actions.FastRemoveAt(i);
						}
					}
				}
			}

			public void Delete(Action inAction) {
				for (int i = m_Actions.Count - 1; i >= 0; --i) {
					ref Handler handler = ref m_Actions[i];
					if (handler.Match(inAction)) {
						if (m_ExecutionDepth > 0) {
							handler.Delete = true;
							m_DeletesQueued = true;
						} else {
							m_Actions.FastRemoveAt(i);
						}
					}
				}
			}

			public void Delete(Action<object> inAction) {
				for (int i = m_Actions.Count - 1; i >= 0; --i) {
					ref Handler handler = ref m_Actions[i];
					if (handler.Match(inAction)) {
						if (m_ExecutionDepth > 0) {
							handler.Delete = true;
							m_DeletesQueued = true;
						} else {
							m_Actions.FastRemoveAt(i);
						}
					}
				}
			}

			public void Delete(MulticastDelegate inCastedAction) {
				for (int i = m_Actions.Count - 1; i >= 0; --i) {
					ref Handler handler = ref m_Actions[i];
					if (handler.Match(inCastedAction)) {
						if (m_ExecutionDepth > 0) {
							handler.Delete = true;
							m_DeletesQueued = true;
						} else {
							m_Actions.FastRemoveAt(i);
						}
					}
				}
			}

			public void Clear() {
				if (m_ExecutionDepth > 0) {
					for (int i = m_Actions.Count - 1; i >= 0; --i) {
						ref Handler handler = ref m_Actions[i];
						handler.Delete = true;
					}
					m_DeletesQueued = true;
				} else {
					m_Actions.Clear();
					m_DeletesQueued = false;
				}
			}

			public int Cleanup() {
				int cleanedUpFromDestroy = 0;

				if (m_ExecutionDepth > 0) {
					m_DeletesQueued = true;
				} else {
					for (int i = m_Actions.Count - 1; i >= 0; --i) {
						bool bindingDestroyed;
						if (m_Actions[i].ShouldDelete(out bindingDestroyed)) {
							if (bindingDestroyed) {
#if UNITY_EDITOR
								UnityEngine.Debug.LogWarningFormat("[EventService] Cleaning up binding on event '{0}' for dead object '{1}'",
									m_EventId.ToDebugString(), m_Actions[i].Name);
#endif // UNITY_EDITOR
								++cleanedUpFromDestroy;
							}
							m_Actions.FastRemoveAt(i);
						}
					}
				}

				return cleanedUpFromDestroy;
			}
		}

		private struct Handler {
			private UnityEngine.Object m_Binding;
			private CastableAction<object> m_Action;

			public string Name;
			public bool Delete;

			public Handler(CastableAction<object> inAction, UnityEngine.Object inBinding) {
				m_Binding = inBinding;
				m_Action = inAction;
				Delete = false;

#if UNITY_EDITOR
				Name = inBinding.IsReferenceNull() ? null : inBinding.name;
#else
                Name = null;
#endif // UNITY_EDITOR
			}

			public bool Match(UnityEngine.Object inBinding) {
				return m_Binding.IsReferenceEquals(inBinding);
			}

			public bool Match(Action inAction) {
				return m_Action.Equals(inAction);
			}

			public bool Match(Action<object> inActionWithContext) {
				return m_Action.Equals(inActionWithContext);
			}

			public bool Match(MulticastDelegate inActionWithCastedContext) {
				return m_Action.Equals(inActionWithCastedContext);
			}

			public bool Invoke(object inContext) {
				bool discard;
				if (ShouldDelete(out discard))
					return false;

				m_Action.Invoke(inContext);
				return !Delete;
			}

			public bool ShouldDelete(out bool outReferenceDestroyed) {
				outReferenceDestroyed = m_Binding.IsReferenceDestroyed();
				return Delete || outReferenceDestroyed;
			}
		}

		#endregion // Types

		#region Inspector

		[SerializeField] private float m_CleanupInterval = 30;

		#endregion // Inspector

		private Routine m_CleanupRoutine;
		private readonly Dictionary<StringHash32, HandlerBlock> m_Handlers = new Dictionary<StringHash32, HandlerBlock>(64);

		#region Registration

		/// <summary>
		/// Registers an event handler, optionally bound to a given object.
		/// </summary>
		public EventService Register(StringHash32 inEventId, Action inAction, UnityEngine.Object inBinding = null) {
			HandlerBlock block;
			if (!m_Handlers.TryGetValue(inEventId, out block)) {
				block = new HandlerBlock(inEventId);
				m_Handlers.Add(inEventId, block);
			}
			block.Add(new Handler(CastableAction<object>.Create(inAction), inBinding));

			return this;
		}

		/// <summary>
		/// Registers an event handler, optionally bound to a given object.
		/// </summary>
		public EventService Register(StringHash32 inEventId, Action<object> inActionWithContext, UnityEngine.Object inBinding = null) {
			HandlerBlock block;
			if (!m_Handlers.TryGetValue(inEventId, out block)) {
				block = new HandlerBlock(inEventId);
				m_Handlers.Add(inEventId, block);
			}
			block.Add(new Handler(CastableAction<object>.Create(inActionWithContext), inBinding));

			return this;
		}

		/// <summary>
		/// Registers an event handler, optionally bound to a given object.
		/// </summary>
		public EventService Register<T>(StringHash32 inEventId, Action<T> inActionWithCastedContext, UnityEngine.Object inBinding = null) {
			HandlerBlock block;
			if (!m_Handlers.TryGetValue(inEventId, out block)) {
				block = new HandlerBlock(inEventId);
				m_Handlers.Add(inEventId, block);
			}
			block.Add(new Handler(CastableAction<object>.Create(inActionWithCastedContext), inBinding));

			return this;
		}

		/// <summary>
		/// Deregisters an event handler.
		/// </summary>
		public EventService Deregister(StringHash32 inEventId, Action inAction) {
			HandlerBlock block;
			if (m_Handlers.TryGetValue(inEventId, out block)) {
				block.Delete(inAction);
			}

			return this;
		}

		/// <summary>
		/// Deregisters an event handler.
		/// </summary>
		public EventService Deregister(StringHash32 inEventId, Action<object> inActionWithContext) {
			HandlerBlock block;
			if (m_Handlers.TryGetValue(inEventId, out block)) {
				block.Delete(inActionWithContext);
			}

			return this;
		}

		/// <summary>
		/// Deregisters an event handler.
		/// </summary>
		public EventService Deregister<T>(StringHash32 inEventId, Action<T> inActionWithCastedContext) {
			HandlerBlock block;
			if (m_Handlers.TryGetValue(inEventId, out block)) {
				block.Delete(inActionWithCastedContext);
			}

			return this;
		}

		/// <summary>
		/// Deregisters all handlers for the given event.
		/// </summary>
		public EventService DeregisterAll(StringHash32 inEventId) {
			HandlerBlock block;
			if (m_Handlers.TryGetValue(inEventId, out block)) {
				block.Clear();
			}

			return this;
		}

		/// <summary>
		/// Deregisters all handlers associated with the given object.
		/// </summary>
		public EventService DeregisterAll(UnityEngine.Object inBinding) {
			if (inBinding.IsReferenceNull())
				return this;

			foreach (var block in m_Handlers.Values) {
				block.Delete(inBinding);
			}

			return this;
		}

		#endregion // Registration

		#region Operations

		/// <summary>
		/// Dispatches the given event with an optional argument.
		/// </summary>
		public void Dispatch(StringHash32 inEventId, object inContext = null) {
			HandlerBlock block;
			if (m_Handlers.TryGetValue(inEventId, out block)) {
				block.Invoke(inContext);
			}
		}

		/// <summary>
		/// Cleans up all floating handlers.
		/// </summary>
		public void Cleanup() {
			int cleanedUpFromDestroyed = 0;
			foreach (var block in m_Handlers.Values) {
				cleanedUpFromDestroyed += block.Cleanup();
			}

			if (cleanedUpFromDestroyed > 0) {
				Debug.LogWarningFormat("[EventService] Cleaned up {0} event listeners whose bindings were destroyed", cleanedUpFromDestroyed);
			}
		}

		#endregion // Operations

		#region Maintenance

		private IEnumerator MaintenanceRoutine() {
			object delay = m_CleanupInterval;
			while (true) {
				yield return delay;
				Cleanup();
			}
		}

		private void OnSceneLoad(SceneBinding inScene, object inContext) {
			Cleanup();
		}

		#endregion // Maintenance

	}
}