using UnityEngine;

public class DragonBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private AnimalType cottonBallOfSheeps;
    protected override void OnEncounterEnemy(EntityBaseBehaviour enemy)
    {
        if (direction > 0)
        {
            if (enemy.GetData().EntityID == cottonBallOfSheeps.EntityID)
            {
                // Take 50% of damage
                currHp -= Mathf.FloorToInt(enemy.GetHealth() * 0.5f);
                if (currHp <= 0)
                {
                    OnDeath();
                }
                enemy.OnDeath(); // instantly kill any cotton ball of sheeps
            }
            else
            {
                base.OnEncounterEnemy(enemy);
            }
        }
    }
    public override void Setup(int direction, int level)
    {
        base.Setup(direction, level);

        foreach(EntityBaseBehaviour behaviour in GameManager.instance.entities)
        {
            if (behaviour.GetDirection() == direction)
            {
                ChickenBehaviour chicken = behaviour as ChickenBehaviour;
                if (chicken)
                {
                    chicken.Hatch();
                }
            }
        }
    }
}
