using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour {

    public string boolName = "Activated";
    public Animator shop;
    public Animator[] notShop;
    public bool closeShop = false;
    public GameObject audioManager;

    public static bool ui = false;

    public void SetTrue(Animator anim)
    {
        anim.SetBool(boolName, true);
    }

    public void SetFalse(Animator anim)
    {
        anim.SetBool(boolName, false);
    }

    public void ToggleBool(Animator anim)
    {
        anim.SetBool(boolName, !anim.GetBool(boolName));
    }

    public void UpdateMenu(Animator anim)
    {
        ui = !ui;
        // if pause menu is activated
        if (anim.GetBool(boolName))
        {
            // activate shop
            shop.SetBool(boolName, true);
        } else
        {
            // otherwise deactivate all of them
            foreach (Animator a in notShop)
            {
                a.SetBool(boolName, false);   
            }

            shop.SetBool(boolName, false);
        }

    }

    public void ToggleAudio()
    {
        audioManager.GetComponent<AudioManager>().Mute();
    }

    public void Woosh()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().Woosh();
    }

    private void Update()
    {
        if (closeShop)
        {
            if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null && ui)
            {
                GetComponent<Button>().onClick.Invoke();
            }

        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
