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
        for (int i = 0; i < tabs.Length; i++)
        {
            int j = i;
            content[j].SetActive(j == 0);
            tabs[j].onClick.AddListener((pdata) =>
            {
                for (int k = 0; k < tabs.Length; k++)
                {
                    content[k].SetActive(j == k);
                }
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
