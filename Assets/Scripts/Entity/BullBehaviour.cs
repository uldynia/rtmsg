using DG.Tweening;
using Mirror;
using Spine;
using Spine.Unity;
using UnityEngine;

public class BullBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private float laneChangeInterval;

    private float currLaneChange;

    private int laneChangeDir;

    [Header("JumpInfo")]
    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private SpriteRenderer sprite;

    [SerializeField]
    private string jumpAnimationName;
    [SerializeField]
    private string attackAnimationName;
    [SerializeField]
    private SkeletonAnimation skeletonAnimation;

    private float currJumpTime;
    private bool isJumping;
    private bool hasJumped;
    public override void OnStartServer()
    {
        if (level != 2) // level 2 starts at 0 HP so don't take hp stats here
        {
            base.OnStartServer();
        }
    }

    public bool GetHasJumped()
    {
        return hasJumped;
    }

    public bool GetIsJumping()
    {
        return isJumping;
    }
    protected override void UpdateServer()
    {
        base.UpdateServer();
        if (currLaneChange > 0 && level < 2)
        {
            currLaneChange -= Time.deltaTime;
            if (currLaneChange <= 0)
            {
                // Set Lane change target
                int currXGrid = GridManager.instance.GetGridCoordinate(transform.position).x;

                if (currXGrid + laneChangeDir < 0 || currXGrid + laneChangeDir >= GridManager.instance.GetMap().x)
                {
                    laneChangeDir *= -1;
                }

                ChangeLane(laneChangeDir);
            }
        }

        if (currJumpTime > 0)
        {
            currJumpTime -= Time.deltaTime;
            //transform.localScale = new(Mathf.Sin((currJumpTime / jumpTime) * Mathf.PI * 0.5f) + 1, Mathf.Sin((currJumpTime / jumpTime) * Mathf.PI * 0.5f) + 1, 1);
            if (currJumpTime <= 0)
            {
                // Finished jumping, set stats back to normal
                isJumping = false;
                ogHp = animalData.Health;
                currHp = ogHp;
            }
        }
    }
    protected override void OnFinishedLaneChange()
    {
        base.OnFinishedLaneChange();

        currLaneChange = laneChangeInterval;
    }
    public void RemoveJump()
    {
        // Finished jumping, set stats back to normal
        isJumping = false;
        ogHp = animalData.Health;
        currHp = ogHp;
    }
    public override void OnDeath()
    {
        if (!hasJumped)
        {
            // Jump
            hasJumped = true;
            isJumping = true;
            currJumpTime = jumpTime;
            transform.DOScale(new Vector3(2f * transform.localScale.x, 2f * transform.localScale.y, 1f), jumpTime).SetEase(Ease.InOutSine).SetLoops(2, LoopType.Yoyo);
            
            RpcJumpAnimation();
        }
        else if (!isJumping)
        {
            base.OnDeath();
        }
    }
    public override void Setup(int direction, int level)
    {
        base.Setup(direction, level);

        laneChangeDir = Random.Range(0, 2) == 0 ? -1 : 1;

        currLaneChange = laneChangeInterval;
        isChangingLane = false;

        isJumping = false;
        hasJumped = level <= 1;

        ogHp = 0;
        currHp = 0;
        currJumpTime = 0;
        currSpd = animalData.Speed;

        if (level == 3)
        {
            // Fun begins. Shift lanes of all registered entities, but make sure to check boundary
            int bullGridX = GridManager.instance.GetGridCoordinate(transform.position).x;
            foreach (EntityBaseBehaviour behaviour in GameManager.instance.entities)
            {
                int gridX = GridManager.instance.GetGridCoordinate(behaviour.transform.position).x;
                // Check 1. grid number to the left of the bull always goes left unless its 0.
                if (gridX + 1 == bullGridX)
                {
                    if (gridX != 0)
                    {
                        behaviour.ChangeLane(-1);
                    }
                }
                // Check 2. grid number to the right of the bull always goes to right unless its the last grid number
                else if (gridX - 1 == bullGridX)
                {
                    if (gridX != GridManager.instance.GetMap().x - 1)
                    {
                        behaviour.ChangeLane(1);
                    }
                }
                // Check 3. Grid Number 0 always goes right
                else if (gridX == 0)
                {
                    behaviour.ChangeLane(1);
                }
                // Check 4. Last grid number always goes left
                else if (gridX == GridManager.instance.GetMap().x - 1)
                {
                    behaviour.ChangeLane(-1);
                }
                // Randomly shift left or right
                else
                {
                    behaviour.ChangeLane(Random.Range(0, 2) == 0 ? -1 : 1);
                }
            }
        }
    }
    [ClientRpc]
    private void RpcJumpAnimation()
    {
        transform.position -= new Vector3(0, 0, 5);

        TrackEntry en = skeletonAnimation.AnimationState.Tracks.Items[0];
        en.TrackEnd = en.AnimationTime;
        skeletonAnimation.AnimationState.SetAnimation(0, jumpAnimationName, false);
        skeletonAnimation.AnimationState.End += JumpAnimationEnd;
    }

    private void JumpAnimationEnd(TrackEntry entry)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimationName, true); 
        transform.position += new Vector3(0, 0, 5);

        skeletonAnimation.AnimationState.End -= JumpAnimationEnd;
    }
}
