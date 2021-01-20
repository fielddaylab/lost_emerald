using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress instance;

    public struct InfoEntry
    {
        public string infoKey;
        public string infoDisplay;
        public string sourceDisplay;
    }

    private Dictionary<string, InfoEntry> shipLog = new Dictionary<string, InfoEntry>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void FillInfo(InfoDropTarget target)
    {
        if (shipLog.TryGetValue(target.targetKey, out InfoEntry entry))
        {
            target.infoLabel.text = entry.infoDisplay;
            target.sourceLabel.text = "From: " + entry.sourceDisplay;
        }
        else
        {
            target.infoLabel.text = "Unknown";
            target.sourceLabel.text = "";
        }
    }

    public void DropInfo(InfoDropTarget target, InfoEntry info)
    {
        shipLog.Add(target.targetKey, info);
        FillInfo(target);
    }
}
