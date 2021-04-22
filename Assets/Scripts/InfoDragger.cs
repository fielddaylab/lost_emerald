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
    public EvidenceBuilder evidenceBuilder;
    public PhotoSlot[] photos;

    private GameObject draggingObject;
    private ScrollRect draggedFromScroll;
    private Vector3 dragStartMouse;
    private Vector3 dragStartInfo;
    private string infoKey;
    private string documentName;
    private string infoDisplay;

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
                            TMP_LinkInfo linkInfo = infoPiece.textInfo.linkInfo[linkIndex];
                            infoKey = linkInfo.GetLinkID();

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
                            infoDisplay = draggingObject.GetComponentInChildren<TextMeshProUGUI>().text;
                        }
                    });
                    pointer.onPointerUp.AddListener(ReleaseDrag);
                }
            }
            documentIndex++;
        }

        foreach (var photo in photos)
        {
            PointerListener pointer = photo.GetComponent<PointerListener>();
            if (pointer)
            {
                pointer.onPointerDown.AddListener((pdata) =>
                {
                    if (!PlayerProgress.instance.IsUnlocked(photo.unlockKey))
                    {
                        return;
                    }
                    infoKey = photo.infoKey;

                    draggingObject = Instantiate(photo.gameObject, transform, true);
                    // for now, delete the extra components inside evidence blocks
                    foreach (var item in draggingObject.GetComponentsInChildren<Transform>())
                    {
                        if (item.gameObject != draggingObject)
                        {
                            Destroy(item.gameObject);
                        }
                    }
                    draggingObject.GetComponent<PhotoSlot>().enabled = false;
                    RectTransform draggingRect = draggingObject.GetComponent<RectTransform>();
                    draggingRect.anchorMin = new Vector2(0f, 1f);
                    draggingRect.anchorMax = new Vector2(0f, 1f);
                    draggingRect.pivot = new Vector2(0f, 0f);
                    draggingRect.position = Input.mousePosition;
                    Image originalImage = photo.gameObject.GetComponent<Image>();
                    float imageWidth = 200f;
                    float imageHeight = imageWidth / originalImage.sprite.rect.width * originalImage.sprite.rect.height;
                    draggingRect.sizeDelta = new Vector2(imageWidth, imageHeight);
                    draggedFromScroll = photo.GetComponentInParent<ScrollRect>();
                    if (draggedFromScroll)
                    {
                        draggedFromScroll.enabled = false;
                    }
                    dragStartMouse = Input.mousePosition;
                    dragStartInfo = draggingRect.position;
                    draggingObject.SetActive(true);
                    documentName = "Photo";
                    infoDisplay = photo.infoDisplay;
                });
                pointer.onPointerUp.AddListener(ReleaseDrag);
            }
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
                if (infoKey == target.correctInfoKey)
                {
                    var entry = new PlayerProgress.InfoEntry
                    {
                        infoKey = infoKey,
                        infoDisplay = infoDisplay,
                        sourceDisplay = documentName
                    };
                    PlayerProgress.instance?.DropInfo(target, entry);
                    Logging.instance.LogUpdateShipOverview("loretta", target.targetKey, infoKey, infoDisplay, documentName);
                }
                else
                {
                    if (infoKey == "coords")
                    {
                        PlayerProgress.instance?.TemporaryBubble("Whoops, wrong spot.");
                    }
                    else
                    {
                        PlayerProgress.instance?.TemporaryBubble("Hmm… how do I know that's correct?");
                    }
                }
                break;
            }
        }

        if (draggingObject)
        {
            Image draggingImage = draggingObject.GetComponent<Image>();
            Sprite sprite = draggingImage?.sprite;
            if (DroppingInto(evidenceBuilder.slotTop.gameObject))
            {
                evidenceBuilder.SetSlot(EvidenceBuilder.Slot.SLOT_TOP, sprite, infoKey);
            }
            else if (DroppingInto(evidenceBuilder.slotBottom.gameObject))
            {
                evidenceBuilder.SetSlot(EvidenceBuilder.Slot.SLOT_BOTTOM, sprite, infoKey);
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
        UpdateTarget(evidenceBuilder.slotTop);
        UpdateTarget(evidenceBuilder.slotBottom);
    }
}
