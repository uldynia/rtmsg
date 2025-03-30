using UnityEngine;
using System.Collections.Generic;
using Mirror;
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } // makes it so that other scripts cant change the singleton instance

    // List of all animals in the game, corresponding to their ids.
    [SerializeField]
    private List<AnimalType> animalTypes = new List<AnimalType>();

    public List<EntityBaseBehaviour> entities = new List<EntityBaseBehaviour>();

    public bool hasEnded;

    public int playerOneHealth; // P1 is Host
    public int playerTwoHealth; // P2 is Other Player

    public System.Action<EntityBaseBehaviour> onEntitySpawn;
    private void Awake()
    {
        instance = this;
        hasEnded = false;
    }
    [Server]
    public void SpawnEntity(int entityID, Vector3 position, int dir, int level)
    {
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
    }

    [Server]
    public void ReachedSpawn(EntityBaseBehaviour entity)
    {
        if (!hasEnded)
        {
            if (entity.GetDirection() == 1) // Checks if entity is host side or enemy side
            {
                playerTwoHealth -= entity.GetHealth();

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
        NetworkServer.Destroy(entity.gameObject);
    }
}
