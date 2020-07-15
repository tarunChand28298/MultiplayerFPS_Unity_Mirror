using UnityEngine.UI;
using UnityEngine;

public class PlayerStateUIScript : MonoBehaviour
{
    public Text bulletsText;
    public Text healthText;

    public void SetBulletsText(int inMag, int spare)
    {
        bulletsText.text = $"{inMag}/{spare}";
    }

    public void SetHealthText(int amount)
    {
        healthText.text = amount.ToString();
    }

}
