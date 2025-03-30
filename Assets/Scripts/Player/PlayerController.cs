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
        if (!hasInitialised)
        {
            if (isServer && GameManager.instance)
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
        }
        else
        {
            HealthUI.instance.UpdateUI(localPlayerHealth, otherPlayerHealth);
        }
    }

    [ClientRpc]
    public void Result(bool isWin) // Function called on host player on both clients
    {
        if (isWin)
        {
            if (isServer)
            {
                Win();
            }
            else
            {
                Lose();
            }
        }
        else
        {
            if (isServer)
            {
                Lose();
            }
            else
            {
                Win();
            }
        }
    }

    [Client]
    private void Win()
    {
        Debug.Log("You Win!");
    }

    [Client]
    private void Lose()
    {
        Debug.Log("L BOZO YOU LOST NOOB GET GOOD L DUDE YOU CANT EVEN BEAT HIM");
    }
}
