using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField]
    private int direction;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EntityBaseBehaviour behaviour = collision.GetComponent<EntityBaseBehaviour>();
        if (behaviour)
        {
            if (direction != behaviour.GetDirection())
            {

            }
        }
    }
}
