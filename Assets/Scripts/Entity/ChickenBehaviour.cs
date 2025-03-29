using UnityEngine;

public class ChickenBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private float timeToHatch;

    private float currTimeToHatch;
    private bool isEgg;

    public override void OnStartServer() // Make sure server does nothing
    {
        
    }

    protected override void UpdateServer()
    {
        base.UpdateServer();

        if (isEgg)
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
            }
        }
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
    }
}
