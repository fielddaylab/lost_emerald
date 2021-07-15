using UnityEngine;
using Shipwreck;
using Leaf;
using UnityEngine.UI;
using BeauUtil;

public class ScriptTest : MonoBehaviour {
    public LeafAsset Script;
    public Button NotifyButton;
    public SerializedHash32 Id;

    private void Start()
    {
        GameMgr.LoadScript(Script);
        GameMgr.RunTrigger("Start");

        //NotifyButton.onClick.AddListener(() => GameMgr.RunTrigger("Special"));
    }
}