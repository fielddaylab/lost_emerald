using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentButton : MonoBehaviour
{
    public string targetKey;

    // Start is called before the first frame update
    void Start()
    {
        PlayerProgress.instance?.SetDocumentPresence(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
