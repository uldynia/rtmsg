using UnityEngine;
using System.Collections.Generic;
using Mirror;
using static UnityEngine.EventSystems.EventTrigger;
using Spine.Unity;
using System.Runtime.CompilerServices;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } // makes it so that other scripts cant change the singleton instance

    // List of all animals in the game, corresponding to their ids.
    [SerializeField]
    private List<AnimalType> animalTypes = new List<AnimalType>();

    public List<EntityBaseBehaviour> entities = new List<EntityBaseBehaviour>();

    public bool hasEnded;
    public float timeBeforeLifeLost;
    public float currTime;
    public float lifeLostInterval;
    public int playerOneHealth; // P1 is Host
    public int playerTwoHealth; // P2 is Other Player

    public System.Action<EntityBaseBehaviour> onEntitySpawn;
    private void Awake()
    {
        instance = this;
        hasEnded = false;
        lifeLostInterval = 0;
        currTime = 0;
    }
    private void Update()
    {
        if (currTime >= timeBeforeLifeLost)
        {
            if (lifeLostInterval >= 1)
            {

                playerTwoHealth -= 1;
                playerOneHealth -= 1;

                if (playerTwoHealth <= 0)
                {
                    playerTwoHealth = 0;
                    PlayerController.localPlayer.Result(true, Vector3.zero);
                    hasEnded = true;
                }
                

                if (playerOneHealth <= 0)
                {
                    playerOneHealth = 0;
                    PlayerController.localPlayer.Result(false, Vector3.zero);
                    hasEnded = true;
                }
                lifeLostInterval = 0;
                PlayerController.localPlayer.UpdateHealthUI(playerOneHealth, playerTwoHealth);
            }
            else
            {
                lifeLostInterval += Time.deltaTime;
            }
        }
        else
        {
            currTime += Time.deltaTime;
        }
    }
    [Server]
    public GameObject SpawnEntity(int entityID, Vector3 position, int dir, int level)
    {
        Debug.Log($"{entityID}, {position}, {dir}, {level}");
        GameObject entity = Instantiate(animalTypes[entityID].PrefabToSpawn,position,Quaternion.identity);

        EntityBaseBehaviour behaviour = entity.GetComponent<EntityBaseBehaviour>();
        behaviour.Setup(dir,level);
        if (behaviour.GetData().Speed <= 0)
        {
            PlayerController.localPlayer.RegisterStationaryObject(GridManager.instance.GetGridCoordinate(position), PlayerController.localPlayer.GetNetId());
        }
        entities.Add(behaviour);
        NetworkServer.Spawn(entity);

        if (onEntitySpawn != null)
        {
            onEntitySpawn.Invoke(behaviour);
        }
        return entity;
    }
    [Server]
    public void ReachedSpawn(EntityBaseBehaviour entity)
    {
        // Check for bull level 2 havent jump because we love hardcoding
        BullBehaviour bullBehaviour = entity as BullBehaviour;
        if (bullBehaviour)
        {
            if (!bullBehaviour.GetHasJumped() || bullBehaviour.GetIsJumping())
            {
                bullBehaviour.RemoveJump();
            }
        }
        if (!hasEnded)
        {
            if (entity.GetDirection() == 1) // Checks if entity is host side or enemy side
            {
                playerTwoHealth -= entity.GetHealth();

                if(TransportManager.instance.tutorialMode)
                {
                    TutorialPlayer.instance.TakeDamageTutorialComplete();
                }

                if (playerTwoHealth <= 0)
                {
                    playerTwoHealth = 0;
                    PlayerController.localPlayer.Result(true, entity.transform.position);
                    hasEnded = true;
                }
            }
            else
            {
                playerOneHealth -= entity.GetHealth();

                if (playerOneHealth <= 0)
                {
                    playerOneHealth = 0;
                    PlayerController.localPlayer.Result(false, entity.transform.position);
                    hasEnded = true;
                }
            }
            PlayerController.localPlayer.UpdateHealthUI(playerOneHealth, playerTwoHealth);
        }
        entities.Remove(entity);
        PlayerController.localPlayer.SpawnPoof(entity.transform.position);
        NetworkServer.Destroy(entity.gameObject);
    }

    public void Forfeit(bool isServer)
    {
        if(isServer)
        {
            playerOneHealth = 0;
                    PlayerController.localPlayer.Result(false, Vector3.zero);
        hasEnded = true;

        }
        else
        {

            playerTwoHealth = 0;
            PlayerController.localPlayer.Result(true, Vector3.zero);
            hasEnded = true;
        }
    }
}
