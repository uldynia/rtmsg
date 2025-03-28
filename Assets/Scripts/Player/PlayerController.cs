using UnityEngine;
using Mirror;
public class PlayerController : NetworkBehaviour
{
    public static PlayerController localPlayer;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayer = this;
    }

    [Command]
    public void SpawnEntity(int entityID, Vector3 newPosition)
    {
        int dir = 1;
        if (localPlayer != this)
        {
            Vector2 yBoundary = GridManager.instance.GetPlaceableBoundaryY();
            // not the local player who spawned, place it on the other side
            newPosition = new(newPosition.x, yBoundary.y - newPosition.y + yBoundary.x, newPosition.z);
            dir = -1;
        }
        GameManager.instance.SpawnEntity(entityID, newPosition, dir);
    }
}
