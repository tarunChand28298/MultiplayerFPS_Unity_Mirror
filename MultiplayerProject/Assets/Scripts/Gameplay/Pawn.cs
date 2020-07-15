using Mirror;
using UnityEngine;

public class Pawn : NetworkBehaviour
{
    public PlayerController playerController;

    public GameObject playerStateUIPrefab;
    GameObject currentPlayerStateUIGameObject;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject childCube;
    public Transform cameraPos;
    public Transform groundCheck;
    public LayerMask groundCollisionMask;

    public Transform muzzelTransform;
    public Transform gunTransform;
    public GameObject gunGfx;

    [SyncVar] public bool dead;
    [SyncVar(hook =nameof(PlayerHealthUIChanged))] public int health;

    public static int bulletsPerMag = 10;
    [SyncVar] public bool reloading;
    [SyncVar(hook =nameof(BulletsInMagChanged))] public int bulletsInMag;
    [SyncVar] public int totalBulletsLeft;

    public float recoilAnimDuration;
    public float reloadAnimDuration;
    public AnimationCurve recoilEasingFunction;
    public AnimationCurve reloadEasingFunction;

    [SyncVar(hook = nameof(OnColourChange))] public Color teamColour;

    public float downwardVelocity;
    public bool isGrounded = true;
    public float xRotation = 0.0f;

    void OnColourChange(Color oldColor, Color newColor)
    {
        childCube.GetComponent<Renderer>().material.SetColor("_Color", newColor);
    }

    public void PullInCamera()
    {
        TargetPullInCamera();
    }
    public void PullOutCamera()
    {
        TargetPullOutCamera();
    }

    public void Shoot()
    {
        totalBulletsLeft--;
        bulletsInMag--;

        GameObject newBullet = Instantiate(bulletPrefab, muzzelTransform.position, muzzelTransform.rotation);
        NetworkServer.Spawn(newBullet);

        RpcShoot();
    }
    public void Reload()
    {
        int nBulletsCanBeLoaded = Mathf.Min(totalBulletsLeft, bulletsPerMag);
        bulletsInMag = nBulletsCanBeLoaded;

        RpcReload();
    }
    public void TakeDamage(int amount, Vector3 position, Vector3 normal)
    {
        health -= amount;
        RpcTakeDamage(position, normal);

        if(health <= 0)
        {
            if(!dead) playerController.NotifyGameManagerOfDeath();
            dead = true;
        }

    }

    [ClientRpc] void RpcShoot()
    {
        LeanTween.rotateLocal(gunGfx, Vector3.right * 50, recoilAnimDuration).setEase(recoilEasingFunction).setOnComplete(() =>
        {
            gunGfx.transform.localRotation = Quaternion.identity;
        });
    }
    [ClientRpc] void RpcReload()
    {
        LeanTween.rotateLocal(gunGfx, Vector3.right * 50, reloadAnimDuration).setEase(reloadEasingFunction).setOnComplete(() =>
        {
            gunGfx.transform.localRotation = Quaternion.identity;
        });
    }
    [ClientRpc] void RpcTakeDamage(Vector3 position, Vector3 normal)
    {
        Debug.Log("Emit particle on player now.");
    }

    [TargetRpc] void TargetPullInCamera()
    {
        Camera.main.transform.SetParent(cameraPos);
        LeanTween.move(Camera.main.gameObject, cameraPos, 1.0f);
        LeanTween.rotate(Camera.main.gameObject, transform.rotation.eulerAngles, 1.0f).setOnComplete(() => {
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;
        });

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ApplicationManager.Instance().cursorShouldBeLocked = true;
    }
    [TargetRpc] void TargetPullOutCamera()
    {
        Camera.main.transform.SetParent(null);
        Transform toXform = GameObject.FindGameObjectWithTag("PulloutCamTransform").transform;
        LeanTween.move(Camera.main.gameObject, toXform, 1.0f);
        LeanTween.rotate(Camera.main.gameObject, toXform.eulerAngles, 1.0f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ApplicationManager.Instance().cursorShouldBeLocked = false;
    }

    void PlayerHealthUIChanged(int oldValue, int newValue)
    {
        if (currentPlayerStateUIGameObject == null) return;
        currentPlayerStateUIGameObject.GetComponent<PlayerStateUIScript>().SetHealthText(newValue);
    }

    void BulletsInMagChanged(int oldValue, int newValue)
    {
        if (currentPlayerStateUIGameObject == null) return;
        currentPlayerStateUIGameObject.GetComponent<PlayerStateUIScript>().SetBulletsText(bulletsInMag, totalBulletsLeft - bulletsInMag);
    }

    public override void OnStartClient()
    {
        if (!hasAuthority) return;
        base.OnStartClient();
        GameObject uiCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
        currentPlayerStateUIGameObject = Instantiate(playerStateUIPrefab, uiCanvas.transform);

        currentPlayerStateUIGameObject.GetComponent<PlayerStateUIScript>().SetHealthText(health);
        currentPlayerStateUIGameObject.GetComponent<PlayerStateUIScript>().SetBulletsText(bulletsInMag, totalBulletsLeft - bulletsInMag);
    }

    private void OnDestroy()
    {
        if (!hasAuthority) return;

        if (currentPlayerStateUIGameObject == null) return;
        Destroy(currentPlayerStateUIGameObject);
    }

}
