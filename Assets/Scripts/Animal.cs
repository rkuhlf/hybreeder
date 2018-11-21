using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour {

    private SpriteRenderer body;

    public GameObject animal;
    Vector3 screenSpace;
    Vector3 offset;

    public float breedSize = 0.5f;

    private float chanceToCoin = 0.5f;

    private bool dragging = false;

    public static int instances = 0;

    public bool ui = false;

    public GameObject particles;
    public GameObject spawnEffect;

    private bool walking = false;
    private Coroutine walkRoutine;

    private void Awake()
    {
        instances++;
        StartCoroutine("StartAnimator");
    }

    public void ShowDust()
    {
        Transform leg1 = transform.Find("Body").Find("Leg1");
        Transform leg2 = transform.Find("Body").Find("Leg2");
        if (leg1.childCount == 1)
        {
            BoxCollider2D collider = leg1.GetChild(0).GetComponent<BoxCollider2D>();
            float yPos = collider.bounds.center.y - collider.bounds.extents.y;
            Instantiate(particles, new Vector2(leg1.position.x, yPos), Quaternion.identity).transform.parent = leg1;
        }
        if (leg2.childCount == 1)
        {
            BoxCollider2D collider = leg2.GetChild(0).GetComponent<BoxCollider2D>();
            float yPos = collider.bounds.center.y - collider.bounds.extents.y;
            Instantiate(particles, new Vector2(leg2.position.x, yPos), Quaternion.identity).transform.parent = leg2;
        }

        transform.Find("Body").Find("Leg1").GetChild(1).GetComponent<ParticleSystem>().Play();
        transform.Find("Body").Find("Leg2").GetChild(1).GetComponent<ParticleSystem>().Play();
    }

    void Start()
    {
        Instantiate(spawnEffect, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        body = transform.Find("Body").GetChild(0).GetComponent<SpriteRenderer>();
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().Boop();
    }

    private void OnDestroy()
    {
        Instantiate(spawnEffect, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        if (ui)
            return;
        instances--;
    }

    private void Update()
    {
        if (!ui)
        {
            foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 1000f) * -1;
            }
            // Make sure that the body is behind all of the limbs (visual thing)
            body.sortingOrder -= 1;
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y + 100);

            if (Random.Range(0, 10000) < chanceToCoin * 100)
                Coins.coins++;

            if (Random.Range(0,10000) < 0.35f * 100)
            {
                if (!walking && !dragging) 
                    walkRoutine = StartCoroutine(Walk());
            }
        }
        
    }

    private IEnumerator Walk()
    {
        walking = true;
        int direction = 1;
        if (Random.Range(0, 100) < 50)
            direction = -1;
        GetComponent<Animator>().SetBool("Walking", true);
        float startx = transform.position.x;
        float times = Random.Range(50, 200);
        float endx = startx + times * Time.deltaTime * Random.Range(0.8f, 1.3f) * direction;
        for (int i = 0; i < times; i++)
        {
            yield return new WaitForEndOfFrame();
            transform.position = new Vector3(Mathf.Lerp(startx, endx, i / times), transform.position.y, transform.position.z);
            transform.localScale = new Vector3(direction, 1);
        }

        for (int i = 0; i < times; i++)
        {
            yield return new WaitForEndOfFrame();
            transform.position = new Vector3(Mathf.Lerp(endx, startx, i / times), transform.position.y, transform.position.z);
            transform.localScale = new Vector3(-direction, 1);
        }
        walking = false;
        GetComponent<Animator>().SetBool("Walking", false);
    }

    public bool AnimalEquals(Transform other)
    {
        bool sameBody = transform.Find("Body").GetChild(0).GetComponent<SpriteRenderer>().sprite == other.Find("Body").GetChild(0).GetComponent<SpriteRenderer>().sprite;
        bool sameHead = transform.Find("Body").Find("Head").GetChild(0).GetComponent<SpriteRenderer>().sprite == other.Find("Body").Find("Head").GetChild(0).GetComponent<SpriteRenderer>().sprite;
        bool sameLeg1 = transform.Find("Body").Find("Leg1").GetChild(0).GetComponent<SpriteRenderer>().sprite == other.Find("Body").Find("Leg1").GetChild(0).GetComponent<SpriteRenderer>().sprite;
        bool sameLeg2 = transform.Find("Body").Find("Leg2").GetChild(0).GetComponent<SpriteRenderer>().sprite == other.Find("Body").Find("Leg2").GetChild(0).GetComponent<SpriteRenderer>().sprite;
        return sameBody && sameHead && sameLeg1 && sameLeg2;
    }

    private void Sell()
    {
        Coins.coins += 3;
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().Ching();
        DestroyImmediate(gameObject);
    }

    public void SetAsUI()
    {
        instances--;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.localScale = Vector3.one * 35;
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer renderer in renderers)
        {
            renderer.sortingOrder = 1;
            renderer.sortingLayerName = "UI";
        }

        ui = true;
    }

    private void OnMouseOver () {
        if (!ui && !ButtonFunctions.ui)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Sell();
            }
        }
        
    }


    private void OnMouseDown()
    {
        if (walking)
        {
            walking = false;
            GetComponent<Animator>().SetBool("Walking", false);
            StopCoroutine(walkRoutine);
        }
        //translate the cubes position from the world to Screen Point
        screenSpace = Camera.main.WorldToScreenPoint(transform.position);

        //calculate any difference between the cubes world position and the mouses Screen position converted to a world point  
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));

    }

    /*
    OnMouseDrag is called when the user has clicked on a GUIElement or Collider and is still holding down the mouse.
    OnMouseDrag is called every frame while the mouse is down.
    */

    private void OnMouseDrag()
    {
        if (!ui && !ButtonFunctions.ui)
        {
            //keep track of the mouse position
            Vector3 curScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);

            //convert the screen mouse position to world point and adjust with offset
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenSpace) + offset;

            //update the position of the object in the world
            transform.position = curPosition;

            dragging = true;

        }
    }

    private void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, breedSize);
            foreach (Collider2D collider in overlaps) {
                if (collider.CompareTag("Animal"))
                {
                    if (collider.gameObject != gameObject)
                    {
                        InstantiateNewAnimal(collider.gameObject);
                    }
                }
            }
        }
    }


    private void InstantiateNewAnimal(GameObject other)
    {
        if (instances >= 15)
        {
            GameObject.FindGameObjectWithTag("TooManyAnimals").GetComponent<TooManyAnimals>().Show();
            return;
        }
        GameObject newAnimal = Instantiate(animal, transform.position, Quaternion.identity);
        Animal newScript = newAnimal.GetComponent<Animal>();
        if (Random.Range(0, 100) < 50)
            newScript.SetBody(transform.Find("Body").gameObject);
        else
            newScript.SetBody(other.transform.Find("Body").gameObject);
        if (Random.Range(0, 100) < 50)
            newScript.SetLeg1(transform.Find("Body").Find("Leg1").GetChild(0).gameObject);
        else
            newScript.SetLeg1(other.transform.Find("Body").Find("Leg1").GetChild(0).gameObject);
        if (Random.Range(0, 100) < 50)
            newScript.SetLeg2(transform.Find("Body").Find("Leg2").GetChild(0).gameObject);
        else
            newScript.SetLeg2(other.transform.Find("Body").Find("Leg2").GetChild(0).gameObject);
        if (Random.Range(0, 100) < 50)
            newScript.SetHead(transform.Find("Body").Find("Head").GetChild(0).gameObject);
        else
            newScript.SetHead(other.transform.Find("Body").Find("Head").GetChild(0).gameObject);

        
        Instantiate(newAnimal, transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0), Quaternion.identity);
        Destroy(newAnimal);
    }

    public void SetBody(GameObject body)
    {
        while (transform.childCount != 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
        Transform newBody = Instantiate(body, body.transform.localPosition + transform.position, body.transform.localRotation).transform;
        
        newBody.parent = transform;
        newBody.localScale = Vector3.one;
        newBody.name = "Body";
    }

    public void SetLeg1(GameObject leg)
    {
        for (int i = transform.Find("Body").Find("Leg1").childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.Find("Body").Find("Leg1").GetChild(i).gameObject);
        }

        Transform newLeg = Instantiate(leg, Vector3.zero, Quaternion.identity).transform;
        newLeg.parent = transform.Find("Body").Find("Leg1");
        newLeg.localRotation = leg.transform.localRotation;
        newLeg.localPosition = leg.transform.localPosition;
        newLeg.localScale = Vector3.one;
    }

    public void SetLeg2(GameObject leg)
    {
        for (int i = transform.Find("Body").Find("Leg2").childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.Find("Body").Find("Leg2").GetChild(i).gameObject);
        }
            
        Transform newLeg = Instantiate(leg, Vector3.zero, Quaternion.identity).transform;
        newLeg.parent = transform.Find("Body").Find("Leg2");
        newLeg.localRotation = leg.transform.localRotation;
        newLeg.localPosition = leg.transform.localPosition;
        newLeg.localScale = Vector3.one;
    }

    public void SetHead(GameObject head)
    {
        for (int i = transform.Find("Body").Find("Head").childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.Find("Body").Find("Head").GetChild(i).gameObject);
        }

        Transform newHead = Instantiate(head, Vector3.zero, Quaternion.identity).transform;
        newHead.parent = transform.Find("Body").Find("Head");
        newHead.localRotation = head.transform.localRotation;
        newHead.localPosition = head.transform.localPosition;
        newHead.localScale = Vector3.one;
    }

    private IEnumerator StartAnimator()
    {
        GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(0.4f);

        GetComponent<Animator>().enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, breedSize);
    }
}
