using Mirror;
using UnityEngine;

public class BullBehaviour : EntityBaseBehaviour
{
    [SerializeField]
    private float laneChangeInterval;

    private float currLaneChange;

    [SerializeField]
    private float laneChangeSpeed;

    private float currLaneSpeed;
    private int laneChangeDir;

    private float targetLaneXPos;
    private float currLaneXPos;
    private bool isChangingLane;

    protected override void UpdateServer()
    {
        base.UpdateServer();
        if (isChangingLane)
        {
            transform.position = new(Mathf.Lerp(currLaneXPos, targetLaneXPos, currLaneSpeed), transform.position.y,transform.position.z);
            currLaneSpeed += Time.deltaTime * laneChangeSpeed;
            if (currLaneSpeed >= 1)
            {
                isChangingLane = false;
                transform.position = new(targetLaneXPos, transform.position.y, transform.position.z);
                currLaneChange = laneChangeInterval;
            }
        }
        else if (currLaneChange > 0)
        {
            currLaneChange -= Time.deltaTime;
            if (currLaneChange <= 0)
            {
                // Set Lane change target
                int currXGrid = GridManager.instance.GetGridCoordinate(transform.position).x;

                if (currXGrid + laneChangeDir < 0 || currXGrid + laneChangeSpeed >= GridManager.instance.GetMap().x)
                {
                    laneChangeDir *= -1;
                }

                currLaneXPos = transform.position.x;
                targetLaneXPos = transform.position.x + GridManager.instance.GetGridSize() * laneChangeDir;

                isChangingLane = true;

                currLaneSpeed = 0;
            }
        }
    }

    public override void Setup(int direction, int level)
    {
        base.Setup(direction, level);

        laneChangeDir = Random.Range(0, 2) == 0 ? -1 : 1;

        currLaneChange = laneChangeInterval;
        isChangingLane = false;

    }
}
