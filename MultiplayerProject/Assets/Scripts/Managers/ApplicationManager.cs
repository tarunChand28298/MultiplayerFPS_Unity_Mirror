using UnityEngine;
using System.Collections.Generic;
using Mirror;

public class ApplicationManager : MonoBehaviour
{
    private static ApplicationManager _instance;
    public static ApplicationManager Instance() { return _instance; }

    NetworkManagerMultiplayer networkManager;
    [Scene] public string openingScene;

    [SerializeField] GameObject pauseMenuPanel;
    bool isPlaying;
    bool isPaused;
    public bool cursorShouldBeLocked;
    GameObject currentlyShowingPauseMenu;

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
        networkManager.OnClientDisconnectEventFired += NetworkManager_OnClientDisconnectEventFired;
        networkManager.OnClientStoppedEventFired += NetworkManager_OnClientStoppedEventFired;
    }

    private void NetworkManager_OnClientStoppedEventFired()
    {
        OpenOpeningScene();
    }

    private void NetworkManager_OnClientDisconnectEventFired()
    {
        OpenOpeningScene();
    }

    void OpenOpeningScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(openingScene);
    }

    public void HostGame(string name, string password, int nPlayers)
    {
        isPlaying = true;
        isHosting = true;
        hostedGameName = name;
        hostedGamePassword = password;
        hostedGameNPlayers = nPlayers;

        networkManager.StartHost();
        TryRegisterWithMatchmaker();
    }

    public void JoinGame(string ipAddress)
    {
        isPlaying = true;
        isHosting = false;

        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();
    }

    public void QuitApplicationButtonClicked()
    {
        Application.Quit();
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

    

    private void Update()
    {
        if(isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!isPaused)
                {
                    isPaused = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    GameObject uiCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
                    currentlyShowingPauseMenu = Instantiate(pauseMenuPanel, uiCanvas.transform);
                    currentlyShowingPauseMenu.GetComponent<PauseMenuScript>().isHosting = isHosting;
                }
                else
                {
                    isPaused = false;
                    if (cursorShouldBeLocked)
                    {
                        Cursor.lockState = CursorLockMode.Locked;
                        Cursor.visible = false;
                    }
                    Destroy(currentlyShowingPauseMenu);
                }
            }
        }
    }
}
