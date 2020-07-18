using Mirror;
using UnityEngine;

public class AmmoPickupScript : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        var triggeringPawn = other.gameObject.GetComponent<Pawn>();
        if (triggeringPawn != null)
        {
            triggeringPawn.ResupplyAmmo();
            NetworkServer.Destroy(gameObject);
        }
    }
}
