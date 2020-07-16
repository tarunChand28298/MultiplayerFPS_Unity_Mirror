using Mirror;
using UnityEngine;

public class BulletComponent : NetworkBehaviour
{
    public float lifetime;
    public float speed;
    public int damageAmount;
    bool hasHit;

    [SyncVar] int hitSurface = 0; //0=no hit, 1=player, 2=surface
    [SyncVar] Vector3 hitnormal;

    public GameObject playerHitParticleObject;
    public GameObject solidHitParticleObject;

    private void Start()
    {
        if (!isServer) { return; }

        Invoke("DestroySelf", lifetime);
    }

    private void Update()
    {
        if (!isServer) { return; }
        if (hasHit)
        {
            NetworkServer.Destroy(gameObject);
            return;
        }

        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, speed * Time.deltaTime))
        {
            hitnormal = hitInfo.normal;
            if(hitInfo.collider.gameObject.tag == "Player")
            {
                hitInfo.collider.gameObject.GetComponent<Pawn>().TakeDamage(damageAmount, hitInfo.point, hitInfo.normal);
                hitSurface = 1;
            }
            else if(hitInfo.collider.gameObject.tag == "SolidObject")
            {
                hitSurface = 2;
            }
            
            hasHit = true;
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    public override void OnStopClient()
    {
        switch (hitSurface)
        {
            case 0: { break; }
            case 1:
                {
                    Instantiate(playerHitParticleObject, transform.position, Quaternion.LookRotation(hitnormal));
                    break;
                }
            case 2:
                {
                    Instantiate(solidHitParticleObject, transform.position, Quaternion.LookRotation(hitnormal));
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    void DestroySelf() 
    {
        if(gameObject != null)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
    
}
