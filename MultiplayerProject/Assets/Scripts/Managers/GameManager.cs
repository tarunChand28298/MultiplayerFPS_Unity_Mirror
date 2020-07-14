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

    private bool putInA = true;

    public SyncGameObjects teamA;
    public SyncGameObjects teamB;

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
        newPawn.GetComponent<Pawn>().teamColour = teamA.Contains(controller.gameObject) ? Color.red : Color.blue;
        newPawn.GetComponent<Pawn>().PullInCamera();
        newPawn.GetComponent<Pawn>().playerController = controller;
        newPawn.GetComponent<Pawn>().totalBulletsLeft = 50;
        newPawn.GetComponent<Pawn>().health = 100;
    }

    public void WhenPlayerDies(PlayerController controller)
    {
        StartCoroutine(WhenPlayerDiesCoroutine(controller));
    }

    private IEnumerator WhenPlayerDiesCoroutine(PlayerController controller)
    {
        controller.controlledPawn.GetComponent<Pawn>().PullOutCamera();

        yield return new WaitForSeconds(1);

        NetworkServer.Destroy(controller.controlledPawn);
    }

}
