using Spine.Unity;
using UnityEngine;

public class ObjectToPlace : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public SpriteRenderer sr;

    private void Start()
    {
        if (sr == null)
        {
            sr = GetComponent<SpriteRenderer>();
        }
        if (skeletonAnimation == null)
        {
            skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        }
    }
}
