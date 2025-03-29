using UnityEngine;
using System.Collections.Generic;
using Mirror;
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; } // makes it so that other scripts cant change the singleton instance

    // List of all animals in the game, corresponding to their ids.
    [SerializeField]
    private List<AnimalType> animalTypes = new List<AnimalType>();
    private void Awake()
    {
        instance = this;
    }

    [Server]
    public void SpawnEntity(int entityID, Vector3 position, int dir, int level)
    {
        GameObject entity = Instantiate(animalTypes[entityID].PrefabToSpawn,position,Quaternion.identity);

        EntityBaseBehaviour behaviour = entity.GetComponent<EntityBaseBehaviour>();
        behaviour.Setup(dir,level);
        if (behaviour.GetData().Speed <= 0)
        {
            PlayerController.localPlayer.RegisterStationaryObject(GridManager.instance.GetGridCoordinate(position));
        }
        Debug.Log("Spawning: " + animalTypes[entityID].Name + " level: " + level);

        NetworkServer.Spawn(entity);
    }
}
