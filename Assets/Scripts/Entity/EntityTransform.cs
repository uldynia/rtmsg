using UnityEngine;
using Mirror;
public class EntityTransform : NetworkTransformReliable
{
    [SerializeField]
    private bool isOwnedByPlayer = false;
    //private bool isSetup = false;

    protected override void UpdateClient()
    {
        if (useFixedUpdate)
        {
            base.UpdateClient();
        }
        else
        {
            // client authority, and local player (= allowed to move myself)?
            if (!IsClientWithAuthority)
            {
                // only while we have snapshots
                if (clientSnapshots.Count > 0)
                {
                    // step the interpolation without touching time.
                    // NetworkClient is responsible for time globally.
                    SnapshotInterpolation.StepInterpolation(
                        clientSnapshots,
                        NetworkTime.time, // == NetworkClient.localTimeline from snapshot interpolation
                        out TransformSnapshot from,
                        out TransformSnapshot to,
                        out double t);
                    TransformSnapshot computed;

                    // interpolate & apply
                    if (GameManager.instance != null) {
                        Vector2 yBoundary = GridManager.instance.GetPlaceableBoundaryY();
                        from.position = new(from.position.x, yBoundary.y - from.position.y + yBoundary.x, from.position.z);
                        to.position = new(to.position.x, yBoundary.y - to.position.y + yBoundary.x, to.position.z);
                    }
                    computed = TransformSnapshot.Interpolate(from, to, t);

                    Apply(computed, to);
                }
            }
        }
    }

    protected override Vector3 GetPosition()
    {
        if (!PlayerController.localPlayer.isServer)
        {
            Vector2 yBoundary = GridManager.instance.GetPlaceableBoundaryY();
            return new(transform.position.x, yBoundary.y - transform.position.y + yBoundary.x, transform.position.z);
        }
        else
        {
            return base.GetPosition();
        }
    }
    public void Setup(bool newIsOwned)
    {
        isOwnedByPlayer = newIsOwned;
       // isSetup = true;
    }
}
