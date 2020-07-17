using UnityEngine;
using UnityEngine.UI;

public class PlayMenuScript : MonoBehaviour
{
    public InputField hostIPAddressInput;

    public void HostButtonClicked()
    {
        ApplicationManager.Instance().HostGame();
    }

    public void JoinButtonClicked()
    {
        ApplicationManager.Instance().JoinGame(hostIPAddressInput.text);
    }

}
