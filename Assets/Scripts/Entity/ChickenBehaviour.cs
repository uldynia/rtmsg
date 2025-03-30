using Mirror;
using System.Collections;
using UnityEngine;

public class ChickenBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private float timeToHatch;

    private float currTimeToHatch;
    private bool isEgg;

    [Header("Level 2")]
    [SerializeField]
    private float secondChickenDelay;

    [SerializeField]
    private GameObject chickenNoHatchTimerPrefab;
    private Vector3 eggPosition;

    [Header("Level 3")]
    [SerializeField]
    private float chickenSpawnerInterval;
    [SerializeField]
    private int bucketHealth;

    private float currChickenSpawnerInterval;

    

    public override void OnStartServer() // Make sure properties get inherited ONLY if its spawned as a chicken and not an egg
    {
        if (!isEgg)
        {
            base.OnStartServer();
        }
    }

    protected override void UpdateServer()
    {
        base.UpdateServer();

        if (isEgg)
        {
            if (level < 3)
            {
                currTimeToHatch -= Time.deltaTime;
                if (currTimeToHatch <= 0)
                {    
                    // Remove egg properties & setup chicken properties
                    isEgg = false;
                    currSpd = animalData.Speed;
                    currHp = animalData.Health;
                    ogHp = currHp;
                    PlayerController.localPlayer.UnregisterStationaryObject(GridManager.instance.GetGridCoordinate(transform.position));

                    if (level > 1) // Level 2 must spawn a second chicken
                    {
                        StartCoroutine(SpawnChicken(secondChickenDelay));
                    }
                }
            }
            else
            {
                // Do not remove egg properties as this is a spawner
                currChickenSpawnerInterval -= Time.deltaTime;
                if (currChickenSpawnerInterval <= 0) // Spawn chicken with no delay as the timer is already counted
                {
                    StartCoroutine(SpawnChicken(0));
                    currChickenSpawnerInterval = chickenSpawnerInterval;
                }
            }
        }
    }

    private IEnumerator SpawnChicken(float delay)
    {
        // Copy pasted entity spawning from gamemanager and manually changed direction and level
        yield return new WaitForSeconds(delay);

        GameObject entity = Instantiate(chickenNoHatchTimerPrefab, eggPosition, Quaternion.identity);

        EntityBaseBehaviour behaviour = entity.GetComponent<EntityBaseBehaviour>();

        behaviour.ChangeDirection(direction);
        behaviour.ChangeLevel(level);
        behaviour.ChangeData(animalData);
        GameManager.instance.entities.Add(behaviour);
        NetworkServer.Spawn(entity);
    }
    public override void OnDeath()
    {
        if (isEgg) // unregister if egg
        {
            PlayerController.localPlayer.UnregisterStationaryObject(GridManager.instance.GetGridCoordinate(transform.position));
        }
        base.OnDeath();
    }
    // Setup egg state
    public override void Setup(int direction, int level)
    {
        base.Setup(direction, level);

        // Setup egg properties
        isEgg = true;
        currTimeToHatch = timeToHatch;
        PlayerController.localPlayer.RegisterStationaryObject(GridManager.instance.GetGridCoordinate(transform.position), PlayerController.localPlayer.GetNetId());
        currHp = 1;
        if (level > 2) // use bucket health instead
        {
            currHp = bucketHealth;
        }
        ogHp = currHp;
        currSpd = 0;
        currChickenSpawnerInterval = chickenSpawnerInterval;
        eggPosition = transform.position;
    }
    // Insta-hatch
    public void Hatch()
    {
        if (isEgg)
        {
            // Spawn chicken for level 3
            if (level > 2)
            {
                StartCoroutine(SpawnChicken(0));
                currChickenSpawnerInterval = chickenSpawnerInterval;
            }
            else
            {
                // Remove egg properties & setup chicken properties
                isEgg = false;
                currSpd = animalData.Speed;
                currHp = animalData.Health;
                ogHp = currHp;
                PlayerController.localPlayer.UnregisterStationaryObject(GridManager.instance.GetGridCoordinate(transform.position));

                if (level > 1) // Spawn extra chicken for level 2
                {
                    StartCoroutine(SpawnChicken(secondChickenDelay));
                }
            }
        }
    }
}
