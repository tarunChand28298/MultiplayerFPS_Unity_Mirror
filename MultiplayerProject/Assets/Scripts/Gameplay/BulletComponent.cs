using Mirror;
using UnityEngine;

public class BulletComponent : NetworkBehaviour
{
    public float lifetime;
    public float speed;
    public int damageAmount;

    private void Start()
    {
        if (!isServer) { return; }

        Invoke("DestroySelf", lifetime);
    }

    private void Update()
    {
        if (!isServer) { return; }

        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, speed * Time.deltaTime))
        {
            if(hitInfo.collider.gameObject.tag == "Player")
            {
                hitInfo.collider.gameObject.GetComponent<Pawn>().TakeDamage(damageAmount, hitInfo.point, hitInfo.normal);
            }
            else if(hitInfo.collider.gameObject.tag == "SolidObject")
            {
                Debug.Log("hit a solid object");
            }

            NetworkServer.Destroy(gameObject);
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void DestroySelf() 
    {
        if(gameObject != null)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
    
}
