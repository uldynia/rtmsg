using System.Collections.Generic;
using UnityEngine;

public class SheepBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private List<AnimalType> dragonTypes = new();
    public override void OnStartServer()
    {
        base.OnStartServer();
        if (level >= 3)
        {
            currHp = animalData.Health + (level - 2) * 2;
            ogHp = currHp;
        }

    }
    protected override void OnEncounterEnemy(EntityBaseBehaviour enemy)
    {
        if (direction > 0)
        {
            if (dragonTypes.Contains(enemy.GetData()))
            {
                // Store Hp
                ogHp = currHp;
                // Take 200% of damage  
                OnTakeDamage(enemy.GetHealth() * 2);

                // Deal 0.5x Damage
                enemy.OnTakeDamage(Mathf.FloorToInt(ogHp * 0.5f));
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

        if (level >= 3)
        {
            currHp = animalData.Health + (level - 2) * 2;
            ogHp = currHp;
        }
    }
}
