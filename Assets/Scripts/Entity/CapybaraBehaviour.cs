using UnityEngine;

public class CapybaraBehaviour : EntityBaseBehaviour
{
    protected override void OnDeath()
    {
        PlayerController.localPlayer.UnregisterStationaryObject(GridManager.instance.GetGridCoordinate(transform.position));
        base.OnDeath();
    }
}
