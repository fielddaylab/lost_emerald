using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    public bool animate;
    private Vector3 initalPosition;
    // Start is called before the first frame update
    void Start()
    {
        PlayerProgress.instance.SetThoughtBubble(this);
        if (animate)
        {
            initalPosition = this.gameObject.transform.position;
            this.gameObject.transform.position = new Vector3 (initalPosition.x, initalPosition.y - 50f, 0);
        }
        


    }

    // Update is called once per frame
    void Update()
    {
        if (animate && this.gameObject.transform.position.y < initalPosition.y)
        {
            this.gameObject.transform.position += (transform.up * Time.deltaTime * 20f);
        }
    }

    private void OnDestroy()
    {
        PlayerProgress.instance.ClearThoughtBubble(this);
    }
}
