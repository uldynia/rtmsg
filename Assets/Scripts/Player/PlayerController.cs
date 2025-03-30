using UnityEngine;
using Mirror;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController localPlayer;

    [SerializeField]
    private int startingHealth;

    private bool hasInitialised = false;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayer = this;    
    }
    private void Update()
    {
        if (isLocalPlayer && localPlayer == null)
        {
            localPlayer = this;
        }
        if (!hasInitialised)
        {
            if (isServer)
            {
                if (GameManager.instance)
                {
                    if (isLocalPlayer)
                    {
                        if (HealthUI.instance)
                        {
                            UpdateHealthUI(startingHealth, startingHealth);
                            GameManager.instance.playerOneHealth = startingHealth;
                            hasInitialised = true;
                        }
                    }
                    else
                    {
                        GameManager.instance.playerTwoHealth = startingHealth;
                        hasInitialised = true;
                    }
                }
            }
            else
            {
                if (!isLocalPlayer && HealthUI.instance)
                {
                    localPlayer.RequestHealthUIUpdate();
                }
            }
        }
    }
    [Command]
    private void RequestHealthUIUpdate()
    {
        UpdateHealthUI(GameManager.instance.playerOneHealth, GameManager.instance.playerTwoHealth);
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
    public void RegisterStationaryObject(Vector2Int position) // Function called on host player on both clients
    {
        if (localPlayer != this)
        {
            position.y = GridManager.instance.GetMap().y - position.y - 1;
        }
        GridManager.instance.coveredGrids.Add(position);
    }

    [ClientRpc]
    public void UnregisterStationaryObject(Vector2Int position)  // Function called on host player on both clients
    {
        if (localPlayer != this)
        {
            position.y = GridManager.instance.GetMap().y - position.y - 1;
        }
        GridManager.instance.coveredGrids.Remove(position);
    }
    [ClientRpc]
    public void UpdateHealthUI(int localPlayerHealth, int otherPlayerHealth)  // Function called on host player on both clients
    {
        if (localPlayer != this)
        {
            HealthUI.instance.UpdateUI(otherPlayerHealth, localPlayerHealth);
            hasInitialised = true;
        }
        else
        {
            HealthUI.instance.UpdateUI(localPlayerHealth, otherPlayerHealth);
        }
    }

    [ClientRpc]
    public void Result(bool isWin, Vector3 cameraPosition) // Function called on host player on both clients
    {
        if (isWin)
        {
            if (isServer)
            {
                EndScreen(true, cameraPosition);
            }
            else
            {
                EndScreen(false, cameraPosition);
            }
        }
        else
        {
            if (isServer)
            {
                EndScreen(false, cameraPosition);
            }
            else
            {
                EndScreen(true, cameraPosition);
            }
        }
    }

    [Client]
    private void EndScreen(bool won, Vector3 pos)=>EndScreenManager.instance.ShowEndScreen(won, pos);
}
