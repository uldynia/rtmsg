using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class CowBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private float MilkGeneratorTimer;

    [SerializeField]
    private GameObject milkPrefab;

    [SerializeField]
    private List<Buff> applyBuffs;

    [SerializeField]
    private List<Vector2> MilkSpawnCoordinate;

    private float currentMilkGenerator;

    private List<Vector2> spawnedMilk = new();
    protected override void UpdateServer()
    {
        base.UpdateServer();

        if (currentMilkGenerator > 0)
        {
            currentMilkGenerator -= Time.deltaTime;
            if (currentMilkGenerator <= 0)
            {
                // TODO: SPAWN MILK
                foreach(Vector2 coord in MilkSpawnCoordinate)
                {
                    // Make sure the spawned milk is in bounds
                    if (coord.x + GridManager.instance.GetGridCoordinate(transform.position).x >= 0 &&
                        coord.x + GridManager.instance.GetGridCoordinate(transform.position).x < GridManager.instance.GetMap().x &&
                        !GridManager.instance.coveredGrids.Contains(new Vector2Int((int)coord.x + GridManager.instance.GetGridCoordinate(transform.position).x, (int)coord.y + GridManager.instance.GetGridCoordinate(transform.position).y))
                        )
                    {
                        if (!spawnedMilk.Contains(coord)) // Spawn milk, its in bounds
                        {
                            GameObject milk = Instantiate(milkPrefab, transform.position + new Vector3(coord.x, coord.y, 0) * direction, Quaternion.identity);

                            Consumeable milkCon = milk.GetComponent<Consumeable>();
                            milkCon.direction = direction;
                            milkCon.pickUpEvent += OnMilkPickup;
                            milkCon.coord = coord;
                            NetworkServer.Spawn(milk);

                            spawnedMilk.Add(coord);
                        }
                    }
                }
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
                GameManager.instance.onEntitySpawn += OnEntitySpawn;
            }
        }
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
        if (milk.hasBeenPicked) // Milk has been used
        {
            return;
        }
        foreach (Buff buff in applyBuffs)
        {
            if (!entity.HasBuff(buff))
            {
                entity.ApplyBuff(buff);
            }
        }
        spawnedMilk.Remove(milk.coord);
        milk.hasBeenPicked = true;
        NetworkServer.Destroy(milk.gameObject);
    }
}
