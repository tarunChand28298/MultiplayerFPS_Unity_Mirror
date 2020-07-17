using UnityEngine.UI;
using UnityEngine;

public class PlayerStateUIScript : MonoBehaviour
{
    public Text bulletsText;
    public RectTransform healthBarAmount;

    public void SetBulletsText(int inMag, int spare)
    {
        bulletsText.text = $"{inMag}/{spare}";
    }

    public void SetHealthText(int amount)
    {
        float healthPercent = (float)amount / 100.0f;
        var currentScale = healthBarAmount.localScale;
        healthBarAmount.localScale = new Vector3(healthPercent, currentScale.y, currentScale.z);
    }

}
