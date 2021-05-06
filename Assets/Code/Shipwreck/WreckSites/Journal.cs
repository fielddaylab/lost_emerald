using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    public float hidePosition = 400f;
    public GameObject journalIcon;
    
    private Vector3 initialPosition;
    private float animateMultiplier = 1f;
    private bool coroutineAllowed;

    // Start is called before the first frame update
    void Start()
    {
        coroutineAllowed = true;
        // initialPosition = this.transform.position;
    }

    void OnEnable()
    {
        // if(coroutineAllowed) {
        //     StartCoroutine(OpenCloseJournal());
        // }
        initialPosition = this.gameObject.transform.position;
        journalIcon.gameObject.SetActive(false);
        this.gameObject.transform.position = new Vector3(initialPosition.x - 400f, initialPosition.y, 0);
        StartCoroutine(RemoveAfterSeconds(10f, gameObject));
    }
    // private IEnumerator HideShowButton(float seconds)
    // {
    //     yield return new WaitForSeconds(seconds);
    //     // obj.SetActive(false);
    // }

    private IEnumerator RemoveAfterSeconds(float seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
        journalIcon.SetActive(true);
    }

    // private IEnumerator OpenCloseJournal() {
    //     // yield return HideShowButton(2f);
    //     coroutineAllowed = false;
    //     animateMultiplier *= -1f;
    //     for (float i = 0f; i <= hidePosition/0.5f; i+= 1f) {
    //         button.transform.position = new Vector3(button.transform.position.x + 0.5f * animateMultiplier, button.transform.position.y, button.transform.position.z);
    //         yield return new WaitForSeconds(0.01f);
    //     }
    //     coroutineAllowed = true;

    // }

    void OnDisable()
    {
        this.gameObject.transform.position = initialPosition;
        // if(coroutineAllowed) {
        //     StartCoroutine(OpenCloseJournal());
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.position.x < initialPosition.x)
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
