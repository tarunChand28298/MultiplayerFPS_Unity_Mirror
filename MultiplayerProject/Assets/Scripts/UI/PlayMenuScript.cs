using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenuScript : MonoBehaviour
{
    public InputField gameNameInput;
    public InputField passwordInput;
    public InputField nPlayersInput;

    public Dropdown hostSelectionDropdown;
    public InputField hostIPAddressInput;

    List<string> hostsAvailable;

    public void Start()
    {
        nPlayersInput.text = 1.ToString();
        hostsAvailable = new List<string>();
        
    }

    public void IncrementNPlayers()
    {
        nPlayersInput.text = Mathf.Clamp((int.Parse(nPlayersInput.text) + 1), 1, 64).ToString();
    }
    public void DecrementNPlayers()
    {
        nPlayersInput.text = Mathf.Clamp((int.Parse(nPlayersInput.text) - 1), 1, 64).ToString();
    }
    public void SetHostIPAddressInputValue(int index)
    {
        hostIPAddressInput.text = hostsAvailable[index];
    }

    public void HostButtonClicked()
    {
        ApplicationManager.Instance().HostGame(gameNameInput.text, passwordInput.text, Mathf.Clamp(int.Parse(nPlayersInput.text), 1, 64));
    }

    public void JoinButtonClicked()
    {
        ApplicationManager.Instance().JoinGame(hostIPAddressInput.text);
    }

    public void RefreshButtonClicked()
    {
        hostSelectionDropdown.ClearOptions();
        hostsAvailable.Clear();

        hostsAvailable = ApplicationManager.Instance().GetActiveHosts();
        hostSelectionDropdown.AddOptions(hostsAvailable);
    }
}
