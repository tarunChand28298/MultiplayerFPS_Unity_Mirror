using UnityEngine.UI;
using UnityEngine;

public class AliasPrompt : MonoBehaviour
{
    public NetworkManagerMultiplayer forwardTo;
    public InputField aliasInput;
    public Mirror.NetworkConnection conn;

    public void OnOKClicked()
    {
        forwardTo.SetAliasForPlayer(aliasInput.text, conn);
        Destroy(gameObject);
    }

}
