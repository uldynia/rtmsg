using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController localPlayer;

    [SerializeField]
    private int startingHealth;

    private int currentHealth;

    private bool hasInitialised = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayer = this;    
    }
    private void Update()
    {
        if (!hasInitialised)
        {
            if (isServer && isLocalPlayer && HealthUI.instance)
            {
                UpdateHealthUI(startingHealth, startingHealth);
            }
        }
    }
    [Command]
    public void SpawnEntity(int entityID, Vector3 newPosition, int level)
    {
        int dir = 1;
        if (localPlayer != this)
        {
            Vector2 yBoundary = GridManager.instance.GetPlaceableBoundaryY();
            // not the local player who spawned, place it on the other side
            newPosition = new(newPosition.x, yBoundary.y - newPosition.y + yBoundary.x, newPosition.z);
            dir = -1;
        }
        GameManager.instance.SpawnEntity(entityID, newPosition, dir, level);
    }

    [ClientRpc]
    public void RegisterStationaryObject(Vector2Int position)
    {
        if (localPlayer != this)
        {
            position.y = GridManager.instance.GetMap().y - position.y - 1;
        }
        GridManager.instance.coveredGrids.Add(position);
    }

    [ClientRpc]
    public void UnregisterStationaryObject(Vector2Int position)
    {
        if (localPlayer != this)
        {
            position.y = GridManager.instance.GetMap().y - position.y - 1;
        }
        GridManager.instance.coveredGrids.Remove(position);
    }
    [ClientRpc]
    public void UpdateHealthUI(int localPlayerHealth, int otherPlayerHealth)
    {
        
        if (localPlayer != this)
        {
            currentHealth = otherPlayerHealth;
            HealthUI.instance.UpdateUI(otherPlayerHealth, localPlayerHealth);
        }
        else
        {
            currentHealth = localPlayerHealth;
            HealthUI.instance.UpdateUI(localPlayerHealth, otherPlayerHealth);
        }
    }
}
