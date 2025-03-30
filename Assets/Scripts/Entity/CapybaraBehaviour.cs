using UnityEngine;

public class CapybaraBehaviour : EntityBaseBehaviour
{
    public override void OnDeath()
    {
        PlayerController.localPlayer.UnregisterStationaryObject(GridManager.instance.GetGridCoordinate(transform.position));
        base.OnDeath();
    }
}
