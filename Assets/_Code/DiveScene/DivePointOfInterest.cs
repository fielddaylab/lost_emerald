using UnityEngine;
using BeauUtil;
using PotatoLocalization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Shipwreck {

	public class DivePointOfInterest : MonoBehaviour {

		public StringHash32 EvidenceUnlock {
			get { return m_evidenceUnlock; }
		}
		public float ZoomMin {
			get { return m_zoomMin; }
		}
		public float ZoomMax {
			get { return m_zoomMax; }
		}
		public LocalizationKey UnlockMessage {
			get { return m_unlockMessage; }
		}
		public LocalizationKey PhotoName {
			get { return m_photoName; }
		}

		[SerializeField]
		private SerializedHash32 m_evidenceUnlock = StringHash32.Null;
		[SerializeField]
		private LocalizationKey m_unlockMessage = LocalizationKey.Empty;
		[SerializeField]
		private LocalizationKey m_photoName = LocalizationKey.Empty;
		[SerializeField]
		private float m_zoomMin = 0f;
		[SerializeField]
		private float m_zoomMax = 1f;

		#if UNITY_EDITOR
		
		[CustomEditor(typeof(DivePointOfInterest))]
		private class Editor : UnityEditor.Editor {

			public override void OnInspectorGUI() {

				DivePointOfInterest component = target as DivePointOfInterest;
				float min = component.m_zoomMin;
				float max = component.m_zoomMax;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_evidenceUnlock"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_unlockMessage"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_photoName"));

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(new GUIContent("Zoom Range"));
				EditorGUILayout.LabelField(min.ToString("0.000"), GUILayout.Width(40f));
				EditorGUILayout.MinMaxSlider(ref min, ref max, 0f, 1f);
				EditorGUILayout.LabelField(max.ToString("0.000"), GUILayout.Width(40f));
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(target, "modified point of interest");
					component.m_zoomMax = max;
					component.m_zoomMin = min;
					serializedObject.ApplyModifiedProperties();
				}
			}

		}

		#endif

	}

}