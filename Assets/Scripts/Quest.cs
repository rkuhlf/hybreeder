using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Quest : MonoBehaviour {

    public Transform imageHolder;
    public GameObject animal;

    public GameObject[] heads;
    public GameObject[] bodies;
    public GameObject[] leg1s;
    public GameObject[] leg2s;

    public int reward = 500;

    public Button finishedButton;

    private void Start()
    {
        SetNewQuest();
    }

    public void SetNewQuest()
    {
        // Destroy the current image
        for (int i = imageHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(imageHolder.GetChild(i).gameObject);
        }

        GameObject newAnimal = Instantiate(animal, transform.position, Quaternion.identity);
        Animal newScript = newAnimal.GetComponent<Animal>();

        newScript.SetBody(bodies[Random.Range(0, bodies.Length)]);
        newScript.SetLeg1(leg1s[Random.Range(0, leg1s.Length)]);
        newScript.SetLeg2(leg2s[Random.Range(0, leg2s.Length)]);
        newScript.SetHead(heads[Random.Range(0, heads.Length)]);


        Instantiate(newAnimal, imageHolder).GetComponent<Animal>().SetAsUI();
        newAnimal.SetActive(false);
        Destroy(newAnimal);

        StartCoroutine("CheckNext");
    }

    public void CheckIfHasAnimal()
    {
        finishedButton.interactable = false;
        GameObject[] a = GameObject.FindGameObjectsWithTag("Animal");
        List<GameObject> animals = new List<GameObject>();

        foreach (GameObject thisA in a)
        {
            if (!thisA.GetComponent<Animal>().ui)
            {
                animals.Add(thisA);
            }
        }

        foreach (GameObject animal in animals)
        {
            
            if (animal.GetComponent<Animal>().AnimalEquals(imageHolder.GetChild(0)))
            {
                print(animal);
                finishedButton.interactable = true;
            }
        }
    }
    
    private IEnumerator CheckNext()
    {
        yield return new WaitForEndOfFrame();
        CheckIfHasAnimal();
    }

    public void TurnIn()
    {
        GameObject[] a = GameObject.FindGameObjectsWithTag("Animal");
        List<GameObject> animals = new List<GameObject>();

        foreach (GameObject thisA in a)
        {
            

            if (!thisA.GetComponent<Animal>().ui)
            {
                
                animals.Add(thisA);
            }
        }
        

        foreach (GameObject animal in animals)
        {
            
            if (animal.GetComponent<Animal>().AnimalEquals(imageHolder.GetChild(0)))
            {
                
                Destroy(animal);
                SetNewQuest();
                Coins.coins += 200;
                GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().Ching();
                break;
            }
        }

        
    }
}
