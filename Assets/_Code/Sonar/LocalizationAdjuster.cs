using PotatoLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LocalizationAdjuster : MonoBehaviour
{
	[SerializeField]
	private float m_enWidth, m_esWidth;

    private void Start() {
        if (LocalizationMgr.CurrentLanguage.Equals(new LanguageCode("en"))) {
			this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_enWidth);
		}
		else if (LocalizationMgr.CurrentLanguage.Equals(new LanguageCode("es"))) {
			this.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_esWidth);
		}
	}
}
