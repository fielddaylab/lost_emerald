using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ProtoCP;
using UnityEngine.EventSystems;

public class InfoDragger : MonoBehaviour
{
    public TextMeshProUGUI[] infoPieces;
    public TextMeshProUGUI[] targets;

    private TextMeshProUGUI draggingObject;
    private ScrollRect draggedFromScroll;
    private Vector3 dragStartMouse;
    private Vector3 dragStartInfo;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var infoPiece in infoPieces)
        {
            PointerListener pointer = infoPiece.GetComponent<PointerListener>();
            if (pointer)
            {
                pointer.onPointerDown.AddListener((pdata) =>
                {
                    draggingObject = Instantiate(infoPiece, transform, true);
                    draggingObject.color = Color.red;
                    draggedFromScroll = infoPiece.GetComponentInParent<ScrollRect>();
                    if (draggedFromScroll)
                    {
                        draggedFromScroll.enabled = false;
                    }
                    dragStartMouse = Input.mousePosition;
                    dragStartInfo = draggingObject.rectTransform.position;
                });
                pointer.onPointerUp.AddListener(ReleaseDrag);
            }
        }
    }

    void ReleaseDrag(PointerEventData pdata)
    {
        draggedFromScroll.enabled = true;

        foreach (var target in targets)
        {
            Vector2 localMouse = target.rectTransform.InverseTransformPoint(Input.mousePosition);
            if (target.rectTransform.rect.Contains(localMouse))
            {
                target.text = draggingObject.text;
                break;
            }
        }

        Destroy(draggingObject.gameObject);
        draggingObject = null;
        draggedFromScroll = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (draggingObject)
        {
            draggingObject.rectTransform.position = dragStartInfo + (Input.mousePosition - dragStartMouse);
        }
        foreach (var target in targets)
        {
            Image targetBG = target.GetComponentInParent<Image>();
            if (targetBG)
            {
                if (draggingObject)
                {
                    Vector2 localMouse = target.rectTransform.InverseTransformPoint(Input.mousePosition);
                    targetBG.color = target.rectTransform.rect.Contains(localMouse) ? Color.gray : Color.white;
                }
                else
                {
                    targetBG.color = Color.white;
                }
            }
        }
    }
}
