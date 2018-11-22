using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    public Animator backStory;

    private int clicks = 0;

    private void Update()
    {
        if (clicks < 2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (clicks == 0)
                {
                    GetComponent<Animator>().SetTrigger("FloatAway");
                    backStory.SetTrigger("FloatUp");
                } else if (clicks == 1)
                {
                    backStory.SetTrigger("FloatAway");
                }
                clicks++;
            }
        }
    }
}
