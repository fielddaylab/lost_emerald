using UnityEngine;
using Shipwreck;
using BeauPools;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using BeauUtil;

internal class PostItTest : MonoBehaviour {

    public PostItAsset Asset;
    public UILineRenderer Lines;
    public RectTransform Root;
    public TMP_Text Hint;

    private PostItEvaluator m_eval;
    [NonSerialized] private List<Button> m_chain = new List<Button>();
    [NonSerialized] private bool m_incorrect = false;
    [NonSerialized] private bool m_correct = false;

    private void Start()
    {
        m_eval = new PostItEvaluator();
        m_eval.Load(Asset);

        foreach(var button in GetComponentsInChildren<Button>()) {
            Button cached = button;
            cached.onClick.AddListener(() => OnButtonClicked(cached));
        }

        Hint.text = string.Empty;
        RefreshLines();
    }

    private void OnButtonClicked(Button button) {
        int currentIdx = m_chain.IndexOf(button);
        if (currentIdx < 0) {
            if (!m_incorrect && !m_correct) {
                m_chain.Add(button);
            }
        } else if (currentIdx < m_chain.Count - 1) {
            m_chain.RemoveRange(currentIdx + 1, m_chain.Count - 1 - currentIdx);
        } else {
            m_chain.RemoveAt(currentIdx);
        }

        RefreshHint();
        RefreshLines();
    }

    private void RefreshHint() {
        StringHash32[] chain = ArrayUtils.MapFrom(m_chain, (b) => new StringHash32(b.name));
        PostItData data = m_eval.Evaluate(Root.name, chain);
        if (data != null) {
            Hint.text = data.Text;
            m_incorrect = data.Response == PostItData.ResponseType.Incorrect;
            m_correct = data.Response == PostItData.ResponseType.Correct;
        } else if (chain.Length < 2) {
            Hint.text = string.Empty;
            m_incorrect = false;
            m_correct = false;
        }
    }

    private void RefreshLines() {
        Vector2[] positions = new Vector2[m_chain.Count + 1];
        positions[0] = Root.localPosition;
        for(int i = 1; i < positions.Length; i++) {
            positions[i] = m_chain[i - 1].transform.localPosition;
        }
        Lines.Points = positions;
        Lines.color = m_correct ? ColorBank.Aqua : (m_incorrect ? Color.red : Color.yellow);
    }
}