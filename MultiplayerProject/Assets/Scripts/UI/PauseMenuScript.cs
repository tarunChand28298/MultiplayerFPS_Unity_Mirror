using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    public bool isHosting;

    public void OnApplicationQuitButtonClicked()
    {
        Application.Quit();
    }

    public void OnDisconnectButtonClicked()
    {
        if (isHosting)
        {
            NetworkManagerMultiplayer.singleton.StopHost();
        }
        else
        {
            NetworkManagerMultiplayer.singleton.StopClient();
        }
    }
}
