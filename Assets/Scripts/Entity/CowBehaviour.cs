using UnityEngine;

public class CowBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private float MilkGeneratorTimer;

    [SerializeField]
    private GameObject milkPrefab;

    private float currentMilkGenerator;

    protected override void UpdateServer()
    {
        base.UpdateServer();

        if (currentMilkGenerator > 0)
        {
            currentMilkGenerator -= Time.deltaTime;
             if (currentMilkGenerator <= 0)
            {
                // TODO: SPAWN MILK
                currentMilkGenerator = MilkGeneratorTimer;
            }
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();

        // Remove one stack of global buff

    }
    public override void Setup(int direction, int level)
    {
        base.Setup(direction, level);

        currentMilkGenerator = MilkGeneratorTimer;

        if (level == 3)
        {
            //provide a global buff to everyone with infinite timer
        }
    }
}
