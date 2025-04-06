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
                // Store Hp
                ogHp = currHp;
                // Take 50% of damage  
                OnTakeDamage(Mathf.FloorToInt(enemy.GetHealth() * 0.5f));

                // Deal 2x Damage
                enemy.OnTakeDamage(ogHp * 2);
                
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
