using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour {

    public static int coins = 0;
    public Text coinText;

    private void Update()
    {
        coinText.text = coins.ToString();
    }
}
