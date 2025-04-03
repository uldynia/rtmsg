using UnityEngine;
using Mirror;
using System.Collections;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Spine.Unity;
using Spine;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController localPlayer;

    [SerializeField]
    private int startingHealth;

    private bool hasInitialised = false;

    [SerializeField]
    private GameObject poofGO;

    [SerializeField]
    private string animationPoofName;

    [SerializeField] AudioClip win_audioclip;
    [SerializeField] AudioClip lose_audioclip;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (localPlayer != null)
        {
            Destroy(localPlayer.gameObject);
        }
        localPlayer = this;
        name = "Local Player";
    }
    private void Update()
    {
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
                    hasInitialised = true;
                }
            }
        }
        if (!TransportManager.transport.Available())
        {
            TransportManager.transport.ConnectToRelay();
            return;
        }
    }
    // made it public because TutorialPlayer needs it :3 - xavier
    [Command]
    public void RequestHealthUIUpdate()
    {
        UpdateHealthUI(GameManager.instance.playerOneHealth, GameManager.instance.playerTwoHealth);
    }
    [Command]
    public void SpawnEntity(int entityID, Vector3 newPosition, int level)
    {
        int dir = 1;
        if (localPlayer.netId != netId)
        {
            Vector2 yBoundary = GridManager.instance.GetPlaceableBoundaryY();
            // not the local player who spawned, place it on the other side
            newPosition = new(newPosition.x, yBoundary.y - newPosition.y + yBoundary.x, newPosition.z);
            dir = -1;
        }
        GameManager.instance.SpawnEntity(entityID, newPosition, dir, level);
    }

    public uint GetNetId()
    {
        return netId;
    }

    [ClientRpc]
    public void RegisterStationaryObject(Vector2Int position, uint netId) // Function called on host player on both clients
    {
        if (localPlayer.netId != this.netId)
        {
            position.y = GridManager.instance.GetMap().y - position.y - 1;
        }
        GridManager.instance.coveredGrids.Add(position);
    }

    [ClientRpc]
    public void UnregisterStationaryObject(Vector2Int position)  // Function called on host player on both clients
    {
        if (localPlayer.netId != netId)
        {
            position.y = GridManager.instance.GetMap().y - position.y - 1;
        }
        GridManager.instance.coveredGrids.Remove(position);
    }
    [ClientRpc]
    public void UpdateHealthUI(int localPlayerHealth, int otherPlayerHealth)  // Function called on host player on both clients
    {
        Debug.Log("Is this host? " + (localPlayer == this));
        if (localPlayer != this)
        {
            Debug.Log("Host health " + otherPlayerHealth);
            Debug.Log("Enemy health " + localPlayerHealth);
            HealthUI.instance.UpdateUI(otherPlayerHealth, localPlayerHealth);
            hasInitialised = true;
        }
        else
        {
            Debug.Log("Host health " + localPlayerHealth);
            Debug.Log("Enemy health " + otherPlayerHealth);
            HealthUI.instance.UpdateUI(localPlayerHealth, otherPlayerHealth);
        }
    }

    [ClientRpc]
    public void Result(bool isHostWin, Vector3 cameraPosition) // Function called on host player on both clients
    {
        if (isHostWin)
        {
            if (isServer)
            {
                EndScreen(true, cameraPosition);
                AudioSfxManager.m_instance.OnPlayNewAudioClip(win_audioclip);
            }
            else
            {
                EndScreen(false, cameraPosition);
                AudioSfxManager.m_instance.OnPlayNewAudioClip(lose_audioclip);
            }
        }
        else
        {
            if (isServer)
            {
                EndScreen(false, cameraPosition);
                AudioSfxManager.m_instance.OnPlayNewAudioClip(lose_audioclip);
            }
            else
            {
                EndScreen(true, cameraPosition);
                AudioSfxManager.m_instance.OnPlayNewAudioClip(win_audioclip);
            }
            
        }
    }

    [ClientRpc]
    public void SpawnPoof(Vector3 pos)
    {
        pos.z = -5;
        if (localPlayer.netId != this.netId)
        {
            Vector2 yBoundary = GridManager.instance.GetPlaceableBoundaryY();
            pos.y = yBoundary.y - pos.y + yBoundary.x;
        }
        SkeletonAnimation anim = Instantiate(poofGO, pos, Quaternion.identity).GetComponent<SkeletonAnimation>();
        TrackEntry en = anim.AnimationState.Tracks.Items[0];
        en.TrackEnd = en.AnimationTime;
        anim.AnimationState.SetAnimation(0,animationPoofName, false);

        // Bandage fix, please fix in the future
        Destroy(anim.gameObject, 1f);
    }

    [Client]
    private void EndScreen(bool won, Vector3 pos)=>EndScreenManager.instance.ShowEndScreen(won, pos);

    [Command]
    public void Forfeit()
    {
        GameManager.instance.Forfeit(isServer);
    }
}
