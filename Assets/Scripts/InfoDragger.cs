using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCP;
using UnityEngine.EventSystems;

public class InfoDragger : MonoBehaviour
{
    public GameObject[] documents;
    public string[] documentNames;
    public InfoDropTarget[] targets;
    public GameObject infoChunkTemplate;
    public Image evidenceTop;
    public Image evidenceBottom;

    private GameObject draggingObject;
    private ScrollRect draggedFromScroll;
    private Vector3 dragStartMouse;
    private Vector3 dragStartInfo;
    private TMP_LinkInfo linkInfo;
    private string documentName;

    // Start is called before the first frame update
    void Start()
    {
        int documentIndex = 0;
        foreach (var document in documents)
        {
            int thisDocumentIndex = documentIndex;
            foreach (var infoPiece in document.GetComponentsInChildren<TextMeshProUGUI>())
            {
                PointerListener pointer = infoPiece.GetComponent<PointerListener>();
                if (pointer)
                {
                    pointer.onPointerDown.AddListener((pdata) =>
                    {
                        int linkIndex = TMP_TextUtilities.FindIntersectingLink(infoPiece, pdata.position, null);
                        if (linkIndex != -1)
                        {
                            linkInfo = infoPiece.textInfo.linkInfo[linkIndex];

                            draggingObject = Instantiate(infoChunkTemplate, transform, true);
                            RectTransform draggingRect = draggingObject.GetComponent<RectTransform>();
                            draggingRect.anchorMin = new Vector2(0f, 1f);
                            draggingRect.anchorMax = new Vector2(0f, 1f);
                            draggingRect.pivot = new Vector2(0f, 0f);
                            draggingRect.position = Input.mousePosition;
                            TextMeshProUGUI draggingText = draggingObject.GetComponentInChildren<TextMeshProUGUI>();
                            draggingText.color = Color.black;
                            draggingText.text = linkInfo.GetLinkText();
                            draggedFromScroll = infoPiece.GetComponentInParent<ScrollRect>();
                            if (draggedFromScroll)
                            {
                                draggedFromScroll.enabled = false;
                            }
                            dragStartMouse = Input.mousePosition;
                            dragStartInfo = draggingRect.position;
                            draggingObject.SetActive(true);
                            documentName = documentNames[thisDocumentIndex];
                        }
                    });
                    pointer.onPointerUp.AddListener(ReleaseDrag);
                }
            }
            documentIndex++;
        }
    }

    void ReleaseDrag(PointerEventData pdata)
    {
        if (draggedFromScroll)
        {
            draggedFromScroll.enabled = true;
        }

        foreach (var target in targets)
        {
            if (DroppingInto(target.gameObject))
            {
                string infoKey = linkInfo.GetLinkID();
                if (infoKey == target.correctInfoKey)
                {
                    var entry = new PlayerProgress.InfoEntry
                    {
                        infoKey = infoKey,
                        infoDisplay = draggingObject.GetComponentInChildren<TextMeshProUGUI>().text,
                        sourceDisplay = documentName
                    };
                    PlayerProgress.instance?.DropInfo(target, entry);
                }
                else
                {
                    PlayerProgress.instance?.TemporaryBubble("Hmm… how do I know that's correct?");
                }
                break;
            }
        }

        if (draggingObject)
        {
            Destroy(draggingObject.gameObject);
        }
        draggingObject = null;
        draggedFromScroll = null;
    }

    private bool DroppingInto(GameObject target)
    {
        if (!draggingObject)
        {
            return false;
        }
        RectTransform targetRect = target.GetComponent<RectTransform>();
        Vector2 localMouse = targetRect.InverseTransformPoint(Input.mousePosition);
        return targetRect.rect.Contains(localMouse) && target.activeInHierarchy;
    }

    private void UpdateTarget(Image targetBG)
    {
        targetBG.color = DroppingInto(targetBG.gameObject) ? Color.gray : Color.white;
    }

    // Update is called once per frame
    void Update()
    {
        if (draggingObject)
        {
            draggingObject.GetComponentInChildren<RectTransform>().position = dragStartInfo + (Input.mousePosition - dragStartMouse);
        }
        foreach (var target in targets)
        {
            Image targetBG = target.GetComponentInParent<Image>();
            if (targetBG)
            {
                UpdateTarget(targetBG);
            }
        }
        UpdateTarget(evidenceTop);
        UpdateTarget(evidenceBottom);
    }
}
