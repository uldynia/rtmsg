using UnityEngine;
using Mirror;
public class EntityTransform : NetworkTransformReliable
{
    private bool isOwnedByPlayer = false;

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

                    // interpolate & apply
                    TransformSnapshot computed = TransformSnapshot.Interpolate(from, to, t);
                    Apply(computed, to);
                }
            }
        }
    }

    public void Setup(bool newIsOwned)
    {
        isOwnedByPlayer = newIsOwned;
    }
}
