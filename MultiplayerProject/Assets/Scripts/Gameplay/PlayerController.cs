using UnityEngine;
using Mirror;

public static class PlayerControllerSettings
{
    public static float MoveSpeed = 10.0f;
    public static float TurnSpeed = 5.0f;
    public static float Gravity = 10.0f;
}

public class PlayerController : NetworkBehaviour
{
    [SyncVar] public string alias;
    [SyncVar] public GameObject controlledPawn;

    Transform pawnCameraGimbal;

    private void Update()
    {
        if (!hasAuthority) { return; }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CmdRequestToSpawn();
        }

        if (controlledPawn == null) return;

        Pawn controlledPawnRef = controlledPawn.GetComponent<Pawn>();

        Vector3 displacement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Depth"));
        Vector3 rotation = new Vector3(0, Input.GetAxisRaw("Mouse X"), 0);
        if(pawnCameraGimbal == null) { pawnCameraGimbal = controlledPawn.GetComponent<Pawn>().cameraPos; }
        else
        {
            controlledPawnRef.xRotation -= Input.GetAxis("Mouse Y") * PlayerControllerSettings.TurnSpeed;
            controlledPawnRef.xRotation = Mathf.Clamp(controlledPawnRef.xRotation, -60, 60);
            pawnCameraGimbal.localRotation = Quaternion.Euler(controlledPawnRef.xRotation, 0.0f, 0.0f);
            CmdSetPitch(pawnCameraGimbal.rotation);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (controlledPawn.GetComponent<Pawn>().bulletsInMag > 0 && !controlledPawn.GetComponent<Pawn>().reloading)
            {
                RaycastHit hitinfo;
                bool doesHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitinfo);
                CmdShootGun(doesHit, hitinfo.point);
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (controlledPawn.GetComponent<Pawn>().totalBulletsLeft > 0)
            {
                CmdReloadGun();
            }
        }

        CmdMovePlayer(displacement, rotation);
    }

    public void NotifyGameManagerOfDeath()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().WhenPlayerDies(this);
    }

    [Command] void CmdRequestToSpawn()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().SpawnPlayerForController(this);
    }
    [Command] void CmdMovePlayer(Vector3 displacement, Vector3 rotation)
    {
        if (controlledPawn == null) return;

        Transform controlledPawnXform = controlledPawn.transform;
        Pawn controlledPawnRef = controlledPawn.GetComponent<Pawn>();

        controlledPawnRef.isGrounded = Physics.CheckSphere(controlledPawnRef.transform.position, 1.5f, controlledPawnRef.groundCollisionMask);

        if (controlledPawnRef.isGrounded) { controlledPawnRef.downwardVelocity = 0; }
        else { controlledPawnRef.downwardVelocity += PlayerControllerSettings.Gravity * Time.deltaTime; }

        Vector3 movement = (controlledPawnXform.forward * displacement.z + controlledPawnXform.right * displacement.x).normalized * Time.deltaTime;
        movement += Vector3.down * controlledPawnRef.downwardVelocity * Time.deltaTime;

        Vector3 turn = Vector3.up * rotation.y;


        controlledPawn.GetComponent<CharacterController>().Move(movement * PlayerControllerSettings.MoveSpeed);
        controlledPawn.transform.Rotate(turn * PlayerControllerSettings.TurnSpeed);
    }
    [Command] void CmdSetPitch(Quaternion absoluteRotation)
    {
        if (controlledPawn == null) return;

        RpcSetPitchOnPawn(absoluteRotation);
    }
    [Command] void CmdShootGun(bool hasHitTarget, Vector3 targetPosition)
    {
        if (controlledPawn == null) return;

        controlledPawn.GetComponent<Pawn>().Shoot(hasHitTarget, targetPosition);
    }
    [Command] void CmdReloadGun()
    {
        if (controlledPawn == null) return;
        if (controlledPawn.GetComponent<Pawn>().bulletsInMag == Pawn.bulletsPerMag) return;

        controlledPawn.GetComponent<Pawn>().reloading = true;
        controlledPawn.GetComponent<Pawn>().Reload();
        Invoke("SetPawnGunReloadFalse", controlledPawn.GetComponent<Pawn>().reloadAnimDuration);
    }


    [ClientRpc] void RpcSetPitchOnPawn(Quaternion absoluteRotation)
    {
        if (controlledPawn == null) return;
        var pawn = controlledPawn.GetComponent<Pawn>();
        if(pawn != null)
        {
            pawn.gunTransform.rotation = absoluteRotation;
        }
    }

    void SetPawnGunReloadFalse() { controlledPawn.GetComponent<Pawn>().reloading = false; }
}
