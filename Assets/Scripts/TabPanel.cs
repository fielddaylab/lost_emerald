using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoCP;

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

    private void SelectTab(int i)
    {
        for (int j = 0; j < tabs.Length; j++)
        {
            content[j].SetActive(false);
        }
        for (int j = 0; j < tabs.Length; j++)
        {
            if (i == j)
            {
                content[j].SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
