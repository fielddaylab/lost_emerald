using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    private Vector3 initalPosition;
    public GameObject journalIcon;

    // Start is called before the first frame update
    void Start()
    {

    }
    void OnEnable()
    {
        initalPosition = this.gameObject.transform.position;
        this.gameObject.transform.position = new Vector3(initalPosition.x - 400f, initalPosition.y, 0);
        StartCoroutine(RemoveAfterSeconds(10, gameObject));
    }
    IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    void OnDisable()
    {
        this.gameObject.transform.position = initalPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.position.x < initalPosition.x)
        {
            this.gameObject.transform.position += (transform.right * Time.deltaTime * 500f);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = new RaycastHit();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject != this.gameObject)
                    {
                        this.gameObject.SetActive(false);
                        journalIcon.SetActive(true);
                    }
                }
                else
                {
                    //Click outside of any object
                    this.gameObject.SetActive(false);
                    journalIcon.SetActive(true);
                }
            }
        }
    }
}
