using UnityEngine;
using Mirror;
using System.Collections;

public class SyncGameObjects : SyncList<GameObject> { }

public class GameManager : NetworkBehaviour
{
    public GameObject pawnPrefab;
    public Transform overheadCam;
    public Transform TeamASpawnPoint;
    public Transform TeamBSpawnPoint;

    public GameObject scoreUIPrefab;
    GameObject currentScoreUIGameObject;

    private bool putInA = true;

    [SyncVar(hook = nameof(UpdateScoreUIForA))] public int teamAScore = 0;
    [SyncVar(hook = nameof(UpdateScoreUIForB))] public int teamBScore = 0;
    public SyncGameObjects teamA;
    public SyncGameObjects teamB;

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameObject uiCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
        currentScoreUIGameObject = Instantiate(scoreUIPrefab, uiCanvas.transform);
    }

    public void AdmitToGame(GameObject newController)
    {
        if(putInA) { teamA.Add(newController); }
        else { teamB.Add(newController); }
        putInA = !putInA;

        SpawnPlayerForController(newController.GetComponent<PlayerController>());
    }

    public void SpawnPlayerForController(PlayerController controller)
    {
        Transform spawnPointCurrent = teamA.Contains(controller.gameObject) ? TeamASpawnPoint : TeamBSpawnPoint;
        GameObject newPawn = Instantiate(pawnPrefab, spawnPointCurrent.position, spawnPointCurrent.rotation);
        NetworkServer.Spawn(newPawn, controller.gameObject);

        controller.controlledPawn = newPawn;
        newPawn.GetComponent<NetworkIdentity>().AssignClientAuthority(controller.connectionToClient);
        newPawn.GetComponent<Pawn>().teamColour = teamA.Contains(controller.gameObject) ? Color.red : Color.blue;
        newPawn.GetComponent<Pawn>().PullInCamera();
        newPawn.GetComponent<Pawn>().playerController = controller;
        newPawn.GetComponent<Pawn>().totalBulletsLeft = 100;
        newPawn.GetComponent<Pawn>().health = 100;
    }

    public void WhenPlayerDies(PlayerController controller)
    {
        if (teamA.Contains(controller.gameObject)) { teamBScore++; }
        else { teamAScore++; }

        NetworkServer.Destroy(controller.controlledPawn);
    }

    void UpdateScoreUIForA(int oldValue, int newValue)
    {
        currentScoreUIGameObject.GetComponent<ScoreUI>().UpdateScoreForA(newValue);
    }
    void UpdateScoreUIForB(int oldValue, int newValue)
    { 
        currentScoreUIGameObject.GetComponent<ScoreUI>().UpdateScoreForB(newValue);
    }

}
