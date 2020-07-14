using UnityEngine;
using System.Collections.Generic;

public class ApplicationManager : MonoBehaviour
{
    private static ApplicationManager _instance;
    public static ApplicationManager Instance() { return _instance; }

    NetworkManagerMultiplayer networkManager;

    private bool isHosting;
    private string hostedGameName;
    private string hostedGamePassword;
    private int hostedGameNPlayers;

    private void Awake()
    {
        if(_instance != null) { Destroy(gameObject); }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManagerMultiplayer>();
    }

    public void HostGame(string name, string password, int nPlayers)
    {
        isHosting = true;
        hostedGameName = name;
        hostedGamePassword = password;
        hostedGameNPlayers = nPlayers;

        networkManager.StartHost();
        TryRegisterWithMatchmaker();
    }

    public void JoinGame(string ipAddress)
    {
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
    }

    public List<string> GetActiveHosts()
    {
        List<string> hostsAvailable = new List<string>();

        hostsAvailable.Add("host1");
        hostsAvailable.Add("host2");
        hostsAvailable.Add("host3");

        return hostsAvailable;
    }

    private void TryRegisterWithMatchmaker()
    {

    }
    private void TryUnregisterFromMatchmaker()
    {

    }
}
