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

                    if (level > 1)
                    {
                        StartCoroutine(SpawnChicken(secondChickenDelay));
                    }
                }
            }
            else
            {
                currChickenSpawnerInterval -= Time.deltaTime;
                if (currChickenSpawnerInterval <= 0)
                {
                    StartCoroutine(SpawnChicken(0));
                    currChickenSpawnerInterval = chickenSpawnerInterval;
                }
            }
        }
    }

    private IEnumerator SpawnChicken(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject entity = Instantiate(chickenNoHatchTimerPrefab, eggPosition, Quaternion.identity);

        EntityBaseBehaviour behaviour = entity.GetComponent<EntityBaseBehaviour>();

        behaviour.ChangeDirection(direction);
        behaviour.ChangeLevel(level);

        NetworkServer.Spawn(entity);
    }
    protected override void OnDeath()
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
        PlayerController.localPlayer.RegisterStationaryObject(GridManager.instance.GetGridCoordinate(transform.position));
        currHp = 1;
        ogHp = 1;
        currSpd = 0;
        currChickenSpawnerInterval = chickenSpawnerInterval;
        eggPosition = transform.position;
    }
}
