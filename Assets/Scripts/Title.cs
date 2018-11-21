using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            GetComponent<Animator>().SetTrigger("FloatAway");
        }
    }
}
