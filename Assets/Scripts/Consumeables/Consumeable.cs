using UnityEngine;
using DG.Tweening;

public class Consumeable : MonoBehaviour
{
    public int direction;
    // Let the owner know its consumeable has been picked up. If there is no owner, then derive a class to use this
    public System.Action<EntityBaseBehaviour,Consumeable> pickUpEvent;
    // identifiable coordinates
    public Vector2Int coord;

    public bool hasBeenPicked = false;

    private void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, -22);
        transform.DOLocalRotate(new Vector3(0, 0, 22), 2.5f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EntityBaseBehaviour behaviour = collision.GetComponent<EntityBaseBehaviour>();
        if (behaviour != null)
        {
            if (behaviour.GetDirection() == direction)
            {
                if (pickUpEvent != null)
                {
                    OnPickup(behaviour);
                }
            }
        }
    }

    protected virtual void OnPickup(EntityBaseBehaviour en)
    {
        pickUpEvent.Invoke(en, this);
    }
}
