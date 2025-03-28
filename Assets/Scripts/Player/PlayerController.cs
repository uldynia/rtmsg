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
    public void SpawnEntity(int entityID, Vector3 position)
    {
        GameManager.instance.SpawnEntity(entityID, position);
    }
}
