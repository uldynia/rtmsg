using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Spine.Unity;
using Spine;

public class CowBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private float MilkGeneratorTimer;

    [SerializeField]
    private GameObject milkPrefab;

    [SerializeField]
    private List<Buff> applyBuffs;

    [SerializeField]
    private List<Vector2Int> MilkSpawnCoordinate;

    [SerializeField]
    private SkeletonAnimation skeletonAnimation;

    [SerializeField]
    private string milkSpawnAnimationName;

    [SerializeField]
    private string idleAnimationName;

    private float currentMilkGenerator;

    private bool isDoingAnim = false;
    protected override void UpdateServer()
    {
        base.UpdateServer();

        if (currentMilkGenerator > 0)
        {
            currentMilkGenerator -= Time.deltaTime;
            if (currentMilkGenerator <= 0)
            {
                RpcSpawnMilkAnimation();
                currentMilkGenerator = MilkGeneratorTimer;
            }
        }
    }

    public override void OnDeath()
    {
        GameManager.instance.onEntitySpawn -= OnEntitySpawn;
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
        PlayerController.localPlayer.UnregisterStationaryObject(GridManager.instance.GetGridCoordinate(transform.position));
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
                //Check if the buff entity is not null
                if (buff.entity != null)
                {
                    entity.ApplyBuff(buff);
                }
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
        GridManager.instance.spawnedMilk.Remove(GridManager.instance.GetGridCoordinate(transform.position) + new Vector2Int(milk.coord.x, milk.coord.y * direction));
        milk.hasBeenPicked = true;
        NetworkServer.Destroy(milk.gameObject);
    }

    [ClientRpc]
    private void RpcSpawnMilkAnimation()
    {
        if (isDoingAnim)
        {
            return;
        }
        isDoingAnim = true;
        TrackEntry en = skeletonAnimation.AnimationState.Tracks.Items[0];
        en.TrackEnd = en.AnimationTime;
        skeletonAnimation.AnimationState.SetAnimation(0, milkSpawnAnimationName, false);
        skeletonAnimation.AnimationState.End += OnEnd;
    }
    private void OnEnd(TrackEntry entry)
    {
        bool hasSpawnedMilk = false;
        if (isServer) 
        {
            // TODO: SPAWN MILK
            foreach (Vector2Int coord in MilkSpawnCoordinate)
            {
                // Make sure the spawned milk is in bounds
                if (coord.x + GridManager.instance.GetGridCoordinate(transform.position).x >= 0 &&
                    coord.x + GridManager.instance.GetGridCoordinate(transform.position).x < GridManager.instance.GetMap().x &&
                    !GridManager.instance.coveredGrids.Contains(new Vector2Int((int)coord.x + GridManager.instance.GetGridCoordinate(transform.position).x, (int)coord.y * direction + GridManager.instance.GetGridCoordinate(transform.position).y))
                    )
                {
                    if (!GridManager.instance.spawnedMilk.Contains(GridManager.instance.GetGridCoordinate(transform.position) + new Vector2Int(coord.x, coord.y * direction))) // Spawn milk, its in bounds
                    {
                        GameObject milk = Instantiate(milkPrefab, transform.position + new Vector3(coord.x, coord.y * direction, 0), Quaternion.identity);

                        Consumeable milkCon = milk.GetComponent<Consumeable>();
                        milkCon.direction = direction;
                        milkCon.pickUpEvent += OnMilkPickup;
                        milkCon.coord = coord;
                        NetworkServer.Spawn(milk);

                        GridManager.instance.spawnedMilk.Add(GridManager.instance.GetGridCoordinate(transform.position) + new Vector2Int(coord.x, coord.y * direction));
                    }
                }
            }
        }
        if (!hasSpawnedMilk)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, idleAnimationName, true);
            hasSpawnedMilk = true; 
            isDoingAnim = false; 
        }
        skeletonAnimation.AnimationState.End -= OnEnd;
    }
}
