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
                enemy.OnDeath(); // instantly kill any cotton ball of sheeps
            }
            else
            {
                base.OnEncounterEnemy(enemy);
            }
        }
    }
}
