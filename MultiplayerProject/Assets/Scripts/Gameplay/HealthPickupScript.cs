using Mirror;
using UnityEngine;

public class HealthPickupScript : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        var triggeringPawn = other.gameObject.GetComponent<Pawn>();
        if (triggeringPawn != null)
        {
            triggeringPawn.RegenHealth();
            NetworkServer.Destroy(gameObject);
        }
    }
}
