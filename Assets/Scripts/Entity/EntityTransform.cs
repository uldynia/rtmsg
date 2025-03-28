using UnityEngine;
using Mirror;
public class EntityTransform : NetworkTransformReliable
{
    private bool isOwnedByPlayer = false;
    private bool isSetup = false;

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
                    if (isOwnedByPlayer)
                    {
                         computed = TransformSnapshot.Interpolate(from, to, t);
                    }
                    else
                    {
                        if (GameManager.instance != null) {
                            Vector2 yBoundary = GameManager.instance.GetPlaceableBoundaryY();
                            from.position = new(from.position.x, from.position.y - yBoundary.y + yBoundary.x, from.position.z);
                            to.position = new(to.position.x, to.position.y - yBoundary.y + yBoundary.x, to.position.z);
                        }
                        computed = TransformSnapshot.Interpolate(from, to, t);
                    }
                    Apply(computed, to);
                }
            }
        }
    }

    public void Setup(bool newIsOwned)
    {
        isOwnedByPlayer = newIsOwned;
        isSetup = true;
    }
}
