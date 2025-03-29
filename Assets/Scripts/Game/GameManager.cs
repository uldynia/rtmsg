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

        entity.GetComponent<EntityBaseBehaviour>().Setup(dir,level);

        NetworkServer.Spawn(entity);
    }
}
