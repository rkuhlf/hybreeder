using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooManyAnimals : MonoBehaviour {

	public void Show()
    {
        GetComponent<Animator>().SetTrigger("Show");
    }
}
