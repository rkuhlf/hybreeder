using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour {

    public Transform animalImage;
    public Text cost;

    public GameObject[] animals;
    public int[] prices;

    public int index = 0;

    private void Start()
    {
        StartCoroutine(OnSecondFrame());
    }

    private IEnumerator OnSecondFrame()
    {
        yield return 0;
        SetImage();
    }

    public void SetImage()
    {
        foreach (Transform child in animalImage)
        {
            Destroy(child.gameObject);
        }

        Instantiate(animals[index], animalImage).GetComponent<Animal>().SetAsUI();
        cost.text = prices[index].ToString();
    }

    public void Shift(int dir)
    {
        index += dir;
        index = index < 0 ? animals.Length - 1 : index;
        index = index >= animals.Length ? 0 : index;

        SetImage();
    }

    public void Buy()
    {
        if (Animal.instances < 15)
        {
            if (Coins.coins > prices[index])
            {
                GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().Ching();
                Instantiate(animals[index], new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), 0), Quaternion.identity);
                Coins.coins -= prices[index];
            }
        } else
        {
            GameObject.FindGameObjectWithTag("TooManyAnimals").GetComponent<TooManyAnimals>().Show();
        }
    }
}
