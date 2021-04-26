using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoCP;
using Shipwreck;

public class TabPanel : MonoBehaviour
{
    public PointerListener[] tabs;
    public GameObject[] content;

    // Start is called before the first frame update
    void Start()
    {
        SelectTab(0);
        for (int i = 0; i < tabs.Length; i++)
        {
            int j = i;
            PointerListener tab = tabs[j];
            tab.onClick.AddListener((pdata) =>
            {
                if (tab.GetComponent<Button>().interactable)
                {
                    SelectTab(j);
                }
            });
        }
    }

    public void SelectTab(int i)
    {
        Logging.instance?.LogViewTab(content[i].name);
        for (int j = 0; j < tabs.Length; j++)
        {
            Image tabImage = tabs[j].GetComponent<Image>();
            if (content[j] == content[i])
            {
                content[j].SetActive(true);
                PlayerProgress.instance.UnlockTabEvidenceBuilder(content[j].name);
                content[j].GetComponent<LogWhenViewed>()?.LogView();
                if (tabImage)
                {
                    tabImage.color = new Color(0.0f, 1.0f, 1.0f);
                }
            }
            else
            {
                content[j].SetActive(false);
                if (tabImage)
                {
                    tabImage.color = new Color(1.0f, 1.0f, 1.0f);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
