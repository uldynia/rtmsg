using System.Collections.Generic;
using UnityEngine;

public class CowBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private float MilkGeneratorTimer;

    [SerializeField]
    private GameObject milkPrefab;

    [SerializeField]
    private List<Buff> applyBuffs;

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
        if (level == 3)
        {
            foreach (Buff buff in applyBuffs)
            {
                foreach (EntityBaseBehaviour en in GameManager.instance.entities)
                {
                    en.RemoveBuff(buff);
                }
            }
        }
    }
    public override void Setup(int direction, int level)
    {
        base.Setup(direction, level);

        currentMilkGenerator = MilkGeneratorTimer;
        foreach (Buff buff in applyBuffs)
        {
            buff.entity = this;
            if (level == 3)
            {   
                //provide a global buff to everyone with infinite timer
                foreach (EntityBaseBehaviour en in GameManager.instance.entities)
                {
                    //Check if its same team
                    if (en.GetDirection() == direction)
                    {
                        en.ApplyBuff(buff);
                    }
                }
            }
        }

        GameManager.instance.onEntitySpawn += OnEntitySpawn;
    }
    private void OnEntitySpawn(EntityBaseBehaviour entity)
    {
        //Check if its same team
        if (entity.GetDirection() == direction)
        {
            foreach (Buff buff in applyBuffs)
            {
                entity.ApplyBuff(buff);
            }
        }
    }
    public void OnMilkPickup(EntityBaseBehaviour entity, Consumeable milk)
    {
        foreach (Buff buff in applyBuffs)
        {
            entity.ApplyBuff(buff);
        }
    }
}
