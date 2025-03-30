using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField]
    private int direction;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PlayerController.localPlayer.isServer)
        {
            EntityBaseBehaviour behaviour = collision.GetComponent<EntityBaseBehaviour>();
            if (behaviour)
            {
                if (direction != behaviour.GetDirection())
                {
                    GameManager.instance.ReachedSpawn(behaviour);
                }
            }
        }
    }
}
