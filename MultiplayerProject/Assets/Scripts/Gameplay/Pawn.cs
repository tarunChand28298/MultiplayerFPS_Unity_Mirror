using Mirror;
using UnityEngine;

public class Pawn : NetworkBehaviour
{
    public PlayerController playerController;
    public CharacterController CharacterController;

    public GameObject playerStateUIPrefab;
    GameObject currentPlayerStateUIGameObject;

    public GameObject teamADeathParticles;
    public GameObject teamBDeathParticles;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject childCube;
    public Transform cameraPos;
    public Transform groundCheck;
    public LayerMask groundCollisionMask;

    public Transform muzzelTransform;
    public Transform gunTransform;
    public GameObject gunGfx;
    public GameObject muzzelFlashParticleObject;

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

    public AudioSource movementCueAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    public Texture albedoTeamA;
    public Texture albedoTeamB;

    public Texture emmisionA;
    public Texture emissionB;

    void OnColourChange(Color oldColor, Color newColor)
    {
        bool isATeam = newColor == Color.red;
        var currentMaterial = childCube.GetComponent<Renderer>().material;
        currentMaterial.SetTexture("_MainTex", isATeam ? albedoTeamA : albedoTeamB);
        currentMaterial.SetTexture("_EmissionMap", isATeam ? emmisionA : emissionB);
    }

    public void PullInCamera()
    {
        TargetPullInCamera();
    }
    public void PullOutCamera()
    {
        TargetPullOutCamera();
    }

    public void Shoot(bool hasHitTarget, Vector3 hitPosition)
    {
        totalBulletsLeft--;
        bulletsInMag--;

        GameObject newBullet = Instantiate(bulletPrefab, muzzelTransform.position, muzzelTransform.rotation);
        if(hasHitTarget) newBullet.transform.rotation = Quaternion.LookRotation(hitPosition - newBullet.transform.position);

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
        if (dead) return;
        health -= amount;
        RpcTakeDamage(position, normal);

        if(health <= 0)
        {
            playerController.NotifyGameManagerOfDeath();
            dead = true;
        }

    }

    [ClientRpc] void RpcShoot()
    {
        LeanTween.rotateLocal(gunGfx, Vector3.right * 50, recoilAnimDuration).setEase(recoilEasingFunction).setOnComplete(() =>
        {
            gunGfx.transform.localRotation = Quaternion.identity;
        });
        Instantiate(muzzelFlashParticleObject, muzzelTransform.position, muzzelTransform.rotation);
        sfxAudioSource.PlayOneShot(shootSound);
    }
    [ClientRpc] void RpcReload()
    {
        sfxAudioSource.PlayOneShot(reloadSound);

        LeanTween.rotateLocal(gunGfx, Vector3.right * 50, reloadAnimDuration).setEase(reloadEasingFunction).setOnComplete(() =>
        {
            gunGfx.transform.localRotation = Quaternion.identity;
        });
    }
    [ClientRpc] void RpcTakeDamage(Vector3 position, Vector3 normal)
    {
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
        PullOutCameraSelf();
    }

    void PullOutCameraSelf()
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
    public override void OnStopClient()
    {
        base.OnStopClient();
        GameObject particleToSpawn = teamColour == Color.red ? teamADeathParticles : teamBDeathParticles;
        Instantiate(particleToSpawn, transform.position, transform.rotation);

        if (!hasAuthority) return;

        PullOutCameraSelf();

        if (currentPlayerStateUIGameObject == null) return;
        Destroy(currentPlayerStateUIGameObject);
    }
    
}
